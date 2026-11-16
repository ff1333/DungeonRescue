$ErrorActionPreference = "Stop"

$project = Split-Path -Parent $PSScriptRoot
$packageDirectory = Join-Path $project "Builds\Packages\v1.5.2"
$windowsSource = Join-Path $project "Builds\Windows\v1.5.2"
$webglSource = Join-Path $project "Builds\WebGL\v1.5.2"
$androidSource = Join-Path $project "Builds\Android\DungeonRescue-v1.5.2.apk"
$windowsArchive = Join-Path $packageDirectory "DungeonRescue-Windows-v1.5.2.zip"
$webglArchive = Join-Path $packageDirectory "DungeonRescue-WebGL-v1.5.2.zip"
$androidPackage = Join-Path $packageDirectory "DungeonRescue-v1.5.2.apk"
$checksumFile = Join-Path $packageDirectory "SHA256SUMS.txt"

foreach ($source in @($windowsSource, $webglSource, $androidSource)) {
    if (-not (Test-Path -LiteralPath $source)) {
        throw "Missing release build: $source"
    }
}

New-Item -ItemType Directory -Force -Path $packageDirectory | Out-Null
$windowsItems = Get-ChildItem -LiteralPath $windowsSource | Where-Object { $_.Name -notlike "release-smoke*" }
Compress-Archive -Path $windowsItems.FullName -DestinationPath $windowsArchive -CompressionLevel Optimal -Force
$webglItems = Get-ChildItem -LiteralPath $webglSource
Compress-Archive -Path $webglItems.FullName -DestinationPath $webglArchive -CompressionLevel Optimal -Force
Copy-Item -LiteralPath $androidSource -Destination $androidPackage -Force

$lines = @()
foreach ($file in @($windowsArchive, $webglArchive, $androidPackage)) {
    $hash = Get-FileHash -LiteralPath $file -Algorithm SHA256
    $lines += "$($hash.Hash.ToLowerInvariant())  $([System.IO.Path]::GetFileName($file))"
}
[System.IO.File]::WriteAllLines($checksumFile, $lines, [System.Text.UTF8Encoding]::new($false))

Get-ChildItem -LiteralPath $packageDirectory -File | Select-Object Name, Length
Get-Content -LiteralPath $checksumFile
