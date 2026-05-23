using System.Runtime.InteropServices;

namespace Rcl.Unity
{
    internal static unsafe class NativeRcl
    {
        [DllImport("rcutils", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeTypes.rcutils_allocator_t rcutils_get_default_allocator();

        [DllImport("rcutils", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool rcutils_error_is_set();

        [DllImport("rcutils", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeTypes.rcutils_error_string_t rcutils_get_error_string();

        [DllImport("rcutils", CallingConvention = CallingConvention.Cdecl)]
        public static extern void rcutils_reset_error();

        [DllImport("rcl", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeTypes.rcl_init_options_t rcl_get_zero_initialized_init_options();

        [DllImport("rcl", CallingConvention = CallingConvention.Cdecl)]
        public static extern RclReturnCode rcl_init_options_init(
            NativeTypes.rcl_init_options_t* initOptions,
            NativeTypes.rcl_allocator_t allocator);

        [DllImport("rcl", CallingConvention = CallingConvention.Cdecl)]
        public static extern RclReturnCode rcl_init_options_fini(NativeTypes.rcl_init_options_t* initOptions);

        [DllImport("rcl", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeTypes.rcl_context_t rcl_get_zero_initialized_context();

        [DllImport("rcl", CallingConvention = CallingConvention.Cdecl)]
        public static extern RclReturnCode rcl_init(
            int argc,
            byte** argv,
            NativeTypes.rcl_init_options_t* options,
            NativeTypes.rcl_context_t* context);

        [DllImport("rcl", CallingConvention = CallingConvention.Cdecl)]
        public static extern RclReturnCode rcl_shutdown(NativeTypes.rcl_context_t* context);

        [DllImport("rcl", CallingConvention = CallingConvention.Cdecl)]
        public static extern RclReturnCode rcl_context_fini(NativeTypes.rcl_context_t* context);

        [DllImport("rcl", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeTypes.rcl_node_t rcl_get_zero_initialized_node();

        [DllImport("rcl", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeTypes.rcl_node_options_t rcl_node_get_default_options();

        [DllImport("rcl", CallingConvention = CallingConvention.Cdecl)]
        public static extern RclReturnCode rcl_node_options_fini(NativeTypes.rcl_node_options_t* options);

        [DllImport("rcl", CallingConvention = CallingConvention.Cdecl)]
        public static extern RclReturnCode rcl_node_init(
            NativeTypes.rcl_node_t* node,
            byte* name,
            byte* namespaceName,
            NativeTypes.rcl_context_t* context,
            NativeTypes.rcl_node_options_t* options);

        [DllImport("rcl", CallingConvention = CallingConvention.Cdecl)]
        public static extern RclReturnCode rcl_node_fini(NativeTypes.rcl_node_t* node);
    }
}
