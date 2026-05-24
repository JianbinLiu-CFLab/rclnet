// Copyright (c) 2026 Jianbin Liu.
// Licensed under the MIT License.
// See LICENSE in the repository root for license information.
using System;
using System.Text;

namespace Rcl.Unity
{
    /// <summary>
    /// Bridges one managed Unity adapter message type to its native ROS 2 C message layout.
    /// </summary>
    /// <typeparam name="T">The managed Unity adapter message type.</typeparam>
    internal unsafe abstract class NativeMessageBridge<T>
    {
        /// <summary>
        /// Initializes a bridge with the native type support handle for a ROS 2 message.
        /// </summary>
        protected NativeMessageBridge(IntPtr typeSupport, string messageName)
        {
            if (typeSupport == IntPtr.Zero)
            {
                throw new RclUnityException(messageName + " type support lookup returned null.");
            }

            TypeSupport = typeSupport;
            MessageName = messageName;
        }

        /// <summary>
        /// Gets the native rosidl type support handle used by rcl publisher/subscription creation.
        /// </summary>
        public IntPtr TypeSupport { get; }

        /// <summary>
        /// Gets the ROS 2 message name used in diagnostics.
        /// </summary>
        protected string MessageName { get; }

        /// <summary>
        /// Allocates a native ROS 2 message instance.
        /// </summary>
        public abstract void* Create();

        /// <summary>
        /// Destroys a native ROS 2 message instance.
        /// </summary>
        public abstract void Destroy(void* message);

        /// <summary>
        /// Writes a managed value into a native ROS 2 message instance.
        /// </summary>
        public abstract void Write(void* message, T value);

        /// <summary>
        /// Reads a managed value from a native ROS 2 message instance.
        /// </summary>
        public abstract T Read(void* message);

        /// <summary>
        /// Throws when a native message pointer is null.
        /// </summary>
        protected static void ThrowIfNull(void* message, string messageName)
        {
            if (message == null)
            {
                throw new ArgumentNullException(messageName);
            }
        }

        /// <summary>
        /// Assigns a managed string to a rosidl_runtime_c string field.
        /// </summary>
        protected static void AssignString(NativeTypes.rosidl_runtime_c__String* rosString, string value)
        {
            if (rosString == null)
            {
                throw new ArgumentNullException(nameof(rosString));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var byteCount = Encoding.UTF8.GetByteCount(value);
            Span<byte> buffer = stackalloc byte[Math.Max(byteCount, 1)];
            Encoding.UTF8.GetBytes(value, buffer);

            fixed (byte* valuePointer = buffer)
            {
                var size = new NativeTypes.size_t
                {
                    Value = new UIntPtr((ulong)byteCount)
                };

                if (!NativeRcl.rosidl_runtime_c_string_assignn(rosString, valuePointer, size))
                {
                    throw new RclUnityException("rosidl_runtime_c__String__assignn failed.");
                }
            }
        }

        /// <summary>
        /// Reads a managed string from a rosidl_runtime_c string field.
        /// </summary>
        protected static string ReadString(NativeTypes.rosidl_runtime_c__String* rosString)
        {
            if (rosString == null)
            {
                throw new ArgumentNullException(nameof(rosString));
            }

            var length = checked((int)rosString->size.Value.ToUInt64());
            if (length == 0)
            {
                return string.Empty;
            }

            if (rosString->data == IntPtr.Zero)
            {
                throw new RclUnityException("Received ROS string has null data with non-zero size.");
            }

            return Encoding.UTF8.GetString((byte*)rosString->data, length);
        }
    }
}
