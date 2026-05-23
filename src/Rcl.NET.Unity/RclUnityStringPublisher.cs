using System;

namespace Rcl.Unity
{
    public sealed unsafe class RclUnityStringPublisher : IDisposable
    {
        private readonly object mutex = new object();
        private readonly RclUnityNode node;
        private NativeTypes.rcl_publisher_t publisher;
        private bool disposed;

        internal RclUnityStringPublisher(RclUnityNode node, string topicName)
        {
            this.node = node;
            TopicName = topicName;
            publisher = node.InitializeStringPublisher(topicName);
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

        public void Publish(string data)
        {
            node.PublishString(this, data);
        }

        public void Dispose()
        {
            node.ReleasePublisher(this);
            GC.SuppressFinalize(this);
        }

        internal void PublishFromNode(string data)
        {
            lock (mutex)
            {
                ThrowIfDisposed();

                var message = RclUnityStringMessage.Create();
                try
                {
                    RclUnityStringMessage.Assign(message, data);

                    fixed (NativeTypes.rcl_publisher_t* publisherPointer = &publisher)
                    {
                        RclError.ThrowIfNonSuccess(
                            NativeRcl.rcl_publish(publisherPointer, message, null),
                            "rcl_publish");
                    }
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
                throw new ObjectDisposedException(nameof(RclUnityStringPublisher));
            }
        }
    }
}
