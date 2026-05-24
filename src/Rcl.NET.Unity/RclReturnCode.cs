// Copyright (c) 2026 Jianbin Liu.
// Licensed under the MIT License.
// See LICENSE in the repository root for license information.
namespace Rcl.Unity
{
    /// <summary>
    /// Contains the rcl return codes used by the Unity adapter's minimal native surface.
    /// </summary>
    internal enum RclReturnCode : int
    {
        /// <summary>
        /// The operation completed successfully.
        /// </summary>
        Ok = 0,

        /// <summary>
        /// A subscription take call found no available message.
        /// </summary>
        SubscriptionTakeFailed = 401
    }
}
