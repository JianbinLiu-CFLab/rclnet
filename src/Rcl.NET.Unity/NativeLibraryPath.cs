using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Rcl.Unity
{
    internal static class NativeLibraryPath
    {
        private static readonly object Mutex = new object();
        private static bool configured;

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
