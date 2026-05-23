using System;
using System.Collections.Generic;

namespace Rcl.Unity
{
    public sealed unsafe class RclUnityNode : IDisposable
    {
        private readonly object mutex = new object();
        private readonly RclUnityContext context;
        private readonly HashSet<RclUnityStringPublisher> publishers = new HashSet<RclUnityStringPublisher>();
        private readonly HashSet<RclUnityStringSubscription> subscriptions = new HashSet<RclUnityStringSubscription>();
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
                publishers.Add(publisher);
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
                subscriptions.Add(subscription);
                return subscription;
            }
        }

        internal NativeTypes.rcl_publisher_t InitializeStringPublisher(string topicName)
        {
            var nativePublisher = NativeRcl.rcl_get_zero_initialized_publisher();
            var options = NativeRcl.rcl_publisher_get_default_options();
            var topicSize = NativeString.GetUtf8BufferSize(topicName);
            Span<byte> topicBuffer = stackalloc byte[topicSize];
            NativeString.FillUtf8Buffer(topicName, topicBuffer);
            var typeSupport = NativeRcl.std_msgs_msg_string_get_type_support();

            if (typeSupport == IntPtr.Zero)
            {
                throw new RclUnityException("std_msgs/String type support lookup returned null.");
            }

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

        internal NativeTypes.rcl_subscription_t InitializeStringSubscription(string topicName)
        {
            var nativeSubscription = NativeRcl.rcl_get_zero_initialized_subscription();
            var options = NativeRcl.rcl_subscription_get_default_options();
            var topicSize = NativeString.GetUtf8BufferSize(topicName);
            Span<byte> topicBuffer = stackalloc byte[topicSize];
            NativeString.FillUtf8Buffer(topicName, topicBuffer);
            var typeSupport = NativeRcl.std_msgs_msg_string_get_type_support();

            if (typeSupport == IntPtr.Zero)
            {
                throw new RclUnityException("std_msgs/String type support lookup returned null.");
            }

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
                if (!publishers.Contains(publisher))
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
                if (!subscriptions.Contains(subscription))
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

        internal void ReleaseSubscription(RclUnityStringSubscription subscription)
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

                    foreach (var publisher in publishers)
                    {
                        publisher.DisposeFromNode(nodePointer);
                    }

                    publishers.Clear();

                    RclError.ThrowIfNonSuccess(NativeRcl.rcl_node_fini(nodePointer), "rcl_node_fini");
                }

                node = NativeRcl.rcl_get_zero_initialized_node();
                disposed = true;
            }

            GC.SuppressFinalize(this);
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
