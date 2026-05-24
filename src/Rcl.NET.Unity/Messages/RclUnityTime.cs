using System;

namespace Rcl.Unity
{
    public readonly struct RclUnityTime : IEquatable<RclUnityTime>
    {
        public RclUnityTime(int sec, uint nanosec)
        {
            if (nanosec > 999_999_999U)
            {
                throw new ArgumentOutOfRangeException(nameof(nanosec), "Nanosec must be in the range [0, 1e9).");
            }

            Sec = sec;
            Nanosec = nanosec;
        }

        public int Sec { get; }

        public uint Nanosec { get; }

        public bool Equals(RclUnityTime other)
        {
            return Sec == other.Sec && Nanosec == other.Nanosec;
        }

        public override bool Equals(object? obj)
        {
            return obj is RclUnityTime other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Sec * 397) ^ (int)Nanosec;
            }
        }
    }
}
