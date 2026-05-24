// Copyright (c) 2026 Jianbin Liu.
// Licensed under the MIT License.
// See LICENSE in the repository root for license information.
namespace Rcl.Unity
{
    /// <summary>
    /// Provides the common native dispose hook used by node-owned Unity adapter entities.
    /// </summary>
    internal unsafe interface IRclUnityNodeEntity
    {
        /// <summary>
        /// Finalizes the entity while the owning node still has a valid native node handle.
        /// </summary>
        /// <param name="nodePointer">The owning native node handle.</param>
        void DisposeFromNode(NativeTypes.rcl_node_t* nodePointer);
    }
}
