using System;

namespace Rcl.Unity
{
    public sealed unsafe class RclUnityStringSubscription : IDisposable
    {
        private readonly object mutex = new object();
        private readonly RclUnityNode node;
        private NativeTypes.rcl_subscription_t subscription;
        private bool disposed;

        internal RclUnityStringSubscription(RclUnityNode node, string topicName)
        {
            this.node = node;
            TopicName = topicName;
            subscription = node.InitializeStringSubscription(topicName);
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

        public bool TryTake(out string? data)
        {
            return node.TryTakeString(this, out data);
        }

        public void Dispose()
        {
            node.ReleaseSubscription(this);
            GC.SuppressFinalize(this);
        }

        internal bool TryTakeFromNode(out string? data)
        {
            lock (mutex)
            {
                ThrowIfDisposed();

                var message = RclUnityStringMessage.Create();
                try
                {
                    fixed (NativeTypes.rcl_subscription_t* subscriptionPointer = &subscription)
                    {
                        var ret = NativeRcl.rcl_take(subscriptionPointer, message, null, null);
                        if (ret == RclReturnCode.SubscriptionTakeFailed)
                        {
                            data = null;
                            return false;
                        }

                        RclError.ThrowIfNonSuccess(ret, "rcl_take");
                    }

                    data = RclUnityStringMessage.Read(message);
                    return true;
                }
                finally
                {
                    RclUnityStringMessage.Destroy(message);
                }
            }
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
                throw new ObjectDisposedException(nameof(RclUnityStringSubscription));
            }
        }
    }
}
