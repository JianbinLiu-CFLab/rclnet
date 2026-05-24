using System;

namespace Rcl.Unity
{
    public readonly struct RclUnityPose : IEquatable<RclUnityPose>
    {
        public RclUnityPose(RclUnityPoint position, RclUnityQuaternion orientation)
        {
            Position = position;
            Orientation = orientation;
        }

        public RclUnityPoint Position { get; }

        public RclUnityQuaternion Orientation { get; }

        public bool Equals(RclUnityPose other)
        {
            return Position.Equals(other.Position) && Orientation.Equals(other.Orientation);
        }

        public override bool Equals(object? obj)
        {
            return obj is RclUnityPose other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Position.GetHashCode() * 397) ^ Orientation.GetHashCode();
            }
        }
    }
}
