namespace Rcl.Unity
{
    internal sealed unsafe class GeometryVector3MessageBridge : NativeMessageBridge<RclUnityVector3>
    {
        public static readonly GeometryVector3MessageBridge Instance = new GeometryVector3MessageBridge();

        private GeometryVector3MessageBridge()
            : base(NativeRcl.geometry_msgs_msg_vector3_get_type_support(), "geometry_msgs/Vector3")
        {
        }

        public override void* Create()
        {
            var message = NativeRcl.geometry_msgs_msg_vector3_create();
            if (message == null)
            {
                throw new RclUnityException("geometry_msgs__msg__Vector3__create returned null.");
            }

            return message;
        }

        public override void Destroy(void* message)
        {
            NativeRcl.geometry_msgs_msg_vector3_destroy((NativeTypes.geometry_msgs__msg__Vector3*)message);
        }

        public override void Write(void* message, RclUnityVector3 value)
        {
            ThrowIfNull(message, nameof(message));
            var native = (NativeTypes.geometry_msgs__msg__Vector3*)message;
            native->x = value.X;
            native->y = value.Y;
            native->z = value.Z;
        }

        public override RclUnityVector3 Read(void* message)
        {
            ThrowIfNull(message, nameof(message));
            var native = (NativeTypes.geometry_msgs__msg__Vector3*)message;
            return new RclUnityVector3(native->x, native->y, native->z);
        }
    }
}
