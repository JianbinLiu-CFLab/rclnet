// Copyright (c) 2026 Jianbin Liu.
// Licensed under the MIT License.
// See LICENSE in the repository root for license information.
using System;

namespace Rcl.Unity
{
    /// <summary>
    /// Takes std_msgs/String values through the lightweight Unity adapter.
    /// </summary>
    public sealed unsafe class RclUnityStringSubscription : IDisposable
    {
        private readonly object mutex = new object();
        private readonly RclUnityNode node;
        private NativeTypes.rcl_subscription_t subscription;
        private bool disposed;

        /// <summary>
        /// Initializes a string subscription owned by a Unity adapter node.
        /// </summary>
        internal RclUnityStringSubscription(RclUnityNode node, string topicName)
        {
            this.node = node;
            TopicName = topicName;
            subscription = node.InitializeStringSubscription(topicName);
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
        /// Attempts to take one string payload from the subscription.
        /// </summary>
        /// <param name="data">The string payload when one was available.</param>
        /// <returns><see langword="true"/> when a message was taken; otherwise <see langword="false"/>.</returns>
        public bool TryTake(out string? data)
        {
            return node.TryTakeString(this, out data);
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
        /// Attempts to take one string value while the owning node lock is held.
        /// </summary>
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
                throw new ObjectDisposedException(nameof(RclUnityStringSubscription));
            }
        }
    }
}
