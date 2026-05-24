using System;
using System.Text;

namespace Rcl.Unity
{
    internal unsafe abstract class NativeMessageBridge<T>
    {
        protected NativeMessageBridge(IntPtr typeSupport, string messageName)
        {
            if (typeSupport == IntPtr.Zero)
            {
                throw new RclUnityException(messageName + " type support lookup returned null.");
            }

            TypeSupport = typeSupport;
            MessageName = messageName;
        }

        public IntPtr TypeSupport { get; }

        protected string MessageName { get; }

        public abstract void* Create();

        public abstract void Destroy(void* message);

        public abstract void Write(void* message, T value);

        public abstract T Read(void* message);

        protected static void ThrowIfNull(void* message, string messageName)
        {
            if (message == null)
            {
                throw new ArgumentNullException(messageName);
            }
        }

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
