# powershell -ExecutionPolicy Bypass -File convert-icon.ps1
# Converts PNG to ICO format with multiple icon sizes for Windows applications

# This script requires ImageMagick to be installed and available in PATH
# Download from: https://imagemagick.org/

$pngPath = "rosco.png"
$icoPath = "rosco.ico"

# Check if ImageMagick is available
$magick = Get-Command magick -ErrorAction SilentlyContinue

if ($magick) {
    # Use ImageMagick to create a proper ICO file with multiple sizes
    # This creates icons at 256, 128, 96, 64, 48, 32, and 16 pixel sizes
    & magick $pngPath -resize 256x256 -define icon:auto-resize=256,128,96,64,48,32,16 $icoPath
    Write-Host "Icon created successfully at $icoPath using ImageMagick"
} else {
    Write-Host "ERROR: ImageMagick not found. Please install ImageMagick from https://imagemagick.org/"
    exit 1
}
