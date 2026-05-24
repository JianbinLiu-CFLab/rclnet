// Copyright (c) 2026 Jianbin Liu.
// Licensed under the MIT License.
// See LICENSE in the repository root for license information.
using System;

namespace Rcl.Unity
{
    /// <summary>
    /// Publishes typed Unity adapter messages through an rcl publisher handle.
    /// </summary>
    /// <typeparam name="T">The managed Unity adapter message type.</typeparam>
    public sealed unsafe class RclUnityPublisher<T> : IDisposable, IRclUnityNodeEntity
    {
        private readonly object mutex = new object();
        private readonly RclUnityNode node;
        private readonly NativeMessageBridge<T> bridge;
        private NativeTypes.rcl_publisher_t publisher;
        private bool disposed;

        /// <summary>
        /// Initializes a typed publisher owned by a Unity adapter node.
        /// </summary>
        internal RclUnityPublisher(RclUnityNode node, string topicName, NativeMessageBridge<T> bridge)
        {
            this.node = node;
            this.bridge = bridge;
            TopicName = topicName;
            publisher = node.InitializePublisher(topicName, bridge.TypeSupport);
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
        /// Publishes a managed message value.
        /// </summary>
        /// <param name="message">The managed message value to publish.</param>
        public void Publish(T message)
        {
            node.Publish(this, message);
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
        /// Publishes a message while the owning node lock is held.
        /// </summary>
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
                throw new ObjectDisposedException(nameof(RclUnityPublisher<T>));
            }
        }
    }
}
