$ErrorActionPreference = "Stop"

Write-Host "1. Building DesktopServer Go Manager (Single File)..." -ForegroundColor Cyan
dotnet publish ".\DesktopServerManagerGo\DesktopServerManagerGo.csproj" -c Release -r win-x64 -p:PublishSingleFile=true --self-contained true

Write-Host "2. Updating Resources..." -ForegroundColor Cyan
$sourceExe = ".\DesktopServerManagerGo\bin\Release\net9.0-windows\win-x64\publish\DesktopServerManagerGo.exe"
$destExe = ".\DesktopServerSetupGo\Resources\DesktopServerManagerGo.exe"

if (Test-Path $sourceExe) {
    Copy-Item $sourceExe $destExe -Force
    Write-Host "   Copied DesktopServerManagerGo.exe (SingleFile) to Resources." -ForegroundColor Green
} else {
    Write-Error "   DesktopServerManagerGo.exe build failed or not found at $sourceExe"
}

Write-Host "3. Building Setup Project..." -ForegroundColor Cyan
dotnet build ".\DesktopServerSetupGo\DesktopServerSetupGo.csproj" -c Release

Write-Host "4. Publishing Installer..." -ForegroundColor Cyan
$publishDir = ".\Publish"
if (Test-Path $publishDir) { Remove-Item $publishDir -Recurse -Force }
dotnet publish ".\DesktopServerSetupGo\DesktopServerSetupGo.csproj" -c Release -r win-x64 -p:PublishSingleFile=true --self-contained true -o $publishDir

Write-Host "Build & Publish Complete!" -ForegroundColor Green
Write-Host "Installer is available at: $publishDir\DesktopServerSetupGo.exe" -ForegroundColor Green
