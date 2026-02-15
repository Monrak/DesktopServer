$ErrorActionPreference = "Stop"

Write-Host "1. Building DesktopServer Pro Manager (Single File)..." -ForegroundColor Cyan
dotnet publish ".\DesktopServerManagerPro\DesktopServerManagerPro.csproj" -c Release -r win-x64 -p:PublishSingleFile=true --self-contained true

Write-Host "2. Updating Resources..." -ForegroundColor Cyan
$sourceExe = ".\DesktopServerManagerPro\bin\Release\net9.0-windows\win-x64\publish\DesktopServerManagerPro.exe"
$destExe = ".\DesktopServerSetupPro\Resources\DesktopServerManagerPro.exe"

if (Test-Path $sourceExe) {
    Copy-Item $sourceExe $destExe -Force
    Write-Host "   Copied DesktopServerManagerPro.exe (SingleFile) to Resources." -ForegroundColor Green
} else {
    Write-Error "   DesktopServerManagerPro.exe build failed or not found at $sourceExe"
}

Write-Host "3. Building Setup Project..." -ForegroundColor Cyan
dotnet build ".\DesktopServerSetupPro\DesktopServerSetupPro.csproj" -c Release

Write-Host "4. Publishing Installer..." -ForegroundColor Cyan
$publishDir = ".\Publish"
if (Test-Path $publishDir) { Remove-Item $publishDir -Recurse -Force }
dotnet publish ".\DesktopServerSetupPro\DesktopServerSetupPro.csproj" -c Release -r win-x64 -p:PublishSingleFile=true --self-contained true -o $publishDir

Write-Host "Build & Publish Complete!" -ForegroundColor Green
Write-Host "Installer is available at: $publishDir\DesktopServerSetupPro.exe" -ForegroundColor Green
