// Copyright (c) 2026 Jianbin Liu.
// Licensed under the MIT License.
// See LICENSE in the repository root for license information.
namespace Rcl.Unity
{
    /// <summary>
    /// Bridges geometry_msgs/Quaternion between managed <see cref="RclUnityQuaternion"/> and the native C message.
    /// </summary>
    internal sealed unsafe class GeometryQuaternionMessageBridge : NativeMessageBridge<RclUnityQuaternion>
    {
        /// <summary>
        /// Gets the singleton bridge instance for geometry_msgs/Quaternion.
        /// </summary>
        public static readonly GeometryQuaternionMessageBridge Instance = new GeometryQuaternionMessageBridge();

        private GeometryQuaternionMessageBridge()
            : base(NativeRcl.geometry_msgs_msg_quaternion_get_type_support(), "geometry_msgs/Quaternion")
        {
        }

        /// <inheritdoc/>
        public override void* Create()
        {
            var message = NativeRcl.geometry_msgs_msg_quaternion_create();
            if (message == null)
            {
                throw new RclUnityException("geometry_msgs__msg__Quaternion__create returned null.");
            }

            return message;
        }

        /// <inheritdoc/>
        public override void Destroy(void* message)
        {
            NativeRcl.geometry_msgs_msg_quaternion_destroy((NativeTypes.geometry_msgs__msg__Quaternion*)message);
        }

        /// <inheritdoc/>
        public override void Write(void* message, RclUnityQuaternion value)
        {
            ThrowIfNull(message, nameof(message));
            var native = (NativeTypes.geometry_msgs__msg__Quaternion*)message;
            native->x = value.X;
            native->y = value.Y;
            native->z = value.Z;
            native->w = value.W;
        }

        /// <inheritdoc/>
        public override RclUnityQuaternion Read(void* message)
        {
            ThrowIfNull(message, nameof(message));
            var native = (NativeTypes.geometry_msgs__msg__Quaternion*)message;
            return new RclUnityQuaternion(native->x, native->y, native->z, native->w);
        }
    }
}
