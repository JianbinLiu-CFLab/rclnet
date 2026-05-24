// Copyright (c) 2026 Jianbin Liu.
// Licensed under the MIT License.
// See LICENSE in the repository root for license information.
using System;

namespace Rcl.Unity
{
    /// <summary>
    /// Represents the subset of std_msgs/Header used by the Unity adapter.
    /// </summary>
    public readonly struct RclUnityHeader : IEquatable<RclUnityHeader>
    {
        /// <summary>
        /// Initializes a header with a timestamp and frame identifier.
        /// </summary>
        /// <param name="stamp">The ROS 2 timestamp carried by the header.</param>
        /// <param name="frameId">The coordinate frame identifier.</param>
        public RclUnityHeader(RclUnityTime stamp, string frameId)
        {
            Stamp = stamp;
            FrameId = frameId ?? throw new ArgumentNullException(nameof(frameId));
        }

        /// <summary>
        /// Gets the timestamp carried by the header.
        /// </summary>
        public RclUnityTime Stamp { get; }

        /// <summary>
        /// Gets the coordinate frame identifier.
        /// </summary>
        public string FrameId { get; }

        /// <summary>
        /// Determines whether this header has the same stamp and frame id as another header.
        /// </summary>
        /// <param name="other">The header to compare with.</param>
        /// <returns><see langword="true"/> when the stamp and frame id are equal.</returns>
        public bool Equals(RclUnityHeader other)
        {
            return Stamp.Equals(other.Stamp) && string.Equals(FrameId, other.FrameId, StringComparison.Ordinal);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is RclUnityHeader other && Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                return (Stamp.GetHashCode() * 397) ^ StringComparer.Ordinal.GetHashCode(FrameId);
            }
        }
    }
}
