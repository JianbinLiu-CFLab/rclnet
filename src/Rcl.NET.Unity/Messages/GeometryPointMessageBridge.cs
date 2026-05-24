// Copyright (c) 2026 Jianbin Liu.
// Licensed under the MIT License.
// See LICENSE in the repository root for license information.
namespace Rcl.Unity
{
    /// <summary>
    /// Bridges geometry_msgs/Point between managed <see cref="RclUnityPoint"/> and the native C message.
    /// </summary>
    internal sealed unsafe class GeometryPointMessageBridge : NativeMessageBridge<RclUnityPoint>
    {
        /// <summary>
        /// Gets the singleton bridge instance for geometry_msgs/Point.
        /// </summary>
        public static readonly GeometryPointMessageBridge Instance = new GeometryPointMessageBridge();

        private GeometryPointMessageBridge()
            : base(NativeRcl.geometry_msgs_msg_point_get_type_support(), "geometry_msgs/Point")
        {
        }

        /// <inheritdoc/>
        public override void* Create()
        {
            var message = NativeRcl.geometry_msgs_msg_point_create();
            if (message == null)
            {
                throw new RclUnityException("geometry_msgs__msg__Point__create returned null.");
            }

            return message;
        }

        /// <inheritdoc/>
        public override void Destroy(void* message)
        {
            NativeRcl.geometry_msgs_msg_point_destroy((NativeTypes.geometry_msgs__msg__Point*)message);
        }

        /// <inheritdoc/>
        public override void Write(void* message, RclUnityPoint value)
        {
            ThrowIfNull(message, nameof(message));
            var native = (NativeTypes.geometry_msgs__msg__Point*)message;
            native->x = value.X;
            native->y = value.Y;
            native->z = value.Z;
        }

        /// <inheritdoc/>
        public override RclUnityPoint Read(void* message)
        {
            ThrowIfNull(message, nameof(message));
            var native = (NativeTypes.geometry_msgs__msg__Point*)message;
            return new RclUnityPoint(native->x, native->y, native->z);
        }
    }
}
