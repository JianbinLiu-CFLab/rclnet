using System;
using System.Runtime.InteropServices;

namespace Rcl.Unity
{
    internal static unsafe class NativeTypes
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct size_t
        {
            public UIntPtr Value;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct rcutils_allocator_t
        {
            public IntPtr allocate;
            public IntPtr deallocate;
            public IntPtr reallocate;
            public IntPtr zero_allocate;
            public IntPtr state;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct rcl_allocator_t
        {
            public rcutils_allocator_t Value;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct rcl_arguments_t
        {
            public IntPtr impl;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct rcl_init_options_t
        {
            public IntPtr impl;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct rcl_context_t
        {
            public rcl_arguments_t global_arguments;
            public IntPtr impl;
            public ulong instance_id_storage;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct rcl_node_t
        {
            public IntPtr context;
            public IntPtr impl;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct rmw_time_t
        {
            public ulong sec;
            public ulong nsec;
        }

        internal enum rmw_qos_history_policy_t : int
        {
            RMW_QOS_POLICY_HISTORY_SYSTEM_DEFAULT,
            RMW_QOS_POLICY_HISTORY_KEEP_LAST,
            RMW_QOS_POLICY_HISTORY_KEEP_ALL,
            RMW_QOS_POLICY_HISTORY_UNKNOWN
        }

        internal enum rmw_qos_reliability_policy_t : int
        {
            RMW_QOS_POLICY_RELIABILITY_SYSTEM_DEFAULT,
            RMW_QOS_POLICY_RELIABILITY_RELIABLE,
            RMW_QOS_POLICY_RELIABILITY_BEST_EFFORT,
            RMW_QOS_POLICY_RELIABILITY_UNKNOWN
        }

        internal enum rmw_qos_durability_policy_t : int
        {
            RMW_QOS_POLICY_DURABILITY_SYSTEM_DEFAULT,
            RMW_QOS_POLICY_DURABILITY_TRANSIENT_LOCAL,
            RMW_QOS_POLICY_DURABILITY_VOLATILE,
            RMW_QOS_POLICY_DURABILITY_UNKNOWN
        }

        internal enum rmw_qos_liveliness_policy_t : int
        {
            RMW_QOS_POLICY_LIVELINESS_SYSTEM_DEFAULT = 0,
            RMW_QOS_POLICY_LIVELINESS_AUTOMATIC = 1,
            RMW_QOS_POLICY_LIVELINESS_MANUAL_BY_NODE = 2,
            RMW_QOS_POLICY_LIVELINESS_MANUAL_BY_TOPIC = 3,
            RMW_QOS_POLICY_LIVELINESS_UNKNOWN = 4
        }

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

        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct rcutils_error_string_t
        {
            public fixed sbyte str[1024];
        }
    }
}
