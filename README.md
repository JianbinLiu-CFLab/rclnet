# Rcl.NET Unity Adapter

This fork is a Unity-focused fork of [`noelex/rclnet`](https://github.com/noelex/rclnet).

The current goal is not to maintain a full, general-purpose ROS 2 .NET client first. The current goal is to prove and grow a small Unity-local ROS 2 adapter that can run inside a Unity Player with a staged Windows ROS 2 native runtime closure.

In this fork, the primary new surface is:

```text
src/Rcl.NET.Unity
```

The upstream `Rcl.NET`, `Rosidl.Runtime`, and generator projects are still present for lineage and future reference, but they are not the default Unity integration path.

## Current Status

Status date: 2026-05-24

Validated locally:

- Windows 10 LTSC
- ROS 2 Jazzy Windows runtime
- `rmw_fastrtps_cpp`
- Unity 6000.3.14f1 Windows64 Player
- `Rcl.NET.Unity.dll` targeting `netstandard2.1`
- Standalone Unity Player runtime without launching through a local ROS environment wrapper

Validated Unity adapter smoke path:

```text
rcl init
node create
std_msgs/msg/String pub/sub
builtin_interfaces/msg/Time pub/sub
std_msgs/msg/Header pub/sub
geometry_msgs/msg/Vector3 pub/sub
geometry_msgs/msg/Quaternion pub/sub
geometry_msgs/msg/Point pub/sub
geometry_msgs/msg/Pose pub/sub
node/context shutdown
```

Latest validated commit:

```text
0779b92 feat: add unity core time header geometry profile
```

This is still an integration track, not a published release.

## Why This Fork Is Diverging

The upstream `rclnet` project is a broad .NET ROS 2 client library with async APIs, generated messages, graph APIs, services, actions, and a wider runtime model.

That is valuable, but Unity has different constraints:

- Unity Player must load managed and native dependencies predictably.
- A staged runtime closure is preferable to requiring a developer ROS installation on every target machine.
- Unity integrations often need a small, stable visual message profile before they need the entire ROS 2 interface universe.
- Avoiding `Rcl.NET.dll`, `Rosidl.Runtime.dll`, and generated-message runtime dependencies keeps the adapter lighter.

This fork is therefore moving toward a smaller adapter-first design:

```text
Rcl.NET.Unity.dll
small hand-coded message profiles
explicit native runtime staging
Unity Player smoke tests
```

## Repository Layout

Important paths:

```text
src/Rcl.NET.Unity
tools/rclnet-unity/Stage-RclNetUnityStandalone.ps1
```

Historical/upstream paths still present:

```text
src/Rcl.NET
src/Rosidl.Runtime
src/Rosidl.Generator.CSharp
src/ros2cs
examples
```

These are not removed yet, but the Unity adapter work should not assume they are part of the runtime closure.

## Rcl.NET.Unity API Surface

Current context/node API:

```csharp
using Rcl.Unity;

using var context = new RclUnityContext(Array.Empty<string>());
using var node = context.CreateNode("unity_node");
```

Current `std_msgs/msg/String` API:

```csharp
using var publisher = node.CreateStringPublisher("/unity/chatter");
using var subscription = node.CreateStringSubscription("/unity/chatter");

publisher.Publish("hello from unity");

if (subscription.TryTake(out var received))
{
    // received is a string
}
```

Current core profile API:

```csharp
var stamp = new RclUnityTime(123, 456_789_000);
var header = new RclUnityHeader(stamp, "map");
var point = new RclUnityPoint(1.0, 2.0, 3.0);
var rotation = new RclUnityQuaternion(0.0, 0.0, 0.7071067811865476, 0.7071067811865476);
var pose = new RclUnityPose(point, rotation);

using var headerPublisher = node.CreateHeaderPublisher("/unity/header");
using var posePublisher = node.CreatePosePublisher("/unity/pose");

headerPublisher.Publish(header);
posePublisher.Publish(pose);
```

Current supported message profile:

```text
std_msgs/msg/String
builtin_interfaces/msg/Time
std_msgs/msg/Header
geometry_msgs/msg/Vector3
geometry_msgs/msg/Quaternion
geometry_msgs/msg/Point
geometry_msgs/msg/Pose
```

Not supported yet:

```text
tf2_msgs/msg/TFMessage
geometry_msgs/msg/TransformStamped
sensor_msgs
foxglove_msgs
services
actions
parameters
full message generation
Linux package artifact
NuGet release
```

## Standalone Unity Runtime Closure

The adapter can be staged into a Unity project with:

```powershell
tools\rclnet-unity\Stage-RclNetUnityStandalone.ps1 `
  -JazzyRoot C:\ros2_jazzy\ros2-windows `
  -AdapterDll <path-to-Rcl.NET.Unity.dll> `
  -UnityProject <path-to-unity-project> `
  -ManifestPath <path-to-manifest.json>
```

The script stages:

- `Rcl.NET.Unity.dll`
- required ROS 2 native DLLs
- required ament resource index files under `Assets/StreamingAssets/rclnet_ros2`

The runtime smoke is expected to run without a machine ROS environment in `PATH`.

Do not stage these for the lightweight Unity adapter path:

```text
Rcl.NET.dll
Rosidl.Runtime.dll
```

## Build

Build only the Unity adapter:

```powershell
dotnet build .\src\Rcl.NET.Unity\Rcl.NET.Unity.csproj -c Release --nologo
```

Expected output:

```text
Build succeeded.
0 Warning(s)
0 Error(s)
```

The generated DLL is:

```text
src\Rcl.NET.Unity\bin\Release\netstandard2.1\Rcl.NET.Unity.dll
```

## Verification Policy

Do not claim support from source inspection alone. A change is considered verified only when it has a command, exit code, and key log marker.

Current important markers:

```text
RCLNET_UNITY_ADAPTER_CONTEXT_NODE_SMOKE_PASS
RCLNET_UNITY_ADAPTER_STRING_PUBSUB_SMOKE_PASS
RCLNET_UNITY_ADAPTER_STANDALONE_STRING_PUBSUB_SMOKE_PASS
RCLNET_UNITY_CORE_GEOMETRY_PROFILE_SMOKE_PASS
```

For this fork, a useful Unity adapter change should usually verify:

1. `dotnet build` for `src/Rcl.NET.Unity`
2. static scan showing no accidental `Rcl.NET.dll` / `Rosidl.Runtime.dll` dependency
3. Unity Editor load
4. Unity Windows64 Player build
5. Unity Player runtime smoke with staged native closure

## Relationship To Upstream

This repository is still a fork of [`noelex/rclnet`](https://github.com/noelex/rclnet).

Upstream remains the right reference for:

- full rclnet APIs
- async execution model
- generated message workflows
- services/actions/graph APIs
- NuGet-oriented .NET usage

This fork is the right place for:

- Unity-first runtime loading
- lightweight hand-coded message profiles
- staged Windows ROS 2 runtime closure
- Foxglove/Unity visual integration experiments

When updating this fork from upstream, keep Unity adapter changes isolated and re-run Unity Player smoke tests before trusting the result.

## Roadmap

Near-term:

- add `geometry_msgs/msg/TransformStamped`
- add `tf2_msgs/msg/TFMessage`
- validate `/tf` inside Unity Player

Then decide based on the next real integration blocker:

- camera image profile
- point cloud profile
- Foxglove-specific message profile
- custom project extension story

The adapter should grow profile by profile, not by importing the full ROS 2 message universe.

## License

This fork keeps the upstream license.

See:

```text
LICENSE
```

