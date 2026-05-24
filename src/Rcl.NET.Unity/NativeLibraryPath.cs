// Copyright (c) 2026 Jianbin Liu.
// Licensed under the MIT License.
// See LICENSE in the repository root for license information.
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Rcl.Unity
{
    /// <summary>
    /// Configures native DLL and ament resource lookup paths for a staged Unity Player runtime.
    /// </summary>
    internal static class NativeLibraryPath
    {
        /// <summary>
        /// Guards one-time environment mutation.
        /// </summary>
        private static readonly object Mutex = new object();

        /// <summary>
        /// Tracks whether the process environment has already been configured.
        /// </summary>
        private static bool configured;

        /// <summary>
        /// Adds Unity plugin and StreamingAssets paths to the process environment once.
        /// </summary>
        public static void Configure()
        {
            lock (Mutex)
            {
                if (configured)
                {
                    return;
                }

                foreach (var directory in GetCandidateDirectories())
                {
                    if (Directory.Exists(directory))
                    {
                        PrependEnvironmentPath("PATH", directory);
                    }
                }

                foreach (var prefix in GetCandidateAmentPrefixes())
                {
                    if (Directory.Exists(Path.Combine(prefix, "share", "ament_index")))
                    {
                        PrependEnvironmentPath("AMENT_PREFIX_PATH", prefix);
                    }
                }

                configured = true;
            }
        }

        /// <summary>
        /// Returns likely directories containing staged native ROS 2 DLLs.
        /// </summary>
        private static IEnumerable<string> GetCandidateDirectories()
        {
            var assemblyLocation = typeof(NativeLibraryPath).GetTypeInfo().Assembly.Location;
            if (!string.IsNullOrEmpty(assemblyLocation))
            {
                var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
                if (!string.IsNullOrEmpty(assemblyDirectory))
                {
                    yield return Path.Combine(assemblyDirectory, "x86_64");
                    yield return Path.GetFullPath(Path.Combine(assemblyDirectory, "..", "Plugins", "x86_64"));
                }
            }

            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (!string.IsNullOrEmpty(baseDirectory))
            {
                yield return Path.Combine(baseDirectory, "Plugins", "x86_64");
                yield return Path.GetFullPath(Path.Combine(baseDirectory, "..", "Plugins", "x86_64"));
            }
        }

        /// <summary>
        /// Returns likely ament prefix directories staged beside the Unity Player.
        /// </summary>
        private static IEnumerable<string> GetCandidateAmentPrefixes()
        {
            var assemblyLocation = typeof(NativeLibraryPath).GetTypeInfo().Assembly.Location;
            if (!string.IsNullOrEmpty(assemblyLocation))
            {
                var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
                if (!string.IsNullOrEmpty(assemblyDirectory))
                {
                    yield return Path.GetFullPath(Path.Combine(assemblyDirectory, "..", "StreamingAssets", "rclnet_ros2"));
                }
            }

            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (!string.IsNullOrEmpty(baseDirectory))
            {
                yield return Path.Combine(baseDirectory, "StreamingAssets", "rclnet_ros2");
                yield return Path.GetFullPath(Path.Combine(baseDirectory, "..", "StreamingAssets", "rclnet_ros2"));
            }
        }

        /// <summary>
        /// Prepends a directory to an environment path variable if it is not already present.
        /// </summary>
        private static void PrependEnvironmentPath(string variableName, string directory)
        {
            var currentPath = Environment.GetEnvironmentVariable(variableName) ?? string.Empty;
            var entries = currentPath.Split(new[] { Path.PathSeparator }, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < entries.Length; i++)
            {
                if (string.Equals(entries[i], directory, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }

            Environment.SetEnvironmentVariable(variableName, directory + Path.PathSeparator + currentPath);
        }
    }
}
