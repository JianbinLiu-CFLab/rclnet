using System;

namespace Rcl.Unity
{
    public readonly struct RclUnityHeader : IEquatable<RclUnityHeader>
    {
        public RclUnityHeader(RclUnityTime stamp, string frameId)
        {
            Stamp = stamp;
            FrameId = frameId ?? throw new ArgumentNullException(nameof(frameId));
        }

        public RclUnityTime Stamp { get; }

        public string FrameId { get; }

        public bool Equals(RclUnityHeader other)
        {
            return Stamp.Equals(other.Stamp) && string.Equals(FrameId, other.FrameId, StringComparison.Ordinal);
        }

        public override bool Equals(object? obj)
        {
            return obj is RclUnityHeader other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Stamp.GetHashCode() * 397) ^ StringComparer.Ordinal.GetHashCode(FrameId);
            }
        }
    }
}
