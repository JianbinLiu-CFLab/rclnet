// Copyright (c) 2026 Jianbin Liu.
// Licensed under the MIT License.
// See LICENSE in the repository root for license information.
using System;

namespace Rcl.Unity
{
    /// <summary>
    /// Represents geometry_msgs/Pose as a position and orientation pair.
    /// </summary>
    public readonly struct RclUnityPose : IEquatable<RclUnityPose>
    {
        /// <summary>
        /// Initializes a pose from a position and an orientation.
        /// </summary>
        /// <param name="position">The position component of the pose.</param>
        /// <param name="orientation">The orientation component of the pose.</param>
        public RclUnityPose(RclUnityPoint position, RclUnityQuaternion orientation)
        {
            Position = position;
            Orientation = orientation;
        }

        /// <summary>
        /// Gets the position component.
        /// </summary>
        public RclUnityPoint Position { get; }

        /// <summary>
        /// Gets the orientation component.
        /// </summary>
        public RclUnityQuaternion Orientation { get; }

        /// <summary>
        /// Determines whether this pose has the same position and orientation as another pose.
        /// </summary>
        /// <param name="other">The pose to compare with.</param>
        /// <returns><see langword="true"/> when both components are equal.</returns>
        public bool Equals(RclUnityPose other)
        {
            return Position.Equals(other.Position) && Orientation.Equals(other.Orientation);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is RclUnityPose other && Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                return (Position.GetHashCode() * 397) ^ Orientation.GetHashCode();
            }
        }
    }
}
