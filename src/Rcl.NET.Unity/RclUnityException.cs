// Copyright (c) 2026 Jianbin Liu.
// Licensed under the MIT License.
// See LICENSE in the repository root for license information.
using System;

namespace Rcl.Unity
{
    /// <summary>
    /// Represents an error reported by the lightweight Unity ROS 2 adapter.
    /// </summary>
    public sealed class RclUnityException : Exception
    {
        /// <summary>
        /// Initializes a new adapter exception with a message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public RclUnityException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new adapter exception with a message and inner exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The underlying exception.</param>
        public RclUnityException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
