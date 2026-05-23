using System;

namespace Rcl.Unity
{
    public sealed class RclUnityException : Exception
    {
        public RclUnityException(string message)
            : base(message)
        {
        }

        public RclUnityException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
