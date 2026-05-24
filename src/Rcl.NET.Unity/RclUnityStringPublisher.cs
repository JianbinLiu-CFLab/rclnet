// Copyright (c) 2026 Jianbin Liu.
// Licensed under the MIT License.
// See LICENSE in the repository root for license information.
using System;

namespace Rcl.Unity
{
    /// <summary>
    /// Publishes std_msgs/String values through the lightweight Unity adapter.
    /// </summary>
    public sealed unsafe class RclUnityStringPublisher : IDisposable
    {
        private readonly object mutex = new object();
        private readonly RclUnityNode node;
        private NativeTypes.rcl_publisher_t publisher;
        private bool disposed;

        /// <summary>
        /// Initializes a string publisher owned by a Unity adapter node.
        /// </summary>
        internal RclUnityStringPublisher(RclUnityNode node, string topicName)
        {
            this.node = node;
            TopicName = topicName;
            publisher = node.InitializeStringPublisher(topicName);
        }

        /// <summary>
        /// Gets the ROS 2 topic name used by this publisher.
        /// </summary>
        public string TopicName { get; }

        /// <summary>
        /// Gets whether this publisher has already been disposed.
        /// </summary>
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

        /// <summary>
        /// Publishes a string value.
        /// </summary>
        /// <param name="data">The string payload to publish.</param>
        public void Publish(string data)
        {
            node.PublishString(this, data);
        }

        /// <summary>
        /// Releases this publisher through its owning node.
        /// </summary>
        public void Dispose()
        {
            node.ReleasePublisher(this);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Publishes a string value while the owning node lock is held.
        /// </summary>
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

        /// <summary>
        /// Finalizes the native publisher handle from the owning node.
        /// </summary>
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

        /// <summary>
        /// Throws when the publisher has already been disposed.
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(RclUnityStringPublisher));
            }
        }
    }
}
