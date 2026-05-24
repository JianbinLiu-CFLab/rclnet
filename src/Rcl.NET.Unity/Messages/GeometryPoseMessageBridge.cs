// Copyright (c) 2026 Jianbin Liu.
// Licensed under the MIT License.
// See LICENSE in the repository root for license information.
namespace Rcl.Unity
{
    /// <summary>
    /// Bridges geometry_msgs/Pose between managed <see cref="RclUnityPose"/> and the native C message.
    /// </summary>
    internal sealed unsafe class GeometryPoseMessageBridge : NativeMessageBridge<RclUnityPose>
    {
        /// <summary>
        /// Gets the singleton bridge instance for geometry_msgs/Pose.
        /// </summary>
        public static readonly GeometryPoseMessageBridge Instance = new GeometryPoseMessageBridge();

        private GeometryPoseMessageBridge()
            : base(NativeRcl.geometry_msgs_msg_pose_get_type_support(), "geometry_msgs/Pose")
        {
        }

        /// <inheritdoc/>
        public override void* Create()
        {
            var message = NativeRcl.geometry_msgs_msg_pose_create();
            if (message == null)
            {
                throw new RclUnityException("geometry_msgs__msg__Pose__create returned null.");
            }

            return message;
        }

        /// <inheritdoc/>
        public override void Destroy(void* message)
        {
            NativeRcl.geometry_msgs_msg_pose_destroy((NativeTypes.geometry_msgs__msg__Pose*)message);
        }

        /// <inheritdoc/>
        public override void Write(void* message, RclUnityPose value)
        {
            ThrowIfNull(message, nameof(message));
            var native = (NativeTypes.geometry_msgs__msg__Pose*)message;
            native->position.x = value.Position.X;
            native->position.y = value.Position.Y;
            native->position.z = value.Position.Z;
            native->orientation.x = value.Orientation.X;
            native->orientation.y = value.Orientation.Y;
            native->orientation.z = value.Orientation.Z;
            native->orientation.w = value.Orientation.W;
        }

        /// <inheritdoc/>
        public override RclUnityPose Read(void* message)
        {
            ThrowIfNull(message, nameof(message));
            var native = (NativeTypes.geometry_msgs__msg__Pose*)message;
            return new RclUnityPose(
                new RclUnityPoint(native->position.x, native->position.y, native->position.z),
                new RclUnityQuaternion(
                    native->orientation.x,
                    native->orientation.y,
                    native->orientation.z,
                    native->orientation.w));
        }
    }
}
