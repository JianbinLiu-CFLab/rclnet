// Copyright (c) 2026 Jianbin Liu.
// Licensed under the MIT License.
// See LICENSE in the repository root for license information.
namespace Rcl.Unity
{
    /// <summary>
    /// Bridges builtin_interfaces/Time between managed <see cref="RclUnityTime"/> and the native C message.
    /// </summary>
    internal sealed unsafe class BuiltinTimeMessageBridge : NativeMessageBridge<RclUnityTime>
    {
        /// <summary>
        /// Gets the singleton bridge instance for builtin_interfaces/Time.
        /// </summary>
        public static readonly BuiltinTimeMessageBridge Instance = new BuiltinTimeMessageBridge();

        private BuiltinTimeMessageBridge()
            : base(NativeRcl.builtin_interfaces_msg_time_get_type_support(), "builtin_interfaces/Time")
        {
        }

        /// <inheritdoc/>
        public override void* Create()
        {
            var message = NativeRcl.builtin_interfaces_msg_time_create();
            if (message == null)
            {
                throw new RclUnityException("builtin_interfaces__msg__Time__create returned null.");
            }

            return message;
        }

        /// <inheritdoc/>
        public override void Destroy(void* message)
        {
            NativeRcl.builtin_interfaces_msg_time_destroy((NativeTypes.builtin_interfaces__msg__Time*)message);
        }

        /// <inheritdoc/>
        public override void Write(void* message, RclUnityTime value)
        {
            ThrowIfNull(message, nameof(message));
            var native = (NativeTypes.builtin_interfaces__msg__Time*)message;
            native->sec = value.Sec;
            native->nanosec = value.Nanosec;
        }

        /// <inheritdoc/>
        public override RclUnityTime Read(void* message)
        {
            ThrowIfNull(message, nameof(message));
            var native = (NativeTypes.builtin_interfaces__msg__Time*)message;
            return new RclUnityTime(native->sec, native->nanosec);
        }
    }
}
