// Copyright (c) 2026 Jianbin Liu.
// Licensed under the MIT License.
// See LICENSE in the repository root for license information.
using System;

namespace Rcl.Unity
{
    /// <summary>
    /// Takes typed Unity adapter messages from an rcl subscription handle.
    /// </summary>
    /// <typeparam name="T">The managed Unity adapter message type.</typeparam>
    public sealed unsafe class RclUnitySubscription<T> : IDisposable, IRclUnityNodeEntity
    {
        private readonly object mutex = new object();
        private readonly RclUnityNode node;
        private readonly NativeMessageBridge<T> bridge;
        private NativeTypes.rcl_subscription_t subscription;
        private bool disposed;

        /// <summary>
        /// Initializes a typed subscription owned by a Unity adapter node.
        /// </summary>
        internal RclUnitySubscription(RclUnityNode node, string topicName, NativeMessageBridge<T> bridge)
        {
            this.node = node;
            this.bridge = bridge;
            TopicName = topicName;
            subscription = node.InitializeSubscription(topicName, bridge.TypeSupport);
        }

        /// <summary>
        /// Gets the ROS 2 topic name used by this subscription.
        /// </summary>
        public string TopicName { get; }

        /// <summary>
        /// Gets whether this subscription has already been disposed.
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
        /// Attempts to take one managed message from the subscription.
        /// </summary>
        /// <param name="message">The managed message value when one was available.</param>
        /// <returns><see langword="true"/> when a message was taken; otherwise <see langword="false"/>.</returns>
        public bool TryTake(out T message)
        {
            return node.TryTake(this, out message);
        }

        /// <summary>
        /// Releases this subscription through its owning node.
        /// </summary>
        public void Dispose()
        {
            node.ReleaseSubscription(this);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Attempts to take one message while the owning node lock is held.
        /// </summary>
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

        /// <summary>
        /// Finalizes the native subscription handle from the owning node.
        /// </summary>
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

        /// <summary>
        /// Throws when the subscription has already been disposed.
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(RclUnitySubscription<T>));
            }
        }
    }
}
