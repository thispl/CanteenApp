# Canteen Management System - Full Publish Script
# Publishes API, Blazor, and ServiceHost to d:\CanteenServices

param(
    [string]$OutputPath = "d:\CanteenServices",
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"
$ProjectRoot = $PSScriptRoot

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Canteen Management System - Full Publish" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Configuration: $Configuration" -ForegroundColor White
Write-Host "Output Path: $OutputPath" -ForegroundColor White
Write-Host ""

# Create output directories
$directories = @(
    "$OutputPath\Api",
    "$OutputPath\Blazor",
    "$OutputPath\ServiceHost"
)

foreach ($dir in $directories) {
    if (Test-Path $dir) {
        Write-Host "Cleaning $dir..." -ForegroundColor Yellow
        Remove-Item -Path "$dir\*" -Recurse -Force -ErrorAction SilentlyContinue
    } else {
        Write-Host "Creating $dir..." -ForegroundColor Green
        New-Item -ItemType Directory -Path $dir -Force | Out-Null
    }
}

# 1. Publish API
Write-Host ""
Write-Host "[1/3] Publishing API..." -ForegroundColor Cyan
$apiProject = Join-Path $ProjectRoot "src\CanteenManagementSystem.HttpApi.Host\CanteenManagementSystem.HttpApi.Host.csproj"
dotnet publish $apiProject -c $Configuration -o "$OutputPath\Api" --no-self-contained
if ($LASTEXITCODE -ne 0) { throw "API publish failed!" }
Write-Host "API published successfully!" -ForegroundColor Green

# 2. Publish Blazor
Write-Host ""
Write-Host "[2/3] Publishing Blazor..." -ForegroundColor Cyan
$blazorProject = Join-Path $ProjectRoot "src\CanteenManagementSystem.Blazor\CanteenManagementSystem.Blazor.csproj"
dotnet publish $blazorProject -c $Configuration -o "$OutputPath\Blazor" --no-self-contained
if ($LASTEXITCODE -ne 0) { throw "Blazor publish failed!" }
Write-Host "Blazor published successfully!" -ForegroundColor Green

# 3. Publish ServiceHost
Write-Host ""
Write-Host "[3/3] Publishing ServiceHost..." -ForegroundColor Cyan
$serviceHostProject = Join-Path $ProjectRoot "src\CanteenManagementSystem.ServiceHost\CanteenManagementSystem.ServiceHost.csproj"
dotnet publish $serviceHostProject -c $Configuration -o "$OutputPath\ServiceHost" --no-self-contained -p:PublishSingleFile=true
if ($LASTEXITCODE -ne 0) { throw "ServiceHost publish failed!" }
Write-Host "ServiceHost published successfully!" -ForegroundColor Green

# Copy install script
Write-Host ""
Write-Host "Copying install script..." -ForegroundColor Cyan
$installScript = Join-Path $ProjectRoot "src\CanteenManagementSystem.ServiceHost\install-service.ps1"
Copy-Item $installScript -Destination $OutputPath -Force
Write-Host "Install script copied!" -ForegroundColor Green

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "Publish Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Published to: $OutputPath" -ForegroundColor White
Write-Host ""
Write-Host "To install as Windows Service, run as Administrator:" -ForegroundColor Cyan
Write-Host "  cd $OutputPath" -ForegroundColor Yellow
Write-Host "  .\install-service.ps1" -ForegroundColor Yellow
Write-Host ""
Write-Host "Or manually:" -ForegroundColor Cyan
Write-Host "  sc create CanteenManagementService binPath= '$OutputPath\ServiceHost\CanteenManagementSystem.ServiceHost.exe' start= auto" -ForegroundColor Gray
Write-Host "  sc start CanteenManagementService" -ForegroundColor Gray
Write-Host ""
