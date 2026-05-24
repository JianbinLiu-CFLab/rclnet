// Copyright (c) 2026 Jianbin Liu.
// Licensed under the MIT License.
// See LICENSE in the repository root for license information.
using System;

namespace Rcl.Unity
{
    /// <summary>
    /// Converts rcl return codes and rcutils error state into adapter exceptions.
    /// </summary>
    internal static unsafe class RclError
    {
        /// <summary>
        /// Throws an exception when an rcl operation did not return success.
        /// </summary>
        public static void ThrowIfNonSuccess(RclReturnCode returnCode, string operation)
        {
            if (returnCode == RclReturnCode.Ok)
            {
                return;
            }

            throw new RclUnityException(CreateMessage(returnCode, operation));
        }

        /// <summary>
        /// Creates a diagnostic message for an rcl operation result.
        /// </summary>
        public static string CreateMessage(RclReturnCode returnCode, string operation)
        {
            var nativeMessage = TryTakeNativeError();
            if (string.IsNullOrEmpty(nativeMessage))
            {
                nativeMessage = "native error state was not set";
            }

            return $"{operation} failed with {(int)returnCode}: {nativeMessage}";
        }

        /// <summary>
        /// Reads and clears the current native rcutils error message, if present.
        /// </summary>
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
