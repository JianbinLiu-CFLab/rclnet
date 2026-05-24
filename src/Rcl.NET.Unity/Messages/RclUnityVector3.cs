// Copyright (c) 2026 Jianbin Liu.
// Licensed under the MIT License.
// See LICENSE in the repository root for license information.
using System;

namespace Rcl.Unity
{
    /// <summary>
    /// Represents geometry_msgs/Vector3 using finite double-precision components.
    /// </summary>
    public readonly struct RclUnityVector3 : IEquatable<RclUnityVector3>
    {
        /// <summary>
        /// Initializes a vector with finite x, y, and z components.
        /// </summary>
        /// <param name="x">The x component.</param>
        /// <param name="y">The y component.</param>
        /// <param name="z">The z component.</param>
        public RclUnityVector3(double x, double y, double z)
        {
            RclUnityMessageValidation.ThrowIfNonFinite(x, nameof(x));
            RclUnityMessageValidation.ThrowIfNonFinite(y, nameof(y));
            RclUnityMessageValidation.ThrowIfNonFinite(z, nameof(z));

            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Gets the x component.
        /// </summary>
        public double X { get; }

        /// <summary>
        /// Gets the y component.
        /// </summary>
        public double Y { get; }

        /// <summary>
        /// Gets the z component.
        /// </summary>
        public double Z { get; }

        /// <summary>
        /// Determines whether this vector has the same components as another vector.
        /// </summary>
        /// <param name="other">The vector to compare with.</param>
        /// <returns><see langword="true"/> when all components are equal.</returns>
        public bool Equals(RclUnityVector3 other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is RclUnityVector3 other && Equals(other);
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
