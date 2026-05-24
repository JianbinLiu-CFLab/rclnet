// Copyright (c) 2026 Jianbin Liu.
// Licensed under the MIT License.
// See LICENSE in the repository root for license information.
namespace Rcl.Unity
{
    /// <summary>
    /// Bridges std_msgs/Header between managed <see cref="RclUnityHeader"/> and the native C message.
    /// </summary>
    internal sealed unsafe class StdHeaderMessageBridge : NativeMessageBridge<RclUnityHeader>
    {
        /// <summary>
        /// Gets the singleton bridge instance for std_msgs/Header.
        /// </summary>
        public static readonly StdHeaderMessageBridge Instance = new StdHeaderMessageBridge();

        private StdHeaderMessageBridge()
            : base(NativeRcl.std_msgs_msg_header_get_type_support(), "std_msgs/Header")
        {
        }

        /// <inheritdoc/>
        public override void* Create()
        {
            var message = NativeRcl.std_msgs_msg_header_create();
            if (message == null)
            {
                throw new RclUnityException("std_msgs__msg__Header__create returned null.");
            }

            return message;
        }

        /// <inheritdoc/>
        public override void Destroy(void* message)
        {
            NativeRcl.std_msgs_msg_header_destroy((NativeTypes.std_msgs__msg__Header*)message);
        }

        /// <inheritdoc/>
        public override void Write(void* message, RclUnityHeader value)
        {
            ThrowIfNull(message, nameof(message));
            var native = (NativeTypes.std_msgs__msg__Header*)message;
            native->stamp.sec = value.Stamp.Sec;
            native->stamp.nanosec = value.Stamp.Nanosec;
            AssignString(&native->frame_id, value.FrameId);
        }

        /// <inheritdoc/>
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
