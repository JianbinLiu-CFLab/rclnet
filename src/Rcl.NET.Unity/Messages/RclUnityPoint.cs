// Copyright (c) 2026 Jianbin Liu.
// Licensed under the MIT License.
// See LICENSE in the repository root for license information.
using System;

namespace Rcl.Unity
{
    /// <summary>
    /// Represents geometry_msgs/Point using finite double-precision coordinates.
    /// </summary>
    public readonly struct RclUnityPoint : IEquatable<RclUnityPoint>
    {
        /// <summary>
        /// Initializes a point with finite x, y, and z coordinates.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="z">The z coordinate.</param>
        public RclUnityPoint(double x, double y, double z)
        {
            RclUnityMessageValidation.ThrowIfNonFinite(x, nameof(x));
            RclUnityMessageValidation.ThrowIfNonFinite(y, nameof(y));
            RclUnityMessageValidation.ThrowIfNonFinite(z, nameof(z));

            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Gets the x coordinate.
        /// </summary>
        public double X { get; }

        /// <summary>
        /// Gets the y coordinate.
        /// </summary>
        public double Y { get; }

        /// <summary>
        /// Gets the z coordinate.
        /// </summary>
        public double Z { get; }

        /// <summary>
        /// Determines whether this point has the same coordinates as another point.
        /// </summary>
        /// <param name="other">The point to compare with.</param>
        /// <returns><see langword="true"/> when all coordinates are equal.</returns>
        public bool Equals(RclUnityPoint other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is RclUnityPoint other && Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = X.GetHashCode();
                hash = (hash * 397) ^ Y.GetHashCode();
                hash = (hash * 397) ^ Z.GetHashCode();
                return hash;
            }
        }
    }
}
