// Copyright (c) 2026 Jianbin Liu.
// Licensed under the MIT License.
// See LICENSE in the repository root for license information.
using System;
using System.Runtime.InteropServices;

namespace Rcl.Unity
{
    /// <summary>
    /// Declares the minimal native rcl, rcutils, and rosidl entry points required by the Unity adapter.
    /// </summary>
    internal static unsafe class NativeRcl
    {
        // rcutils allocator and error-state APIs.
        [DllImport("rcutils", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeTypes.rcutils_allocator_t rcutils_get_default_allocator();

        [DllImport("rcutils", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool rcutils_error_is_set();

        [DllImport("rcutils", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeTypes.rcutils_error_string_t rcutils_get_error_string();

        [DllImport("rcutils", CallingConvention = CallingConvention.Cdecl)]
        public static extern void rcutils_reset_error();

        // rcl context and node lifecycle APIs.
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

        // rcl publisher APIs used by the lightweight synchronous publish path.
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

        // rcl subscription APIs used by the lightweight polling take path.
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

        // std_msgs/String type support and C message allocation APIs.
        [DllImport("std_msgs__rosidl_typesupport_c", EntryPoint = "rosidl_typesupport_c__get_message_type_support_handle__std_msgs__msg__String", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr std_msgs_msg_string_get_type_support();

        [DllImport("std_msgs__rosidl_generator_c", EntryPoint = "std_msgs__msg__String__create", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeTypes.std_msgs__msg__String* std_msgs_msg_string_create();

        [DllImport("std_msgs__rosidl_generator_c", EntryPoint = "std_msgs__msg__String__destroy", CallingConvention = CallingConvention.Cdecl)]
        public static extern void std_msgs_msg_string_destroy(NativeTypes.std_msgs__msg__String* message);

        // builtin_interfaces/Time type support and C message allocation APIs.
        [DllImport("builtin_interfaces__rosidl_typesupport_c", EntryPoint = "rosidl_typesupport_c__get_message_type_support_handle__builtin_interfaces__msg__Time", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr builtin_interfaces_msg_time_get_type_support();

        [DllImport("builtin_interfaces__rosidl_generator_c", EntryPoint = "builtin_interfaces__msg__Time__create", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeTypes.builtin_interfaces__msg__Time* builtin_interfaces_msg_time_create();

        [DllImport("builtin_interfaces__rosidl_generator_c", EntryPoint = "builtin_interfaces__msg__Time__destroy", CallingConvention = CallingConvention.Cdecl)]
        public static extern void builtin_interfaces_msg_time_destroy(NativeTypes.builtin_interfaces__msg__Time* message);

        // std_msgs/Header type support and C message allocation APIs.
        [DllImport("std_msgs__rosidl_typesupport_c", EntryPoint = "rosidl_typesupport_c__get_message_type_support_handle__std_msgs__msg__Header", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr std_msgs_msg_header_get_type_support();

        [DllImport("std_msgs__rosidl_generator_c", EntryPoint = "std_msgs__msg__Header__create", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeTypes.std_msgs__msg__Header* std_msgs_msg_header_create();

        [DllImport("std_msgs__rosidl_generator_c", EntryPoint = "std_msgs__msg__Header__destroy", CallingConvention = CallingConvention.Cdecl)]
        public static extern void std_msgs_msg_header_destroy(NativeTypes.std_msgs__msg__Header* message);

        // geometry_msgs/Vector3 type support and C message allocation APIs.
        [DllImport("geometry_msgs__rosidl_typesupport_c", EntryPoint = "rosidl_typesupport_c__get_message_type_support_handle__geometry_msgs__msg__Vector3", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr geometry_msgs_msg_vector3_get_type_support();

        [DllImport("geometry_msgs__rosidl_generator_c", EntryPoint = "geometry_msgs__msg__Vector3__create", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeTypes.geometry_msgs__msg__Vector3* geometry_msgs_msg_vector3_create();

        [DllImport("geometry_msgs__rosidl_generator_c", EntryPoint = "geometry_msgs__msg__Vector3__destroy", CallingConvention = CallingConvention.Cdecl)]
        public static extern void geometry_msgs_msg_vector3_destroy(NativeTypes.geometry_msgs__msg__Vector3* message);

        // geometry_msgs/Quaternion type support and C message allocation APIs.
        [DllImport("geometry_msgs__rosidl_typesupport_c", EntryPoint = "rosidl_typesupport_c__get_message_type_support_handle__geometry_msgs__msg__Quaternion", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr geometry_msgs_msg_quaternion_get_type_support();

        [DllImport("geometry_msgs__rosidl_generator_c", EntryPoint = "geometry_msgs__msg__Quaternion__create", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeTypes.geometry_msgs__msg__Quaternion* geometry_msgs_msg_quaternion_create();

        [DllImport("geometry_msgs__rosidl_generator_c", EntryPoint = "geometry_msgs__msg__Quaternion__destroy", CallingConvention = CallingConvention.Cdecl)]
        public static extern void geometry_msgs_msg_quaternion_destroy(NativeTypes.geometry_msgs__msg__Quaternion* message);

        // geometry_msgs/Point type support and C message allocation APIs.
        [DllImport("geometry_msgs__rosidl_typesupport_c", EntryPoint = "rosidl_typesupport_c__get_message_type_support_handle__geometry_msgs__msg__Point", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr geometry_msgs_msg_point_get_type_support();

        [DllImport("geometry_msgs__rosidl_generator_c", EntryPoint = "geometry_msgs__msg__Point__create", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeTypes.geometry_msgs__msg__Point* geometry_msgs_msg_point_create();

        [DllImport("geometry_msgs__rosidl_generator_c", EntryPoint = "geometry_msgs__msg__Point__destroy", CallingConvention = CallingConvention.Cdecl)]
        public static extern void geometry_msgs_msg_point_destroy(NativeTypes.geometry_msgs__msg__Point* message);

        // geometry_msgs/Pose type support and C message allocation APIs.
        [DllImport("geometry_msgs__rosidl_typesupport_c", EntryPoint = "rosidl_typesupport_c__get_message_type_support_handle__geometry_msgs__msg__Pose", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr geometry_msgs_msg_pose_get_type_support();

        [DllImport("geometry_msgs__rosidl_generator_c", EntryPoint = "geometry_msgs__msg__Pose__create", CallingConvention = CallingConvention.Cdecl)]
        public static extern NativeTypes.geometry_msgs__msg__Pose* geometry_msgs_msg_pose_create();

        [DllImport("geometry_msgs__rosidl_generator_c", EntryPoint = "geometry_msgs__msg__Pose__destroy", CallingConvention = CallingConvention.Cdecl)]
        public static extern void geometry_msgs_msg_pose_destroy(NativeTypes.geometry_msgs__msg__Pose* message);

        // rosidl_runtime_c string helper used by managed-to-native string assignment.
        [DllImport("rosidl_runtime_c", EntryPoint = "rosidl_runtime_c__String__assignn", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool rosidl_runtime_c_string_assignn(
            NativeTypes.rosidl_runtime_c__String* rosString,
            byte* value,
            NativeTypes.size_t size);
    }
}
