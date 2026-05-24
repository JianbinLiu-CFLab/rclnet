using System;

namespace Rcl.Unity
{
    public readonly struct RclUnityQuaternion : IEquatable<RclUnityQuaternion>
    {
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

        public double X { get; }

        public double Y { get; }

        public double Z { get; }

        public double W { get; }

        public bool Equals(RclUnityQuaternion other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z) && W.Equals(other.W);
        }

        public override bool Equals(object? obj)
        {
            return obj is RclUnityQuaternion other && Equals(other);
        }

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
