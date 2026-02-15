$ErrorActionPreference = "Stop"
$root = Get-Location

Write-Host ">>> Building DesktopServerLite (Standard)..." -ForegroundColor Magenta
Set-Location "$root\Lite"
.\build.ps1

Write-Host ">>> Building DesktopServerPro (Pro)..." -ForegroundColor Magenta
Set-Location "$root\Pro"
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

Write-Host ">>> Generating Archives..." -ForegroundColor Magenta
$sevenZip = "C:\Program Files\7-Zip\7z.exe"

# Lite Archives
Write-Host "   Generating Lite Archives..." -ForegroundColor Cyan
if (Test-Path ".\Deploy\DesktopServerSetupLite.exe") {
    Compress-Archive -Path ".\Deploy\DesktopServerSetupLite.exe" -DestinationPath ".\Deploy\DesktopServerSetupLite.zip" -Force
    if (Test-Path $sevenZip) {
        & $sevenZip a -t7z ".\Deploy\DesktopServerSetupLite.7z" ".\Deploy\DesktopServerSetupLite.exe" | Out-Null
    }
}

# Pro Archives
Write-Host "   Generating Pro Archives..." -ForegroundColor Cyan
if (Test-Path ".\Deploy\DesktopServerSetupPro.exe") {
    Compress-Archive -Path ".\Deploy\DesktopServerSetupPro.exe" -DestinationPath ".\Deploy\DesktopServerSetupPro.zip" -Force
    if (Test-Path $sevenZip) {
        & $sevenZip a -t7z ".\Deploy\DesktopServerSetupPro.7z" ".\Deploy\DesktopServerSetupPro.exe" | Out-Null
    }
}

Write-Host "All Done! Final artifacts available in .\Deploy" -ForegroundColor Green
