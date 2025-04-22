# Define paths
$sourceAssets = ".\Assets"
$buildOutputBase = ".\Build"
$buildOutput = Join-Path -Path $buildOutputBase -ChildPath "DutyTally" # .\Build\DutyTally

# --- Define Asset folders managed by this script ---
# List all folders here that contain assets copied FROM .\Assets
# *** IMPORTANT: Do NOT include "Assemblies" in this list ***
$assetFoldersToCleanAndCopy = @(
    "About",
    "Languages"
#     "Defs",       # Add if you have Defs
#     "Textures",   # Add if you have Textures
#     "Patches"     # Add if you have Patches
    # Add any other folders like "Sounds", etc. that originate from .\Assets
)

Write-Host "Cleaning specific asset folders within: $buildOutput"

# --- Clean ONLY the specified asset folders ---
# Ensure the base output directory exists first
New-Item -ItemType Directory -Path $buildOutput -Force

# Loop through the defined asset folders and remove them if they exist
foreach ($folderName in $assetFoldersToCleanAndCopy) {
    $folderPath = Join-Path -Path $buildOutput -ChildPath $folderName
    if (Test-Path $folderPath) {
        Write-Host "Removing $folderPath..."
        Remove-Item -Path $folderPath -Recurse -Force -ErrorAction SilentlyContinue
    }
}

# --- Recreate Directory Structure & Copy Assets ---
Write-Host "Recreating structure and copying assets from $sourceAssets to $buildOutput..."

foreach ($folderName in $assetFoldersToCleanAndCopy) {
    $sourceFolderPath = Join-Path -Path $sourceAssets -ChildPath $folderName
    $destinationFolderPath = Join-Path -Path $buildOutput -ChildPath $folderName

    # Recreate the directory structure within the build output
    New-Item -ItemType Directory -Path $destinationFolderPath -Force

    # Copy assets if the source folder exists
    if (Test-Path $sourceFolderPath) {
        # Copy the *contents* of the source folder
        Copy-Item -Path (Join-Path -Path $sourceFolderPath -ChildPath "*") -Destination $destinationFolderPath -Recurse -Force -ErrorAction SilentlyContinue
    }
}

Write-Host "Asset copy complete. Assemblies folder should remain untouched."