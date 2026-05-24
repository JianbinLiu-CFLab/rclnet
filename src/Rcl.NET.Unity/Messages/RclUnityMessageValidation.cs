// Copyright (c) 2026 Jianbin Liu.
// Licensed under the MIT License.
// See LICENSE in the repository root for license information.
using System;

namespace Rcl.Unity
{
    /// <summary>
    /// Validates managed message values before they are written into native ROS 2 messages.
    /// </summary>
    internal static class RclUnityMessageValidation
    {
        /// <summary>
        /// Throws when a floating-point value cannot be represented as a finite ROS 2 numeric field.
        /// </summary>
        public static void ThrowIfNonFinite(double value, string paramName)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                throw new ArgumentOutOfRangeException(paramName, "Value must be finite.");
            }
        }
    }
}
