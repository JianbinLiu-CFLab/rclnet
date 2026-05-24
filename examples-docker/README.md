# Legacy Containerized rclnet Nodes

This directory contains the original upstream-style containerized rclnet node
examples. They are kept as reference material for the broader `Rcl.NET` stack,
but they are not the primary Unity adapter path for this fork.

For the current Unity-focused work, start from the repository root README and
`src/Rcl.NET.Unity` instead. The Unity adapter is validated through staged
Windows ROS 2 native runtime closure tests, not through these Docker examples.

To run these legacy examples, make sure Docker is installed. A host .NET SDK and
host ROS 2 installation are not required.

To build and run the example, simply run the following command from current directory:

```sh
docker compose -f docker-compose.yml up --build
```
