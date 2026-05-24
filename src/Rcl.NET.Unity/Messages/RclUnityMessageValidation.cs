using System;

namespace Rcl.Unity
{
    internal static class RclUnityMessageValidation
    {
        public static void ThrowIfNonFinite(double value, string paramName)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                throw new ArgumentOutOfRangeException(paramName, "Value must be finite.");
            }
        }
    }
}
