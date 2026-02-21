$ErrorActionPreference = "Stop"
$root = Get-Location

Write-Host ">>> Building DesktopServerLite (Standard)..." -ForegroundColor Magenta
Set-Location "$root\Lite"
.\build.ps1

Write-Host ">>> Building DesktopServerPro (Pro)..." -ForegroundColor Magenta
Set-Location "$root\Pro"
.\build.ps1

Write-Host ">>> Building DesktopServerGo (Go)..." -ForegroundColor Magenta
Set-Location "$root\Go"
.\build.ps1

Set-Location $root

Write-Host ">>> Preparing Deploy Directory..." -ForegroundColor Magenta
if (Test-Path ".\Deploy") { Remove-Item ".\Deploy\*" -Force }
else { New-Item -ItemType Directory ".\Deploy" | Out-Null }

Write-Host ">>> Copying Installers to Deploy Folder..." -ForegroundColor Magenta

$src1 = ".\Lite\Publish\DesktopServerSetupLite.exe"
if (Test-Path $src1) {
    Copy-Item $src1 ".\Deploy\DesktopServerSetupLite.exe" -Force
    Write-Host "   Copied DesktopServerSetupLite.exe to Deploy\" -ForegroundColor Green
} else {
    Write-Error "   DesktopServerSetupLite.exe not found!"
}

$src2 = ".\Pro\Publish\DesktopServerSetupPro.exe"
if (Test-Path $src2) {
    Copy-Item $src2 ".\Deploy\DesktopServerSetupPro.exe" -Force
    Write-Host "   Copied DesktopServerSetupPro.exe to Deploy\" -ForegroundColor Green
} else {
    Write-Error "   DesktopServerSetupPro.exe not found!"
}

$src3 = ".\Go\Publish\DesktopServerSetupGo.exe"
if (Test-Path $src3) {
    Copy-Item $src3 ".\Deploy\DesktopServerSetupGo.exe" -Force
    Write-Host "   Copied DesktopServerSetupGo.exe to Deploy\" -ForegroundColor Green
} else {
    Write-Error "   DesktopServerSetupGo.exe not found!"
}

# Helper for safe archiving to prevent locking issues
function Safe-Archive($source, $destZip, $dest7z) {
    if (-not (Test-Path $source)) { return }
    
    $maxRetries = 3
    $retryCount = 0
    $success = $false
    
    while (-not $success -and $retryCount -lt $maxRetries) {
        try {
            Write-Host "   Processing: $(Split-Path $source -Leaf)..." -ForegroundColor Gray
            Start-Sleep -Seconds 2 # Give OS/Antivirus a moment
            
            # Zip Archive
            Compress-Archive -Path $source -DestinationPath $destZip -Force
            
            # 7z Archive
            if (Test-Path $sevenZip) {
                & $sevenZip a -t7z $dest7z $source | Out-Null
            }
            $success = $true
        }
        catch {
            $retryCount++
            if ($retryCount -lt $maxRetries) {
                Write-Host "   Locked! Retrying in 2s... ($retryCount/$maxRetries)" -ForegroundColor Yellow
                Start-Sleep -Seconds 2
            } else {
                Write-Error "   Failed to archive $source after $maxRetries attempts."
            }
        }
    }
}

Write-Host ">>> Generating Archives..." -ForegroundColor Magenta
$sevenZip = "C:\Program Files\7-Zip\7z.exe"

Safe-Archive ".\Deploy\DesktopServerSetupLite.exe" ".\Deploy\DesktopServerSetupLite.zip" ".\Deploy\DesktopServerSetupLite.7z"
Safe-Archive ".\Deploy\DesktopServerSetupPro.exe" ".\Deploy\DesktopServerSetupPro.zip" ".\Deploy\DesktopServerSetupPro.7z"
Safe-Archive ".\Deploy\DesktopServerSetupGo.exe" ".\Deploy\DesktopServerSetupGo.zip" ".\Deploy\DesktopServerSetupGo.7z"

Write-Host "All Done! Final artifacts available in .\Deploy" -ForegroundColor Green
