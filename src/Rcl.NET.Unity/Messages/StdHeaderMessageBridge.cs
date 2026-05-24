namespace Rcl.Unity
{
    internal sealed unsafe class StdHeaderMessageBridge : NativeMessageBridge<RclUnityHeader>
    {
        public static readonly StdHeaderMessageBridge Instance = new StdHeaderMessageBridge();

        private StdHeaderMessageBridge()
            : base(NativeRcl.std_msgs_msg_header_get_type_support(), "std_msgs/Header")
        {
        }

        public override void* Create()
        {
            var message = NativeRcl.std_msgs_msg_header_create();
            if (message == null)
            {
                throw new RclUnityException("std_msgs__msg__Header__create returned null.");
            }

            return message;
        }

        public override void Destroy(void* message)
        {
            NativeRcl.std_msgs_msg_header_destroy((NativeTypes.std_msgs__msg__Header*)message);
        }

        public override void Write(void* message, RclUnityHeader value)
        {
            ThrowIfNull(message, nameof(message));
            var native = (NativeTypes.std_msgs__msg__Header*)message;
            native->stamp.sec = value.Stamp.Sec;
            native->stamp.nanosec = value.Stamp.Nanosec;
            AssignString(&native->frame_id, value.FrameId);
        }

        public override RclUnityHeader Read(void* message)
        {
            ThrowIfNull(message, nameof(message));
            var native = (NativeTypes.std_msgs__msg__Header*)message;
            return new RclUnityHeader(
                new RclUnityTime(native->stamp.sec, native->stamp.nanosec),
                ReadString(&native->frame_id));
        }
    }
}
