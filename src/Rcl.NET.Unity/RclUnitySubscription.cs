using System;

namespace Rcl.Unity
{
    public sealed unsafe class RclUnitySubscription<T> : IDisposable, IRclUnityNodeEntity
    {
        private readonly object mutex = new object();
        private readonly RclUnityNode node;
        private readonly NativeMessageBridge<T> bridge;
        private NativeTypes.rcl_subscription_t subscription;
        private bool disposed;

        internal RclUnitySubscription(RclUnityNode node, string topicName, NativeMessageBridge<T> bridge)
        {
            this.node = node;
            this.bridge = bridge;
            TopicName = topicName;
            subscription = node.InitializeSubscription(topicName, bridge.TypeSupport);
        }

        public string TopicName { get; }

        public bool IsDisposed
        {
            get
            {
                lock (mutex)
                {
                    return disposed;
                }
            }
        }

        public bool TryTake(out T message)
        {
            return node.TryTake(this, out message);
        }

        public void Dispose()
        {
            node.ReleaseSubscription(this);
            GC.SuppressFinalize(this);
        }

        internal bool TryTakeFromNode(out T data)
        {
            lock (mutex)
            {
                ThrowIfDisposed();

                var message = bridge.Create();
                try
                {
                    fixed (NativeTypes.rcl_subscription_t* subscriptionPointer = &subscription)
                    {
                        var ret = NativeRcl.rcl_take(subscriptionPointer, message, null, null);
                        if (ret == RclReturnCode.SubscriptionTakeFailed)
                        {
                            data = default!;
                            return false;
                        }

                        RclError.ThrowIfNonSuccess(ret, "rcl_take");
                    }

                    data = bridge.Read(message);
                    return true;
                }
                finally
                {
                    bridge.Destroy(message);
                }
            }
        }

        void IRclUnityNodeEntity.DisposeFromNode(NativeTypes.rcl_node_t* nodePointer)
        {
            DisposeFromNode(nodePointer);
        }

        internal void DisposeFromNode(NativeTypes.rcl_node_t* nodePointer)
        {
            lock (mutex)
            {
                if (disposed)
                {
                    return;
                }

                fixed (NativeTypes.rcl_subscription_t* subscriptionPointer = &subscription)
                {
                    RclError.ThrowIfNonSuccess(
                        NativeRcl.rcl_subscription_fini(subscriptionPointer, nodePointer),
                        "rcl_subscription_fini");
                }

                subscription = NativeRcl.rcl_get_zero_initialized_subscription();
                disposed = true;
            }

            GC.SuppressFinalize(this);
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(RclUnitySubscription<T>));
            }
        }
    }
}
