// Copyright (c) 2026 Jianbin Liu.
// Licensed under the MIT License.
// See LICENSE in the repository root for license information.
using System;
using System.Collections.Generic;

namespace Rcl.Unity
{
    /// <summary>
    /// Represents a Unity-scoped ROS 2 node and owns publishers/subscriptions created through it.
    /// </summary>
    public sealed unsafe class RclUnityNode : IDisposable
    {
        private readonly object mutex = new object();
        private readonly RclUnityContext context;
        private readonly HashSet<RclUnityStringPublisher> stringPublishers = new HashSet<RclUnityStringPublisher>();
        private readonly HashSet<RclUnityStringSubscription> stringSubscriptions = new HashSet<RclUnityStringSubscription>();
        private readonly HashSet<IRclUnityNodeEntity> publishers = new HashSet<IRclUnityNodeEntity>();
        private readonly HashSet<IRclUnityNodeEntity> subscriptions = new HashSet<IRclUnityNodeEntity>();
        private NativeTypes.rcl_node_t node;
        private bool disposed;

        /// <summary>
        /// Initializes a node from an already initialized adapter context.
        /// </summary>
        /// <param name="context">The context that owns the native node.</param>
        /// <param name="name">The node name.</param>
        /// <param name="namespaceName">The node namespace.</param>
        internal RclUnityNode(RclUnityContext context, string name, string namespaceName)
        {
            this.context = context;
            Name = name;
            Namespace = namespaceName;
            node = context.InitializeNode(name, namespaceName);
        }

        /// <summary>
        /// Gets the ROS 2 node name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the ROS 2 namespace.
        /// </summary>
        public string Namespace { get; }

        /// <summary>
        /// Gets whether the node has already been disposed.
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
        /// Releases this node through its owning context.
        /// </summary>
        public void Dispose()
        {
            context.ReleaseNode(this);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Creates a publisher for std_msgs/String.
        /// </summary>
        /// <param name="topicName">The ROS 2 topic name.</param>
        /// <returns>A string publisher owned by this node.</returns>
        public RclUnityStringPublisher CreateStringPublisher(string topicName)
        {
            ValidateTopicName(topicName);

            lock (mutex)
            {
                ThrowIfDisposed();

                var publisher = new RclUnityStringPublisher(this, topicName);
                stringPublishers.Add(publisher);
                return publisher;
            }
        }

        /// <summary>
        /// Creates a subscription for std_msgs/String.
        /// </summary>
        /// <param name="topicName">The ROS 2 topic name.</param>
        /// <returns>A string subscription owned by this node.</returns>
        public RclUnityStringSubscription CreateStringSubscription(string topicName)
        {
            ValidateTopicName(topicName);

            lock (mutex)
            {
                ThrowIfDisposed();

                var subscription = new RclUnityStringSubscription(this, topicName);
                stringSubscriptions.Add(subscription);
                return subscription;
            }
        }

        /// <summary>
        /// Creates a publisher for builtin_interfaces/Time.
        /// </summary>
        public RclUnityPublisher<RclUnityTime> CreateTimePublisher(string topicName)
        {
            return CreatePublisher(topicName, BuiltinTimeMessageBridge.Instance);
        }

        /// <summary>
        /// Creates a subscription for builtin_interfaces/Time.
        /// </summary>
        public RclUnitySubscription<RclUnityTime> CreateTimeSubscription(string topicName)
        {
            return CreateSubscription(topicName, BuiltinTimeMessageBridge.Instance);
        }

        /// <summary>
        /// Creates a publisher for std_msgs/Header.
        /// </summary>
        public RclUnityPublisher<RclUnityHeader> CreateHeaderPublisher(string topicName)
        {
            return CreatePublisher(topicName, StdHeaderMessageBridge.Instance);
        }

        /// <summary>
        /// Creates a subscription for std_msgs/Header.
        /// </summary>
        public RclUnitySubscription<RclUnityHeader> CreateHeaderSubscription(string topicName)
        {
            return CreateSubscription(topicName, StdHeaderMessageBridge.Instance);
        }

        /// <summary>
        /// Creates a publisher for geometry_msgs/Vector3.
        /// </summary>
        public RclUnityPublisher<RclUnityVector3> CreateVector3Publisher(string topicName)
        {
            return CreatePublisher(topicName, GeometryVector3MessageBridge.Instance);
        }

        /// <summary>
        /// Creates a subscription for geometry_msgs/Vector3.
        /// </summary>
        public RclUnitySubscription<RclUnityVector3> CreateVector3Subscription(string topicName)
        {
            return CreateSubscription(topicName, GeometryVector3MessageBridge.Instance);
        }

        /// <summary>
        /// Creates a publisher for geometry_msgs/Quaternion.
        /// </summary>
        public RclUnityPublisher<RclUnityQuaternion> CreateQuaternionPublisher(string topicName)
        {
            return CreatePublisher(topicName, GeometryQuaternionMessageBridge.Instance);
        }

        /// <summary>
        /// Creates a subscription for geometry_msgs/Quaternion.
        /// </summary>
        public RclUnitySubscription<RclUnityQuaternion> CreateQuaternionSubscription(string topicName)
        {
            return CreateSubscription(topicName, GeometryQuaternionMessageBridge.Instance);
        }

        /// <summary>
        /// Creates a publisher for geometry_msgs/Point.
        /// </summary>
        public RclUnityPublisher<RclUnityPoint> CreatePointPublisher(string topicName)
        {
            return CreatePublisher(topicName, GeometryPointMessageBridge.Instance);
        }

        /// <summary>
        /// Creates a subscription for geometry_msgs/Point.
        /// </summary>
        public RclUnitySubscription<RclUnityPoint> CreatePointSubscription(string topicName)
        {
            return CreateSubscription(topicName, GeometryPointMessageBridge.Instance);
        }

        /// <summary>
        /// Creates a publisher for geometry_msgs/Pose.
        /// </summary>
        public RclUnityPublisher<RclUnityPose> CreatePosePublisher(string topicName)
        {
            return CreatePublisher(topicName, GeometryPoseMessageBridge.Instance);
        }

        /// <summary>
        /// Creates a subscription for geometry_msgs/Pose.
        /// </summary>
        public RclUnitySubscription<RclUnityPose> CreatePoseSubscription(string topicName)
        {
            return CreateSubscription(topicName, GeometryPoseMessageBridge.Instance);
        }

        /// <summary>
        /// Initializes a native std_msgs/String publisher.
        /// </summary>
        internal NativeTypes.rcl_publisher_t InitializeStringPublisher(string topicName)
        {
            return InitializePublisher(topicName, NativeRcl.std_msgs_msg_string_get_type_support());
        }

        /// <summary>
        /// Initializes a native std_msgs/String subscription.
        /// </summary>
        internal NativeTypes.rcl_subscription_t InitializeStringSubscription(string topicName)
        {
            return InitializeSubscription(topicName, NativeRcl.std_msgs_msg_string_get_type_support());
        }

        /// <summary>
        /// Initializes a native publisher for a typed message bridge.
        /// </summary>
        internal NativeTypes.rcl_publisher_t InitializePublisher(string topicName, IntPtr typeSupport)
        {
            if (typeSupport == IntPtr.Zero)
            {
                throw new RclUnityException("Message type support lookup returned null.");
            }

            var nativePublisher = NativeRcl.rcl_get_zero_initialized_publisher();
            var options = NativeRcl.rcl_publisher_get_default_options();
            var topicSize = NativeString.GetUtf8BufferSize(topicName);
            Span<byte> topicBuffer = stackalloc byte[topicSize];
            NativeString.FillUtf8Buffer(topicName, topicBuffer);

            fixed (byte* topicPointer = topicBuffer)
            fixed (NativeTypes.rcl_node_t* nodePointer = &node)
            {
                var publisherPointer = &nativePublisher;
                var optionsPointer = &options;
                RclError.ThrowIfNonSuccess(
                    NativeRcl.rcl_publisher_init(publisherPointer, nodePointer, typeSupport, topicPointer, optionsPointer),
                    "rcl_publisher_init");
            }

            return nativePublisher;
        }

        /// <summary>
        /// Initializes a native subscription for a typed message bridge.
        /// </summary>
        internal NativeTypes.rcl_subscription_t InitializeSubscription(string topicName, IntPtr typeSupport)
        {
            if (typeSupport == IntPtr.Zero)
            {
                throw new RclUnityException("Message type support lookup returned null.");
            }

            var nativeSubscription = NativeRcl.rcl_get_zero_initialized_subscription();
            var options = NativeRcl.rcl_subscription_get_default_options();
            var topicSize = NativeString.GetUtf8BufferSize(topicName);
            Span<byte> topicBuffer = stackalloc byte[topicSize];
            NativeString.FillUtf8Buffer(topicName, topicBuffer);

            try
            {
                fixed (byte* topicPointer = topicBuffer)
                fixed (NativeTypes.rcl_node_t* nodePointer = &node)
                {
                    var subscriptionPointer = &nativeSubscription;
                    var optionsPointer = &options;
                    RclError.ThrowIfNonSuccess(
                        NativeRcl.rcl_subscription_init(subscriptionPointer, nodePointer, typeSupport, topicPointer, optionsPointer),
                        "rcl_subscription_init");
                }

                return nativeSubscription;
            }
            finally
            {
                var optionsPointer = &options;
                RclError.ThrowIfNonSuccess(
                    NativeRcl.rcl_subscription_options_fini(optionsPointer),
                    "rcl_subscription_options_fini");
            }
        }

        /// <summary>
        /// Publishes a std_msgs/String value through an owned string publisher.
        /// </summary>
        internal void PublishString(RclUnityStringPublisher publisher, string data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            lock (mutex)
            {
                ThrowIfDisposed();
                if (!stringPublishers.Contains(publisher))
                {
                    throw new ObjectDisposedException(nameof(RclUnityStringPublisher));
                }

                publisher.PublishFromNode(data);
            }
        }

        /// <summary>
        /// Attempts to take a std_msgs/String value from an owned string subscription.
        /// </summary>
        internal bool TryTakeString(RclUnityStringSubscription subscription, out string? data)
        {
            lock (mutex)
            {
                ThrowIfDisposed();
                if (!stringSubscriptions.Contains(subscription))
                {
                    throw new ObjectDisposedException(nameof(RclUnityStringSubscription));
                }

                return subscription.TryTakeFromNode(out data);
            }
        }

        /// <summary>
        /// Removes and disposes an owned string publisher.
        /// </summary>
        internal void ReleasePublisher(RclUnityStringPublisher publisher)
        {
            lock (mutex)
            {
                if (disposed || !stringPublishers.Remove(publisher))
                {
                    return;
                }

                fixed (NativeTypes.rcl_node_t* nodePointer = &node)
                {
                    publisher.DisposeFromNode(nodePointer);
                }
            }
        }

        /// <summary>
        /// Removes and disposes an owned string subscription.
        /// </summary>
        internal void ReleaseSubscription(RclUnityStringSubscription subscription)
        {
            lock (mutex)
            {
                if (disposed || !stringSubscriptions.Remove(subscription))
                {
                    return;
                }

                fixed (NativeTypes.rcl_node_t* nodePointer = &node)
                {
                    subscription.DisposeFromNode(nodePointer);
                }
            }
        }

        /// <summary>
        /// Publishes a bridge-backed message through an owned typed publisher.
        /// </summary>
        internal void Publish<T>(RclUnityPublisher<T> publisher, T message)
        {
            lock (mutex)
            {
                ThrowIfDisposed();
                if (!publishers.Contains(publisher))
                {
                    throw new ObjectDisposedException(nameof(RclUnityPublisher<T>));
                }

                publisher.PublishFromNode(message);
            }
        }

        /// <summary>
        /// Attempts to take a bridge-backed message from an owned typed subscription.
        /// </summary>
        internal bool TryTake<T>(RclUnitySubscription<T> subscription, out T message)
        {
            lock (mutex)
            {
                ThrowIfDisposed();
                if (!subscriptions.Contains(subscription))
                {
                    throw new ObjectDisposedException(nameof(RclUnitySubscription<T>));
                }

                return subscription.TryTakeFromNode(out message);
            }
        }

        /// <summary>
        /// Removes and disposes an owned typed publisher.
        /// </summary>
        internal void ReleasePublisher<T>(RclUnityPublisher<T> publisher)
        {
            lock (mutex)
            {
                if (disposed || !publishers.Remove(publisher))
                {
                    return;
                }

                fixed (NativeTypes.rcl_node_t* nodePointer = &node)
                {
                    publisher.DisposeFromNode(nodePointer);
                }
            }
        }

        /// <summary>
        /// Removes and disposes an owned typed subscription.
        /// </summary>
        internal void ReleaseSubscription<T>(RclUnitySubscription<T> subscription)
        {
            lock (mutex)
            {
                if (disposed || !subscriptions.Remove(subscription))
                {
                    return;
                }

                fixed (NativeTypes.rcl_node_t* nodePointer = &node)
                {
                    subscription.DisposeFromNode(nodePointer);
                }
            }
        }

        /// <summary>
        /// Disposes the node and all children without calling back into the owning context.
        /// </summary>
        internal void DisposeFromContext()
        {
            lock (mutex)
            {
                if (disposed)
                {
                    return;
                }

                fixed (NativeTypes.rcl_node_t* nodePointer = &node)
                {
                    foreach (var subscription in subscriptions)
                    {
                        subscription.DisposeFromNode(nodePointer);
                    }

                    subscriptions.Clear();

                    foreach (var subscription in stringSubscriptions)
                    {
                        subscription.DisposeFromNode(nodePointer);
                    }

                    stringSubscriptions.Clear();

                    foreach (var publisher in publishers)
                    {
                        publisher.DisposeFromNode(nodePointer);
                    }

                    publishers.Clear();

                    foreach (var publisher in stringPublishers)
                    {
                        publisher.DisposeFromNode(nodePointer);
                    }

                    stringPublishers.Clear();

                    RclError.ThrowIfNonSuccess(NativeRcl.rcl_node_fini(nodePointer), "rcl_node_fini");
                }

                node = NativeRcl.rcl_get_zero_initialized_node();
                disposed = true;
            }

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Creates and tracks a typed publisher backed by a native message bridge.
        /// </summary>
        private RclUnityPublisher<T> CreatePublisher<T>(string topicName, NativeMessageBridge<T> bridge)
        {
            ValidateTopicName(topicName);

            lock (mutex)
            {
                ThrowIfDisposed();

                var publisher = new RclUnityPublisher<T>(this, topicName, bridge);
                publishers.Add(publisher);
                return publisher;
            }
        }

        /// <summary>
        /// Creates and tracks a typed subscription backed by a native message bridge.
        /// </summary>
        private RclUnitySubscription<T> CreateSubscription<T>(string topicName, NativeMessageBridge<T> bridge)
        {
            ValidateTopicName(topicName);

            lock (mutex)
            {
                ThrowIfDisposed();

                var subscription = new RclUnitySubscription<T>(this, topicName, bridge);
                subscriptions.Add(subscription);
                return subscription;
            }
        }

        /// <summary>
        /// Validates a topic name before handing it to rcl.
        /// </summary>
        private static void ValidateTopicName(string topicName)
        {
            if (string.IsNullOrEmpty(topicName))
            {
                throw new ArgumentException("Topic name must not be null or empty.", nameof(topicName));
            }
        }

        /// <summary>
        /// Throws when the node has already been disposed.
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(RclUnityNode));
            }
        }
    }
}
