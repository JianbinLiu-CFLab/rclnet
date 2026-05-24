// Copyright (c) 2026 Jianbin Liu.
// Licensed under the MIT License.
// See LICENSE in the repository root for license information.
using System;
using System.Runtime.InteropServices;

namespace Rcl.Unity
{
    /// <summary>
    /// Contains native struct and enum layouts used by the Unity adapter's P/Invoke surface.
    /// </summary>
    internal static unsafe class NativeTypes
    {
        /// <summary>
        /// Represents the native C size_t value used by ROS 2 C APIs.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct size_t
        {
            /// <summary>
            /// Stores the platform-sized unsigned value.
            /// </summary>
            public UIntPtr Value;
        }

        /// <summary>
        /// Mirrors rcutils_allocator_t.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct rcutils_allocator_t
        {
            public IntPtr allocate;
            public IntPtr deallocate;
            public IntPtr reallocate;
            public IntPtr zero_allocate;
            public IntPtr state;
        }

        /// <summary>
        /// Mirrors rcl_allocator_t.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct rcl_allocator_t
        {
            public rcutils_allocator_t Value;
        }

        /// <summary>
        /// Mirrors rcl_arguments_t for context initialization.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct rcl_arguments_t
        {
            public IntPtr impl;
        }

        /// <summary>
        /// Mirrors rcl_init_options_t.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct rcl_init_options_t
        {
            public IntPtr impl;
        }

        /// <summary>
        /// Mirrors rcl_context_t for the native ROS 2 context.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct rcl_context_t
        {
            public rcl_arguments_t global_arguments;
            public IntPtr impl;
            public ulong instance_id_storage;
        }

        /// <summary>
        /// Mirrors rcl_node_t for native node ownership.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct rcl_node_t
        {
            public IntPtr context;
            public IntPtr impl;
        }

        /// <summary>
        /// Mirrors rcl_publisher_t for native publisher ownership.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct rcl_publisher_t
        {
            public IntPtr impl;
        }

        /// <summary>
        /// Mirrors rcl_subscription_t for native subscription ownership.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct rcl_subscription_t
        {
            public IntPtr impl;
        }

        /// <summary>
        /// Mirrors rmw_time_t for QoS duration fields.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct rmw_time_t
        {
            public ulong sec;
            public ulong nsec;
        }

        /// <summary>
        /// Mirrors rmw_qos_history_policy_t.
        /// </summary>
        internal enum rmw_qos_history_policy_t : int
        {
            RMW_QOS_POLICY_HISTORY_SYSTEM_DEFAULT,
            RMW_QOS_POLICY_HISTORY_KEEP_LAST,
            RMW_QOS_POLICY_HISTORY_KEEP_ALL,
            RMW_QOS_POLICY_HISTORY_UNKNOWN
        }

        /// <summary>
        /// Mirrors rmw_qos_reliability_policy_t.
        /// </summary>
        internal enum rmw_qos_reliability_policy_t : int
        {
            RMW_QOS_POLICY_RELIABILITY_SYSTEM_DEFAULT,
            RMW_QOS_POLICY_RELIABILITY_RELIABLE,
            RMW_QOS_POLICY_RELIABILITY_BEST_EFFORT,
            RMW_QOS_POLICY_RELIABILITY_UNKNOWN
        }

        /// <summary>
        /// Mirrors rmw_qos_durability_policy_t.
        /// </summary>
        internal enum rmw_qos_durability_policy_t : int
        {
            RMW_QOS_POLICY_DURABILITY_SYSTEM_DEFAULT,
            RMW_QOS_POLICY_DURABILITY_TRANSIENT_LOCAL,
            RMW_QOS_POLICY_DURABILITY_VOLATILE,
            RMW_QOS_POLICY_DURABILITY_UNKNOWN
        }

        /// <summary>
        /// Mirrors rmw_qos_liveliness_policy_t.
        /// </summary>
        internal enum rmw_qos_liveliness_policy_t : int
        {
            RMW_QOS_POLICY_LIVELINESS_SYSTEM_DEFAULT = 0,
            RMW_QOS_POLICY_LIVELINESS_AUTOMATIC = 1,
            RMW_QOS_POLICY_LIVELINESS_MANUAL_BY_NODE = 2,
            RMW_QOS_POLICY_LIVELINESS_MANUAL_BY_TOPIC = 3,
            RMW_QOS_POLICY_LIVELINESS_UNKNOWN = 4
        }

        /// <summary>
        /// Mirrors rmw_unique_network_flow_endpoints_requirement_t.
        /// </summary>
        internal enum rmw_unique_network_flow_endpoints_requirement_t : int
        {
            RMW_UNIQUE_NETWORK_FLOW_ENDPOINTS_NOT_REQUIRED = 0,
            RMW_UNIQUE_NETWORK_FLOW_ENDPOINTS_STRICTLY_REQUIRED = 1,
            RMW_UNIQUE_NETWORK_FLOW_ENDPOINTS_OPTIONALLY_REQUIRED = 2,
            RMW_UNIQUE_NETWORK_FLOW_ENDPOINTS_SYSTEM_DEFAULT = 3
        }

        /// <summary>
        /// Mirrors rmw_qos_profile_t for publisher and subscription creation.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct rmw_qos_profile_t
        {
            public rmw_qos_history_policy_t history;
            public size_t depth;
            public rmw_qos_reliability_policy_t reliability;
            public rmw_qos_durability_policy_t durability;
            public rmw_time_t deadline;
            public rmw_time_t lifespan;
            public rmw_qos_liveliness_policy_t liveliness;
            public rmw_time_t liveliness_lease_duration;
            [MarshalAs(UnmanagedType.I1)]
            public bool avoid_ros_namespace_conventions;
        }

        /// <summary>
        /// Mirrors rcl_node_options_t.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct rcl_node_options_t
        {
            public rcl_allocator_t allocator;
            [MarshalAs(UnmanagedType.I1)]
            public bool use_global_arguments;
            public rcl_arguments_t arguments;
            [MarshalAs(UnmanagedType.I1)]
            public bool enable_rosout;
            public rmw_qos_profile_t rosout_qos;
        }

        /// <summary>
        /// Mirrors rmw_publisher_options_t.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct rmw_publisher_options_t
        {
            public IntPtr rmw_specific_publisher_payload;
            public rmw_unique_network_flow_endpoints_requirement_t require_unique_network_flow_endpoints;
        }

        /// <summary>
        /// Mirrors rcl_publisher_options_t.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct rcl_publisher_options_t
        {
            public rmw_qos_profile_t qos;
            public rcl_allocator_t allocator;
            public rmw_publisher_options_t rmw_publisher_options;
            [MarshalAs(UnmanagedType.I1)]
            public bool disable_loaned_message;
        }

        /// <summary>
        /// Mirrors rmw_subscription_options_t.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct rmw_subscription_options_t
        {
            public IntPtr rmw_specific_subscription_payload;
            [MarshalAs(UnmanagedType.I1)]
            public bool ignore_local_publications;
            public rmw_unique_network_flow_endpoints_requirement_t require_unique_network_flow_endpoints;
            public IntPtr content_filter_options;
        }

        /// <summary>
        /// Mirrors rcl_subscription_options_t.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct rcl_subscription_options_t
        {
            public rmw_qos_profile_t qos;
            public rcl_allocator_t allocator;
            public rmw_subscription_options_t rmw_subscription_options;
            [MarshalAs(UnmanagedType.I1)]
            public bool disable_loaned_message;
        }

        /// <summary>
        /// Mirrors rosidl_runtime_c__String.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct rosidl_runtime_c__String
        {
            public IntPtr data;
            public size_t size;
            public size_t capacity;
        }

        /// <summary>
        /// Mirrors std_msgs/msg/String.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct std_msgs__msg__String
        {
            public rosidl_runtime_c__String data;
        }

        /// <summary>
        /// Mirrors builtin_interfaces/msg/Time.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct builtin_interfaces__msg__Time
        {
            public int sec;
            public uint nanosec;
        }

        /// <summary>
        /// Mirrors std_msgs/msg/Header.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct std_msgs__msg__Header
        {
            public builtin_interfaces__msg__Time stamp;
            public rosidl_runtime_c__String frame_id;
        }

        /// <summary>
        /// Mirrors geometry_msgs/msg/Vector3.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct geometry_msgs__msg__Vector3
        {
            public double x;
            public double y;
            public double z;
        }

        /// <summary>
        /// Mirrors geometry_msgs/msg/Quaternion.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct geometry_msgs__msg__Quaternion
        {
            public double x;
            public double y;
            public double z;
            public double w;
        }

        /// <summary>
        /// Mirrors geometry_msgs/msg/Point.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct geometry_msgs__msg__Point
        {
            public double x;
            public double y;
            public double z;
        }

        /// <summary>
        /// Mirrors geometry_msgs/msg/Pose.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct geometry_msgs__msg__Pose
        {
            public geometry_msgs__msg__Point position;
            public geometry_msgs__msg__Quaternion orientation;
        }

        /// <summary>
        /// Mirrors rcutils_error_string_t.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct rcutils_error_string_t
        {
            public fixed sbyte str[1024];
        }
    }
}
