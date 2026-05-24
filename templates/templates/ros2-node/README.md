# Company.Node1

This is the legacy upstream `rclnet` node template. It remains useful for
general .NET ROS 2 experiments, but it is not the recommended entry point for
the Unity-local adapter work in this fork.

For Unity integration, see the repository root README and `src/Rcl.NET.Unity`.
The current Unity adapter avoids depending on generated message assemblies for
its lightweight runtime profile.

Edit `ros2cs.spec` to include ROS 2 interface packages you wish to use.

Install `ros2cs` utility with the following command if not yet installed:

```sh
dotnet tool install -g ros2cs
```

After installation, simply run `ros2cs` in the project root to generate messages.

This project can also be built as a colcon package. Feel free to modify `package.xml`
and `CMakeLists.txt` to specify package dependencies and add other resources
you would like to include in the package.

See the root README for this fork's Unity adapter direction. See
https://github.com/noelex/rclnet for the original upstream project.
