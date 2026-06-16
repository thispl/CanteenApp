# Canteen Management System - Service Installation Script
# Run as Administrator

param(
    [string]$InstallPath = "d:\CanteenServices",
    [string]$ServiceName = "CanteenManagementService"
)

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Canteen Management Service Installer" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if running as Administrator
if (-not ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    Write-Error "Please run this script as Administrator!"
    exit 1
}

# Stop and remove existing service if exists
Write-Host "Checking for existing service..." -ForegroundColor Yellow
$existingService = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
if ($existingService) {
    Write-Host "Stopping existing service..." -ForegroundColor Yellow
    Stop-Service -Name $ServiceName -Force -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 2
    
    Write-Host "Removing existing service..." -ForegroundColor Yellow
    sc delete $ServiceName | Out-Null
    Start-Sleep -Seconds 2
}

# Also stop legacy services if they exist
$legacyServices = @("CanteenAPI", "CanteenUI")
foreach ($svc in $legacyServices) {
    $legacy = Get-Service -Name $svc -ErrorAction SilentlyContinue
    if ($legacy) {
        Write-Host "Stopping legacy service: $svc" -ForegroundColor Yellow
        Stop-Service -Name $svc -Force -ErrorAction SilentlyContinue
        sc delete $svc | Out-Null
    }
}

# Verify installation path exists
$serviceHostPath = Join-Path $InstallPath "ServiceHost\CanteenManagementSystem.ServiceHost.exe"
if (-not (Test-Path $serviceHostPath)) {
    Write-Error "Service Host not found at: $serviceHostPath"
    Write-Host "Please publish the ServiceHost project first!" -ForegroundColor Red
    exit 1
}

Write-Host "Service Host found at: $serviceHostPath" -ForegroundColor Green

# Create service
Write-Host "Creating Windows Service: $ServiceName..." -ForegroundColor Green
sc create $ServiceName binPath= "$serviceHostPath" start= auto obj= "NT AUTHORITY\LocalService" | Out-Null

if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to create service!"
    exit 1
}

# Configure service recovery
Write-Host "Configuring service recovery..." -ForegroundColor Green
sc failure $ServiceName reset= 86400 actions= restart/5000/restart/5000/restart/5000 | Out-Null

# Start service
Write-Host "Starting service..." -ForegroundColor Green
sc start $ServiceName | Out-Null

Start-Sleep -Seconds 5

# Verify service status
$service = Get-Service -Name $ServiceName
if ($service.Status -eq "Running") {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "Service installed and started successfully!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Service Name: $ServiceName" -ForegroundColor White
    Write-Host "Install Path: $InstallPath" -ForegroundColor White
    Write-Host "API URL: http://localhost:5002" -ForegroundColor White
    Write-Host "Blazor URL: http://localhost:5003" -ForegroundColor White
    Write-Host ""
    Write-Host "Useful commands:" -ForegroundColor Cyan
    Write-Host "  Stop Service:  sc stop $ServiceName" -ForegroundColor Gray
    Write-Host "  Start Service: sc start $ServiceName" -ForegroundColor Gray
    Write-Host "  View Logs:     Get-Content '$InstallPath\ServiceHost\logs\service-host.log' -Tail 50" -ForegroundColor Gray
} else {
    Write-Warning "Service created but may not be running. Status: $($service.Status)"
}

Write-Host ""
Write-Host "Installation complete!" -ForegroundColor Green
