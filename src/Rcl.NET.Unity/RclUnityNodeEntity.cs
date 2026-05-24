namespace Rcl.Unity
{
    internal unsafe interface IRclUnityNodeEntity
    {
        void DisposeFromNode(NativeTypes.rcl_node_t* nodePointer);
    }
}
