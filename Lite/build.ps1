$ErrorActionPreference = "Stop"

Write-Host "1. Building DesktopServer Manager (Single File)..." -ForegroundColor Cyan
dotnet publish ".\DesktopServerManager\DesktopServerManager.csproj" -c Release -r win-x64 -p:PublishSingleFile=true --self-contained true

Write-Host "2. Updating Resources..." -ForegroundColor Cyan
$sourceExe = ".\DesktopServerManager\bin\Release\net9.0-windows\win-x64\publish\DesktopServerManager.exe"
$destExe = ".\DesktopServerSetup\Resources\DesktopServerManager.exe"

if (Test-Path $sourceExe) {
    Copy-Item $sourceExe $destExe -Force
    Write-Host "   Copied DesktopServerManager.exe (SingleFile) to Resources." -ForegroundColor Green
} else {
    Write-Error "   DesktopServerManager.exe build failed or not found at $sourceExe"
}

Write-Host "3. Building Setup Project..." -ForegroundColor Cyan
dotnet build ".\DesktopServerSetup\DesktopServerSetupLite.csproj" -c Release

Write-Host "4. Publishing Installer..." -ForegroundColor Cyan
$publishDir = ".\Publish"
if (Test-Path $publishDir) { Remove-Item $publishDir -Recurse -Force }
dotnet publish ".\DesktopServerSetup\DesktopServerSetupLite.csproj" -c Release -r win-x64 -p:PublishSingleFile=true --self-contained true -o $publishDir

Write-Host "Build & Publish Complete!" -ForegroundColor Green
Write-Host "Installer is available at: $publishDir\DesktopServerSetupLite.exe" -ForegroundColor Green
