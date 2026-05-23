using System;

namespace Rcl.Unity
{
    internal static unsafe class RclError
    {
        public static void ThrowIfNonSuccess(RclReturnCode returnCode, string operation)
        {
            if (returnCode == RclReturnCode.Ok)
            {
                return;
            }

            throw new RclUnityException(CreateMessage(returnCode, operation));
        }

        public static string CreateMessage(RclReturnCode returnCode, string operation)
        {
            var nativeMessage = TryTakeNativeError();
            if (string.IsNullOrEmpty(nativeMessage))
            {
                nativeMessage = "native error state was not set";
            }

            return $"{operation} failed with {(int)returnCode}: {nativeMessage}";
        }

        private static string? TryTakeNativeError()
        {
            try
            {
                if (!NativeRcl.rcutils_error_is_set())
                {
                    return null;
                }

                var error = NativeRcl.rcutils_get_error_string();
                return NativeString.FromNullTerminatedUtf8(error.str);
            }
            finally
            {
                NativeRcl.rcutils_reset_error();
            }
        }
    }
}
