using System;

namespace Rcl.Unity
{
    public sealed unsafe class RclUnityNode : IDisposable
    {
        private readonly object mutex = new object();
        private readonly RclUnityContext context;
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
                    RclError.ThrowIfNonSuccess(NativeRcl.rcl_node_fini(nodePointer), "rcl_node_fini");
                }

                node = NativeRcl.rcl_get_zero_initialized_node();
                disposed = true;
            }

            GC.SuppressFinalize(this);
        }
    }
}
