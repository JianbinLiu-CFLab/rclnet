// Copyright (c) 2026 Jianbin Liu.
// Licensed under the MIT License.
// See LICENSE in the repository root for license information.
using System;

namespace Rcl.Unity
{
    /// <summary>
    /// Represents a ROS 2 builtin_interfaces/Time value used by the Unity adapter profile.
    /// </summary>
    public readonly struct RclUnityTime : IEquatable<RclUnityTime>
    {
        /// <summary>
        /// Initializes a ROS 2 time value from whole seconds and nanoseconds.
        /// </summary>
        /// <param name="sec">Whole seconds in ROS 2 time representation.</param>
        /// <param name="nanosec">Nanoseconds within the current second; must be less than 1,000,000,000.</param>
        public RclUnityTime(int sec, uint nanosec)
        {
            if (nanosec > 999_999_999U)
            {
                throw new ArgumentOutOfRangeException(nameof(nanosec), "Nanosec must be in the range [0, 1e9).");
            }

            Sec = sec;
            Nanosec = nanosec;
        }

        /// <summary>
        /// Gets the whole-second portion of the time value.
        /// </summary>
        public int Sec { get; }

        /// <summary>
        /// Gets the nanosecond portion of the time value.
        /// </summary>
        public uint Nanosec { get; }

        /// <summary>
        /// Determines whether this value has the same seconds and nanoseconds as another value.
        /// </summary>
        /// <param name="other">The value to compare with.</param>
        /// <returns><see langword="true"/> when both components are equal.</returns>
        public bool Equals(RclUnityTime other)
        {
            return Sec == other.Sec && Nanosec == other.Nanosec;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is RclUnityTime other && Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                return (Sec * 397) ^ (int)Nanosec;
            }
        }
    }
}
