using System;
using System.Collections.Generic;

namespace Rcl.Unity
{
    public sealed unsafe class RclUnityContext : IDisposable
    {
        private readonly object mutex = new object();
        private readonly HashSet<RclUnityNode> nodes = new HashSet<RclUnityNode>();
        private NativeTypes.rcl_context_t context;
        private bool disposed;

        public RclUnityContext(string[]? args = null)
        {
            NativeLibraryPath.Configure();

            context = NativeRcl.rcl_get_zero_initialized_context();
            var initOptions = NativeRcl.rcl_get_zero_initialized_init_options();
            var initOptionsInitialized = false;

            try
            {
                var allocator = new NativeTypes.rcl_allocator_t
                {
                    Value = NativeRcl.rcutils_get_default_allocator()
                };

                var initOptionsPointer = &initOptions;
                RclError.ThrowIfNonSuccess(
                    NativeRcl.rcl_init_options_init(initOptionsPointer, allocator),
                    "rcl_init_options_init");
                initOptionsInitialized = true;

                InitializeContext(args ?? Array.Empty<string>(), initOptionsPointer);
            }
            catch
            {
                BestEffortContextFini();
                throw;
            }
            finally
            {
                if (initOptionsInitialized)
                {
                    var initOptionsPointer = &initOptions;
                    RclError.ThrowIfNonSuccess(
                        NativeRcl.rcl_init_options_fini(initOptionsPointer),
                        "rcl_init_options_fini");
                }
            }
        }

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

        public RclUnityNode CreateNode(string name, string namespaceName = "/")
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Node name must not be null or empty.", nameof(name));
            }

            if (string.IsNullOrEmpty(namespaceName))
            {
                throw new ArgumentException("Node namespace must not be null or empty.", nameof(namespaceName));
            }

            lock (mutex)
            {
                ThrowIfDisposed();

                var node = new RclUnityNode(this, name, namespaceName);
                nodes.Add(node);
                return node;
            }
        }

        public void Dispose()
        {
            RclUnityNode[] nodesToDispose;

            lock (mutex)
            {
                if (disposed)
                {
                    return;
                }

                disposed = true;
                nodesToDispose = new RclUnityNode[nodes.Count];
                nodes.CopyTo(nodesToDispose);
                nodes.Clear();
            }

            for (var i = 0; i < nodesToDispose.Length; i++)
            {
                nodesToDispose[i].DisposeFromContext();
            }

            fixed (NativeTypes.rcl_context_t* contextPointer = &context)
            {
                RclError.ThrowIfNonSuccess(NativeRcl.rcl_shutdown(contextPointer), "rcl_shutdown");
                RclError.ThrowIfNonSuccess(NativeRcl.rcl_context_fini(contextPointer), "rcl_context_fini");
            }

            GC.SuppressFinalize(this);
        }

        internal NativeTypes.rcl_node_t InitializeNode(string name, string namespaceName)
        {
            var nativeNode = NativeRcl.rcl_get_zero_initialized_node();
            var options = NativeRcl.rcl_node_get_default_options();
            options.enable_rosout = false;

            var nameSize = NativeString.GetUtf8BufferSize(name);
            var namespaceSize = NativeString.GetUtf8BufferSize(namespaceName);
            Span<byte> nameBuffer = stackalloc byte[nameSize];
            Span<byte> namespaceBuffer = stackalloc byte[namespaceSize];
            NativeString.FillUtf8Buffer(name, nameBuffer);
            NativeString.FillUtf8Buffer(namespaceName, namespaceBuffer);

            try
            {
                fixed (byte* namePointer = nameBuffer)
                fixed (byte* namespacePointer = namespaceBuffer)
                fixed (NativeTypes.rcl_context_t* contextPointer = &context)
                {
                    var nodePointer = &nativeNode;
                    var optionsPointer = &options;
                    RclError.ThrowIfNonSuccess(
                        NativeRcl.rcl_node_init(nodePointer, namePointer, namespacePointer, contextPointer, optionsPointer),
                        "rcl_node_init");
                }

                return nativeNode;
            }
            finally
            {
                var optionsPointer = &options;
                RclError.ThrowIfNonSuccess(NativeRcl.rcl_node_options_fini(optionsPointer), "rcl_node_options_fini");
            }
        }

        internal void ReleaseNode(RclUnityNode node)
        {
            lock (mutex)
            {
                if (disposed)
                {
                    return;
                }

                nodes.Remove(node);
                node.DisposeFromContext();
            }
        }

        private void InitializeContext(string[] args, NativeTypes.rcl_init_options_t* initOptionsPointer)
        {
            fixed (NativeTypes.rcl_context_t* contextPointer = &context)
            {
                if (args.Length == 0)
                {
                    RclError.ThrowIfNonSuccess(
                        NativeRcl.rcl_init(0, null, initOptionsPointer, contextPointer),
                        "rcl_init");
                    return;
                }

                var bufferSize = NativeString.GetUtf8BufferSize(args);
                Span<byte> argBuffer = stackalloc byte[bufferSize];
                var argv = stackalloc byte*[args.Length];
                NativeString.FillUtf8Buffer(args, argBuffer, argv);

                RclError.ThrowIfNonSuccess(
                    NativeRcl.rcl_init(args.Length, argv, initOptionsPointer, contextPointer),
                    "rcl_init");
            }
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(RclUnityContext));
            }
        }

        private void BestEffortContextFini()
        {
            try
            {
                fixed (NativeTypes.rcl_context_t* contextPointer = &context)
                {
                    NativeRcl.rcl_context_fini(contextPointer);
                }
            }
            catch
            {
            }
        }
    }
}
