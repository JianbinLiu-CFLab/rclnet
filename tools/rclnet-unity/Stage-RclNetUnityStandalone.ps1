Param(
    [Parameter(Mandatory=$true)]
    [string]$JazzyRoot,

    [Parameter(Mandatory=$true)]
    [string]$AdapterDll,

    [Parameter(Mandatory=$true)]
    [string]$UnityProject,

    [string]$ManifestPath
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

function Resolve-RequiredPath {
    Param(
        [Parameter(Mandatory=$true)]
        [string]$Path,

        [Parameter(Mandatory=$true)]
        [string]$Description
    )

    if (-not (Test-Path -LiteralPath $Path)) {
        throw "$Description does not exist: $Path"
    }

    return (Resolve-Path -LiteralPath $Path).Path
}

function Add-ExistingDirectory {
    Param(
        [System.Collections.Generic.List[string]]$Directories,

        [Parameter(Mandatory=$true)]
        [string]$Path
    )

    if (Test-Path -LiteralPath $Path -PathType Container) {
        $resolved = (Resolve-Path -LiteralPath $Path).Path
        if (-not $Directories.Contains($resolved)) {
            $Directories.Add($resolved)
        }
    }
}

function Find-Dumpbin {
    $command = Get-Command dumpbin -ErrorAction SilentlyContinue
    if ($command -ne $null) {
        return $command.Source
    }

    $patterns = @(
        "$env:ProgramFiles\Microsoft Visual Studio\18\*\VC\Tools\MSVC\*\bin\Hostx64\x64\dumpbin.exe",
        "$env:ProgramFiles\Microsoft Visual Studio\2022\*\VC\Tools\MSVC\*\bin\Hostx64\x64\dumpbin.exe"
    )

    foreach ($pattern in $patterns) {
        $match = Get-ChildItem -Path $pattern -ErrorAction SilentlyContinue |
            Sort-Object FullName -Descending |
            Select-Object -First 1
        if ($match -ne $null) {
            return $match.FullName
        }
    }

    return $null
}

function Test-SystemDll {
    Param(
        [Parameter(Mandatory=$true)]
        [string]$Name
    )

    $upper = $Name.ToUpperInvariant()
    if ($upper -match '^API-MS-WIN-.*\.DLL$' -or $upper -match '^EXT-MS-.*\.DLL$') {
        return $true
    }

    if ($upper -match '^MSVCP[0-9A-Z_]*\.DLL$' -or $upper -match '^VCRUNTIME[0-9A-Z_]*\.DLL$') {
        return $true
    }

    $systemDlls = @(
        'ADVAPI32.DLL',
        'BCRYPT.DLL',
        'COMDLG32.DLL',
        'CRYPT32.DLL',
        'DNSAPI.DLL',
        'GDI32.DLL',
        'IMM32.DLL',
        'IPHLPAPI.DLL',
        'KERNEL32.DLL',
        'MSWSOCK.DLL',
        'NORMALIZ.DLL',
        'NTDLL.DLL',
        'OLE32.DLL',
        'OLEAUT32.DLL',
        'RPCRT4.DLL',
        'SECUR32.DLL',
        'SHELL32.DLL',
        'SHLWAPI.DLL',
        'UCRTBASE.DLL',
        'USER32.DLL',
        'USERENV.DLL',
        'VERSION.DLL',
        'WINMM.DLL',
        'WS2_32.DLL',
        'WSOCK32.DLL'
    )

    return $systemDlls -contains $upper
}

function Get-DumpbinDependencies {
    Param(
        [Parameter(Mandatory=$true)]
        [string]$DumpbinPath,

        [Parameter(Mandatory=$true)]
        [string]$DllPath
    )

    $lines = & $DumpbinPath /DEPENDENTS $DllPath 2>&1
    if ($LASTEXITCODE -ne 0) {
        throw "dumpbin failed for $DllPath"
    }

    $dependencies = New-Object System.Collections.Generic.List[string]
    foreach ($line in $lines) {
        $trimmed = ($line | Out-String).Trim()
        if ($trimmed -match '^[A-Za-z0-9_.+\-]+\.dll$') {
            if (-not $dependencies.Contains($trimmed)) {
                $dependencies.Add($trimmed)
            }
        }
    }

    return $dependencies
}

$JazzyRoot = Resolve-RequiredPath -Path $JazzyRoot -Description 'Jazzy root'
$AdapterDll = Resolve-RequiredPath -Path $AdapterDll -Description 'Rcl.NET.Unity adapter DLL'
$UnityProject = Resolve-RequiredPath -Path $UnityProject -Description 'Unity project'
$assetsPath = Resolve-RequiredPath -Path (Join-Path $UnityProject 'Assets') -Description 'Unity Assets directory'
$jazzyBin = Resolve-RequiredPath -Path (Join-Path $JazzyRoot 'bin') -Description 'Jazzy bin directory'

if ([string]::IsNullOrWhiteSpace($ManifestPath)) {
    $ManifestPath = Join-Path $UnityProject 'Assets\Plugins\rclnet-unity-standalone-closure-manifest.json'
}

$pluginRoot = Join-Path $assetsPath 'Plugins'
$nativeRoot = Join-Path $pluginRoot 'x86_64'
$streamingPrefix = Join-Path $assetsPath 'StreamingAssets\rclnet_ros2'
$resourceDestination = Join-Path $streamingPrefix 'share\ament_index\resource_index'
if (Test-Path -LiteralPath $nativeRoot) {
    Remove-Item -LiteralPath $nativeRoot -Recurse -Force
}

if (Test-Path -LiteralPath $streamingPrefix) {
    Remove-Item -LiteralPath $streamingPrefix -Recurse -Force
}

New-Item -ItemType Directory -Force -Path $pluginRoot, $nativeRoot, $resourceDestination | Out-Null

$managedDestination = Join-Path $pluginRoot 'Rcl.NET.Unity.dll'
Copy-Item -LiteralPath $AdapterDll -Destination $managedDestination -Force

$resourceSource = Resolve-RequiredPath -Path (Join-Path $JazzyRoot 'share\ament_index\resource_index') -Description 'Jazzy ament resource index'
Copy-Item -Path (Join-Path $resourceSource '*') -Destination $resourceDestination -Recurse -Force

$allowedDirectories = New-Object System.Collections.Generic.List[string]
Add-ExistingDirectory -Directories $allowedDirectories -Path $jazzyBin
Add-ExistingDirectory -Directories $allowedDirectories -Path (Join-Path $JazzyRoot 'lib')
Add-ExistingDirectory -Directories $allowedDirectories -Path (Join-Path $JazzyRoot '.pixi\envs\default\Library\bin')

$optPath = Join-Path $JazzyRoot 'opt'
if (Test-Path -LiteralPath $optPath -PathType Container) {
    foreach ($vendorDirectory in Get-ChildItem -LiteralPath $optPath -Directory) {
        Add-ExistingDirectory -Directories $allowedDirectories -Path (Join-Path $vendorDirectory.FullName 'bin')
        Add-ExistingDirectory -Directories $allowedDirectories -Path (Join-Path $vendorDirectory.FullName 'lib')
    }
}

$dllIndex = @{}
foreach ($directory in $allowedDirectories) {
    foreach ($dll in Get-ChildItem -LiteralPath $directory -File -Filter '*.dll' -ErrorAction SilentlyContinue) {
        $key = $dll.Name.ToLowerInvariant()
        if (-not $dllIndex.ContainsKey($key)) {
            $dllIndex[$key] = $dll.FullName
        }
    }
}

$seedDlls = @(
    'rcl.dll',
    'rcutils.dll',
    'rmw.dll',
    'rmw_dds_common__rosidl_typesupport_fastrtps_cpp.dll',
    'rmw_dds_common__rosidl_typesupport_introspection_cpp.dll',
    'rmw_implementation.dll',
    'rmw_fastrtps_cpp.dll',
    'rosidl_runtime_c.dll',
    'rosidl_typesupport_c.dll',
    'rosidl_typesupport_interface.dll',
    'builtin_interfaces__rosidl_generator_c.dll',
    'builtin_interfaces__rosidl_typesupport_c.dll',
    'builtin_interfaces__rosidl_typesupport_fastrtps_c.dll',
    'builtin_interfaces__rosidl_typesupport_introspection_c.dll',
    'std_msgs__rosidl_generator_c.dll',
    'std_msgs__rosidl_typesupport_c.dll',
    'std_msgs__rosidl_typesupport_cpp.dll',
    'std_msgs__rosidl_typesupport_fastrtps_c.dll',
    'std_msgs__rosidl_typesupport_fastrtps_cpp.dll',
    'std_msgs__rosidl_typesupport_introspection_c.dll',
    'std_msgs__rosidl_typesupport_introspection_cpp.dll',
    'geometry_msgs__rosidl_generator_c.dll',
    'geometry_msgs__rosidl_typesupport_c.dll',
    'geometry_msgs__rosidl_typesupport_cpp.dll',
    'geometry_msgs__rosidl_typesupport_fastrtps_c.dll',
    'geometry_msgs__rosidl_typesupport_fastrtps_cpp.dll',
    'geometry_msgs__rosidl_typesupport_introspection_c.dll',
    'geometry_msgs__rosidl_typesupport_introspection_cpp.dll'
)

$dumpbin = Find-Dumpbin
$mode = if ($dumpbin -ne $null) { 'dependency-derived' } else { 'explicit-copy' }
$queue = New-Object System.Collections.Queue
$staged = @{}
$nativeFiles = New-Object System.Collections.Generic.List[object]
$skippedSystemFiles = New-Object System.Collections.Generic.List[string]
$missingFiles = New-Object System.Collections.Generic.List[object]

foreach ($seed in $seedDlls) {
    $key = $seed.ToLowerInvariant()
    if ($dllIndex.ContainsKey($key)) {
        $queue.Enqueue($dllIndex[$key])
    } else {
        $missingFiles.Add([ordered]@{
            name = $seed
            reason = 'seed-not-found'
            requested_by = '<seed>'
        })
    }
}

while ($queue.Count -gt 0) {
    $source = [string]$queue.Dequeue()
    $name = [System.IO.Path]::GetFileName($source)
    $key = $name.ToLowerInvariant()
    if ($staged.ContainsKey($key)) {
        continue
    }

    $destination = Join-Path $nativeRoot $name
    Copy-Item -LiteralPath $source -Destination $destination -Force
    $staged[$key] = $destination
    $nativeFiles.Add([ordered]@{
        name = $name
        source = $source
        destination = $destination
        length = (Get-Item -LiteralPath $destination).Length
    })

    if ($dumpbin -eq $null) {
        continue
    }

    foreach ($dependency in Get-DumpbinDependencies -DumpbinPath $dumpbin -DllPath $source) {
        $dependencyKey = $dependency.ToLowerInvariant()
        if (Test-SystemDll -Name $dependency) {
            if (-not $skippedSystemFiles.Contains($dependency)) {
                $skippedSystemFiles.Add($dependency)
            }
            continue
        }

        if ($staged.ContainsKey($dependencyKey)) {
            continue
        }

        if ($dllIndex.ContainsKey($dependencyKey)) {
            $queue.Enqueue($dllIndex[$dependencyKey])
            continue
        }

        $alreadyMissing = $false
        foreach ($missing in $missingFiles) {
            if ($missing.name -eq $dependency -and $missing.requested_by -eq $name) {
                $alreadyMissing = $true
                break
            }
        }

        if (-not $alreadyMissing) {
            $missingFiles.Add([ordered]@{
                name = $dependency
                reason = 'dependency-not-found'
                requested_by = $name
            })
        }
    }
}

$manifestDirectory = [System.IO.Path]::GetDirectoryName($ManifestPath)
if (-not [string]::IsNullOrWhiteSpace($manifestDirectory)) {
    New-Item -ItemType Directory -Force -Path $manifestDirectory | Out-Null
}

$resourceFiles = New-Object System.Collections.Generic.List[object]
foreach ($resourceFile in Get-ChildItem -LiteralPath $resourceDestination -Recurse -File) {
    $resourceFiles.Add([ordered]@{
        relative_path = $resourceFile.FullName.Substring($resourceDestination.Length).TrimStart('\', '/')
        destination = $resourceFile.FullName
        length = $resourceFile.Length
    })
}

$manifest = [ordered]@{
    generated_at = (Get-Date).ToUniversalTime().ToString('o')
    jazzy_root = $JazzyRoot
    adapter_dll = $AdapterDll
    unity_project = $UnityProject
    mode = $mode
    dumpbin = $dumpbin
    allowed_directories = $allowedDirectories
    managed_files = @([ordered]@{
        name = 'Rcl.NET.Unity.dll'
        source = $AdapterDll
        destination = $managedDestination
        length = (Get-Item -LiteralPath $managedDestination).Length
    })
    native_files = $nativeFiles
    resource_prefix = $streamingPrefix
    resource_files = $resourceFiles
    skipped_system_files = $skippedSystemFiles
    missing_files = $missingFiles
}

$manifest | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $ManifestPath -Encoding UTF8

Write-Output "RCLNET_UNITY_STANDALONE_STAGE_MODE=$mode"
Write-Output "RCLNET_UNITY_STANDALONE_MANAGED_COUNT=1"
Write-Output "RCLNET_UNITY_STANDALONE_NATIVE_COUNT=$($nativeFiles.Count)"
Write-Output "RCLNET_UNITY_STANDALONE_RESOURCE_COUNT=$($resourceFiles.Count)"
Write-Output "RCLNET_UNITY_STANDALONE_MISSING_COUNT=$($missingFiles.Count)"
Write-Output "RCLNET_UNITY_STANDALONE_MANIFEST=$ManifestPath"
