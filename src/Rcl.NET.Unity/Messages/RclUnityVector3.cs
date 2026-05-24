using System;

namespace Rcl.Unity
{
    public readonly struct RclUnityVector3 : IEquatable<RclUnityVector3>
    {
        public RclUnityVector3(double x, double y, double z)
        {
            RclUnityMessageValidation.ThrowIfNonFinite(x, nameof(x));
            RclUnityMessageValidation.ThrowIfNonFinite(y, nameof(y));
            RclUnityMessageValidation.ThrowIfNonFinite(z, nameof(z));

            X = x;
            Y = y;
            Z = z;
        }

        public double X { get; }

        public double Y { get; }

        public double Z { get; }

        public bool Equals(RclUnityVector3 other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
        }

        public override bool Equals(object? obj)
        {
            return obj is RclUnityVector3 other && Equals(other);
        }

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
