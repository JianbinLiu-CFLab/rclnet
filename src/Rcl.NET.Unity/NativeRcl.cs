using System;
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

        [DllImport("rcl", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeTypes.rcl_publisher_t rcl_get_zero_initialized_publisher();

        [DllImport("rcl", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeTypes.rcl_publisher_options_t rcl_publisher_get_default_options();

        [DllImport("rcl", CallingConvention = CallingConvention.Cdecl)]
        public static extern RclReturnCode rcl_publisher_init(
            NativeTypes.rcl_publisher_t* publisher,
            NativeTypes.rcl_node_t* node,
            IntPtr typeSupport,
            byte* topicName,
            NativeTypes.rcl_publisher_options_t* options);

        [DllImport("rcl", CallingConvention = CallingConvention.Cdecl)]
        public static extern RclReturnCode rcl_publish(
            NativeTypes.rcl_publisher_t* publisher,
            void* rosMessage,
            void* allocation);

        [DllImport("rcl", CallingConvention = CallingConvention.Cdecl)]
        public static extern RclReturnCode rcl_publisher_fini(
            NativeTypes.rcl_publisher_t* publisher,
            NativeTypes.rcl_node_t* node);

        [DllImport("rcl", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeTypes.rcl_subscription_t rcl_get_zero_initialized_subscription();

        [DllImport("rcl", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeTypes.rcl_subscription_options_t rcl_subscription_get_default_options();

        [DllImport("rcl", CallingConvention = CallingConvention.Cdecl)]
        public static extern RclReturnCode rcl_subscription_options_fini(
            NativeTypes.rcl_subscription_options_t* options);

        [DllImport("rcl", CallingConvention = CallingConvention.Cdecl)]
        public static extern RclReturnCode rcl_subscription_init(
            NativeTypes.rcl_subscription_t* subscription,
            NativeTypes.rcl_node_t* node,
            IntPtr typeSupport,
            byte* topicName,
            NativeTypes.rcl_subscription_options_t* options);

        [DllImport("rcl", CallingConvention = CallingConvention.Cdecl)]
        public static extern RclReturnCode rcl_take(
            NativeTypes.rcl_subscription_t* subscription,
            void* rosMessage,
            void* messageInfo,
            void* allocation);

        [DllImport("rcl", CallingConvention = CallingConvention.Cdecl)]
        public static extern RclReturnCode rcl_subscription_fini(
            NativeTypes.rcl_subscription_t* subscription,
            NativeTypes.rcl_node_t* node);

        [DllImport("std_msgs__rosidl_typesupport_c", EntryPoint = "rosidl_typesupport_c__get_message_type_support_handle__std_msgs__msg__String", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr std_msgs_msg_string_get_type_support();

        [DllImport("std_msgs__rosidl_generator_c", EntryPoint = "std_msgs__msg__String__create", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeTypes.std_msgs__msg__String* std_msgs_msg_string_create();

        [DllImport("std_msgs__rosidl_generator_c", EntryPoint = "std_msgs__msg__String__destroy", CallingConvention = CallingConvention.Cdecl)]
        public static extern void std_msgs_msg_string_destroy(NativeTypes.std_msgs__msg__String* message);

        [DllImport("rosidl_runtime_c", EntryPoint = "rosidl_runtime_c__String__assignn", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool rosidl_runtime_c_string_assignn(
            NativeTypes.rosidl_runtime_c__String* rosString,
            byte* value,
            NativeTypes.size_t size);
    }
}
