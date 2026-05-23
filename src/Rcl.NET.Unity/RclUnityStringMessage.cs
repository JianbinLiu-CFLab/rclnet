using System;
using System.Text;

namespace Rcl.Unity
{
    internal static unsafe class RclUnityStringMessage
    {
        public static NativeTypes.std_msgs__msg__String* Create()
        {
            var message = NativeRcl.std_msgs_msg_string_create();
            if (message == null)
            {
                throw new RclUnityException("std_msgs__msg__String__create returned null.");
            }

            return message;
        }

        public static void Destroy(NativeTypes.std_msgs__msg__String* message)
        {
            if (message != null)
            {
                NativeRcl.std_msgs_msg_string_destroy(message);
            }
        }

        public static void Assign(NativeTypes.std_msgs__msg__String* message, string value)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
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
                    Value = new UIntPtr((uint)byteCount)
                };

                if (!NativeRcl.rosidl_runtime_c_string_assignn(&message->data, valuePointer, size))
                {
                    throw new RclUnityException("rosidl_runtime_c__String__assignn failed.");
                }
            }
        }

        public static string Read(NativeTypes.std_msgs__msg__String* message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var length = checked((int)message->data.size.Value.ToUInt64());
            if (length == 0)
            {
                return string.Empty;
            }

            if (message->data.data == IntPtr.Zero)
            {
                throw new RclUnityException("Received std_msgs/String has null data with non-zero size.");
            }

            return Encoding.UTF8.GetString((byte*)message->data.data, length);
        }
    }
}
