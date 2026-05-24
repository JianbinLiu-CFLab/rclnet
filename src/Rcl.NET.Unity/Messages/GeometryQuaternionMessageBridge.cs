namespace Rcl.Unity
{
    internal sealed unsafe class GeometryQuaternionMessageBridge : NativeMessageBridge<RclUnityQuaternion>
    {
        public static readonly GeometryQuaternionMessageBridge Instance = new GeometryQuaternionMessageBridge();

        private GeometryQuaternionMessageBridge()
            : base(NativeRcl.geometry_msgs_msg_quaternion_get_type_support(), "geometry_msgs/Quaternion")
        {
        }

        public override void* Create()
        {
            var message = NativeRcl.geometry_msgs_msg_quaternion_create();
            if (message == null)
            {
                throw new RclUnityException("geometry_msgs__msg__Quaternion__create returned null.");
            }

            return message;
        }

        public override void Destroy(void* message)
        {
            NativeRcl.geometry_msgs_msg_quaternion_destroy((NativeTypes.geometry_msgs__msg__Quaternion*)message);
        }

        public override void Write(void* message, RclUnityQuaternion value)
        {
            ThrowIfNull(message, nameof(message));
            var native = (NativeTypes.geometry_msgs__msg__Quaternion*)message;
            native->x = value.X;
            native->y = value.Y;
            native->z = value.Z;
            native->w = value.W;
        }

        public override RclUnityQuaternion Read(void* message)
        {
            ThrowIfNull(message, nameof(message));
            var native = (NativeTypes.geometry_msgs__msg__Quaternion*)message;
            return new RclUnityQuaternion(native->x, native->y, native->z, native->w);
        }
    }
}
