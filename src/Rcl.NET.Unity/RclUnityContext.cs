// Copyright (c) 2026 Jianbin Liu.
// Licensed under the MIT License.
// See LICENSE in the repository root for license information.
using System;
using System.Collections.Generic;

namespace Rcl.Unity
{
    /// <summary>
    /// Owns the native rcl context used by the Unity adapter and creates Unity-scoped ROS 2 nodes.
    /// </summary>
    public sealed unsafe class RclUnityContext : IDisposable
    {
        private readonly object mutex = new object();
        private readonly HashSet<RclUnityNode> nodes = new HashSet<RclUnityNode>();
        private NativeTypes.rcl_context_t context;
        private bool disposed;

        /// <summary>
        /// Initializes rcl for the current Unity adapter process.
        /// </summary>
        /// <param name="args">Optional ROS 2 command-line arguments to pass to rcl_init.</param>
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

        /// <summary>
        /// Gets whether this context has already been disposed.
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
        /// Creates a ROS 2 node owned by this context.
        /// </summary>
        /// <param name="name">The ROS 2 node name.</param>
        /// <param name="namespaceName">The ROS 2 namespace for the node.</param>
        /// <returns>A node that should be disposed directly or through the context.</returns>
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

        /// <summary>
        /// Shuts down all owned nodes and finalizes the native rcl context.
        /// </summary>
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

        /// <summary>
        /// Initializes a native node handle using this context.
        /// </summary>
        /// <param name="name">The node name encoded for rcl.</param>
        /// <param name="namespaceName">The node namespace encoded for rcl.</param>
        /// <returns>An initialized native node handle.</returns>
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

        /// <summary>
        /// Removes and disposes a node that was created by this context.
        /// </summary>
        /// <param name="node">The node to release.</param>
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

        /// <summary>
        /// Calls rcl_init with optional command-line arguments.
        /// </summary>
        /// <param name="args">Arguments to pass to rcl_init.</param>
        /// <param name="initOptionsPointer">Initialized rcl init options.</param>
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

        /// <summary>
        /// Throws when the context has already been disposed.
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(RclUnityContext));
            }
        }

        /// <summary>
        /// Finalizes a partially initialized context during constructor failure without masking the original error.
        /// </summary>
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
