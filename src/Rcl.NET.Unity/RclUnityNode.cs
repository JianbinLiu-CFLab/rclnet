using System;
using System.Collections.Generic;

namespace Rcl.Unity
{
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

        internal RclUnityNode(RclUnityContext context, string name, string namespaceName)
        {
            this.context = context;
            Name = name;
            Namespace = namespaceName;
            node = context.InitializeNode(name, namespaceName);
        }

        public string Name { get; }

        public string Namespace { get; }

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

        public void Dispose()
        {
            context.ReleaseNode(this);
            GC.SuppressFinalize(this);
        }

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

        public RclUnityPublisher<RclUnityTime> CreateTimePublisher(string topicName)
        {
            return CreatePublisher(topicName, BuiltinTimeMessageBridge.Instance);
        }

        public RclUnitySubscription<RclUnityTime> CreateTimeSubscription(string topicName)
        {
            return CreateSubscription(topicName, BuiltinTimeMessageBridge.Instance);
        }

        public RclUnityPublisher<RclUnityHeader> CreateHeaderPublisher(string topicName)
        {
            return CreatePublisher(topicName, StdHeaderMessageBridge.Instance);
        }

        public RclUnitySubscription<RclUnityHeader> CreateHeaderSubscription(string topicName)
        {
            return CreateSubscription(topicName, StdHeaderMessageBridge.Instance);
        }

        public RclUnityPublisher<RclUnityVector3> CreateVector3Publisher(string topicName)
        {
            return CreatePublisher(topicName, GeometryVector3MessageBridge.Instance);
        }

        public RclUnitySubscription<RclUnityVector3> CreateVector3Subscription(string topicName)
        {
            return CreateSubscription(topicName, GeometryVector3MessageBridge.Instance);
        }

        public RclUnityPublisher<RclUnityQuaternion> CreateQuaternionPublisher(string topicName)
        {
            return CreatePublisher(topicName, GeometryQuaternionMessageBridge.Instance);
        }

        public RclUnitySubscription<RclUnityQuaternion> CreateQuaternionSubscription(string topicName)
        {
            return CreateSubscription(topicName, GeometryQuaternionMessageBridge.Instance);
        }

        public RclUnityPublisher<RclUnityPoint> CreatePointPublisher(string topicName)
        {
            return CreatePublisher(topicName, GeometryPointMessageBridge.Instance);
        }

        public RclUnitySubscription<RclUnityPoint> CreatePointSubscription(string topicName)
        {
            return CreateSubscription(topicName, GeometryPointMessageBridge.Instance);
        }

        public RclUnityPublisher<RclUnityPose> CreatePosePublisher(string topicName)
        {
            return CreatePublisher(topicName, GeometryPoseMessageBridge.Instance);
        }

        public RclUnitySubscription<RclUnityPose> CreatePoseSubscription(string topicName)
        {
            return CreateSubscription(topicName, GeometryPoseMessageBridge.Instance);
        }

        internal NativeTypes.rcl_publisher_t InitializeStringPublisher(string topicName)
        {
            return InitializePublisher(topicName, NativeRcl.std_msgs_msg_string_get_type_support());
        }

        internal NativeTypes.rcl_subscription_t InitializeStringSubscription(string topicName)
        {
            return InitializeSubscription(topicName, NativeRcl.std_msgs_msg_string_get_type_support());
        }

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

        private static void ValidateTopicName(string topicName)
        {
            if (string.IsNullOrEmpty(topicName))
            {
                throw new ArgumentException("Topic name must not be null or empty.", nameof(topicName));
            }
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(RclUnityNode));
            }
        }
    }
}
