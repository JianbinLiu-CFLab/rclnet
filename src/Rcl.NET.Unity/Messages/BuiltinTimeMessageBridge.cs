namespace Rcl.Unity
{
    internal sealed unsafe class BuiltinTimeMessageBridge : NativeMessageBridge<RclUnityTime>
    {
        public static readonly BuiltinTimeMessageBridge Instance = new BuiltinTimeMessageBridge();

        private BuiltinTimeMessageBridge()
            : base(NativeRcl.builtin_interfaces_msg_time_get_type_support(), "builtin_interfaces/Time")
        {
        }

        public override void* Create()
        {
            var message = NativeRcl.builtin_interfaces_msg_time_create();
            if (message == null)
            {
                throw new RclUnityException("builtin_interfaces__msg__Time__create returned null.");
            }

            return message;
        }

        public override void Destroy(void* message)
        {
            NativeRcl.builtin_interfaces_msg_time_destroy((NativeTypes.builtin_interfaces__msg__Time*)message);
        }

        public override void Write(void* message, RclUnityTime value)
        {
            ThrowIfNull(message, nameof(message));
            var native = (NativeTypes.builtin_interfaces__msg__Time*)message;
            native->sec = value.Sec;
            native->nanosec = value.Nanosec;
        }

        public override RclUnityTime Read(void* message)
        {
            ThrowIfNull(message, nameof(message));
            var native = (NativeTypes.builtin_interfaces__msg__Time*)message;
            return new RclUnityTime(native->sec, native->nanosec);
        }
    }
}
