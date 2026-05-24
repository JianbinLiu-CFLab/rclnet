# Company.MessageLibrary1

This is the legacy upstream `rclnet` message-library template. It is kept for
general .NET ROS 2 code generation experiments, but it is not the recommended
entry point for the Unity-local adapter work in this fork.

For Unity integration, see the repository root README and `src/Rcl.NET.Unity`.
The current Unity adapter grows a small hand-validated message profile before
bringing in broader generated-message support.

Edit `ros2cs.spec` to include ROS 2 interface packages you wish to use.

Install `ros2cs` utility with the following command if not yet installed:

```sh
dotnet tool install -g ros2cs
```

After installation, simply run `ros2cs` in the project root to generate messages.

See the root README for this fork's Unity adapter direction. See
https://github.com/noelex/rclnet for the original upstream project.
