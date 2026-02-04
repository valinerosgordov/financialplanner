# Temporary Icon Generator for Nexus Finance
# Creates a basic blue/green gradient icon with "NF" text

Add-Type -AssemblyName System.Drawing

$iconPath = Join-Path $PSScriptRoot "app.ico"

if (Test-Path $iconPath) {
    Write-Host "Icon already exists at: $iconPath" -ForegroundColor Yellow
    $overwrite = Read-Host "Overwrite? (y/n)"
    if ($overwrite -ne 'y') {
        Write-Host "Cancelled." -ForegroundColor Red
        exit
    }
}

Write-Host "Generating temporary icon..." -ForegroundColor Cyan

# Create multiple sizes for proper Windows icon
$sizes = @(256, 128, 64, 48, 32, 16)
$bitmaps = @()

foreach ($size in $sizes) {
    $bitmap = New-Object System.Drawing.Bitmap($size, $size)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $graphics.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::AntiAlias
    
    # Gradient background (Blue to Green)
    $rect = New-Object System.Drawing.Rectangle(0, 0, $size, $size)
    $brush = New-Object System.Drawing.Drawing2D.LinearGradientBrush(
        $rect,
        [System.Drawing.Color]::FromArgb(255, 41, 128, 185),  # Blue
        [System.Drawing.Color]::FromArgb(255, 39, 174, 96),   # Green
        45
    )
    $graphics.FillRectangle($brush, $rect)
    
    # Draw "NF" text
    $fontSize = [Math]::Floor($size * 0.4)
    $font = New-Object System.Drawing.Font("Segoe UI", $fontSize, [System.Drawing.FontStyle]::Bold)
    $textBrush = [System.Drawing.Brushes]::White
    
    $text = "NF"
    $textSize = $graphics.MeasureString($text, $font)
    $x = ($size - $textSize.Width) / 2
    $y = ($size - $textSize.Height) / 2
    
    $graphics.DrawString($text, $font, $textBrush, $x, $y)
    
    $graphics.Dispose()
    $bitmaps += $bitmap
}

# Save as multi-resolution icon
$iconStream = New-Object System.IO.MemoryStream
$iconWriter = New-Object System.IO.BinaryWriter($iconStream)

# ICO header
$iconWriter.Write([UInt16]0)  # Reserved
$iconWriter.Write([UInt16]1)  # Type (1 = ICO)
$iconWriter.Write([UInt16]$sizes.Length)  # Number of images

$imageOffset = 6 + (16 * $sizes.Length)

for ($i = 0; $i -lt $sizes.Length; $i++) {
    $size = $sizes[$i]
    $bitmap = $bitmaps[$i]
    
    # Convert bitmap to PNG
    $pngStream = New-Object System.IO.MemoryStream
    $bitmap.Save($pngStream, [System.Drawing.Imaging.ImageFormat]::Png)
    $pngData = $pngStream.ToArray()
    $pngStream.Dispose()
    
    # Icon directory entry
    $iconWriter.Write([Byte]($size % 256))  # Width
    $iconWriter.Write([Byte]($size % 256))  # Height
    $iconWriter.Write([Byte]0)              # Color palette
    $iconWriter.Write([Byte]0)              # Reserved
    $iconWriter.Write([UInt16]1)            # Color planes
    $iconWriter.Write([UInt16]32)           # Bits per pixel
    $iconWriter.Write([UInt32]$pngData.Length)  # Image size
    $iconWriter.Write([UInt32]$imageOffset)     # Image offset
    
    $imageOffset += $pngData.Length
}

# Write PNG data
for ($i = 0; $i -lt $sizes.Length; $i++) {
    $bitmap = $bitmaps[$i]
    $pngStream = New-Object System.IO.MemoryStream
    $bitmap.Save($pngStream, [System.Drawing.Imaging.ImageFormat]::Png)
    $iconWriter.Write($pngStream.ToArray())
    $pngStream.Dispose()
    $bitmap.Dispose()
}

# Save to file
[System.IO.File]::WriteAllBytes($iconPath, $iconStream.ToArray())
$iconWriter.Close()
$iconStream.Close()

Write-Host "✓ Icon created: $iconPath" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Run .\publish-release.bat to build the exe" -ForegroundColor White
Write-Host "2. (Optional) Replace app.ico with your custom icon" -ForegroundColor Gray
