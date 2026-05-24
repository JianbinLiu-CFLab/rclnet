using System;

namespace Rcl.Unity
{
    public sealed unsafe class RclUnityPublisher<T> : IDisposable, IRclUnityNodeEntity
    {
        private readonly object mutex = new object();
        private readonly RclUnityNode node;
        private readonly NativeMessageBridge<T> bridge;
        private NativeTypes.rcl_publisher_t publisher;
        private bool disposed;

        internal RclUnityPublisher(RclUnityNode node, string topicName, NativeMessageBridge<T> bridge)
        {
            this.node = node;
            this.bridge = bridge;
            TopicName = topicName;
            publisher = node.InitializePublisher(topicName, bridge.TypeSupport);
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

        public void Publish(T message)
        {
            node.Publish(this, message);
        }

        public void Dispose()
        {
            node.ReleasePublisher(this);
            GC.SuppressFinalize(this);
        }

        internal void PublishFromNode(T value)
        {
            lock (mutex)
            {
                ThrowIfDisposed();

                var message = bridge.Create();
                try
                {
                    bridge.Write(message, value);

                    fixed (NativeTypes.rcl_publisher_t* publisherPointer = &publisher)
                    {
                        RclError.ThrowIfNonSuccess(
                            NativeRcl.rcl_publish(publisherPointer, message, null),
                            "rcl_publish");
                    }
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

                fixed (NativeTypes.rcl_publisher_t* publisherPointer = &publisher)
                {
                    RclError.ThrowIfNonSuccess(
                        NativeRcl.rcl_publisher_fini(publisherPointer, nodePointer),
                        "rcl_publisher_fini");
                }

                publisher = NativeRcl.rcl_get_zero_initialized_publisher();
                disposed = true;
            }

            GC.SuppressFinalize(this);
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(RclUnityPublisher<T>));
            }
        }
    }
}
