// Copyright (c) 2026 Jianbin Liu.
// Licensed under the MIT License.
// See LICENSE in the repository root for license information.
using System;

namespace Rcl.Unity
{
    /// <summary>
    /// Represents geometry_msgs/Quaternion using finite double-precision components.
    /// </summary>
    public readonly struct RclUnityQuaternion : IEquatable<RclUnityQuaternion>
    {
        /// <summary>
        /// Initializes a quaternion with finite x, y, z, and w components.
        /// </summary>
        /// <param name="x">The x component.</param>
        /// <param name="y">The y component.</param>
        /// <param name="z">The z component.</param>
        /// <param name="w">The w component.</param>
        public RclUnityQuaternion(double x, double y, double z, double w)
        {
            RclUnityMessageValidation.ThrowIfNonFinite(x, nameof(x));
            RclUnityMessageValidation.ThrowIfNonFinite(y, nameof(y));
            RclUnityMessageValidation.ThrowIfNonFinite(z, nameof(z));
            RclUnityMessageValidation.ThrowIfNonFinite(w, nameof(w));

            X = x;
            Y = y;
            Z = z;
            W = w;
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
        /// Gets the w component.
        /// </summary>
        public double W { get; }

        /// <summary>
        /// Determines whether this quaternion has the same components as another quaternion.
        /// </summary>
        /// <param name="other">The quaternion to compare with.</param>
        /// <returns><see langword="true"/> when all components are equal.</returns>
        public bool Equals(RclUnityQuaternion other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z) && W.Equals(other.W);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is RclUnityQuaternion other && Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = X.GetHashCode();
                hash = (hash * 397) ^ Y.GetHashCode();
                hash = (hash * 397) ^ Z.GetHashCode();
                hash = (hash * 397) ^ W.GetHashCode();
                return hash;
            }
        }
    }
}
