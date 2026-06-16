# CMS - Canteen Management System

A Blazor WebAssembly application for managing canteen check-ins, employee directories, and branding. Built with ABP Framework 10.4.1, .NET 10, and SQL Server.

## Features

- **Employee Directory** - Manage employees with ID, name, department
- **Live Check-ins** - Real-time canteen entry logs
- **ZkTeco Integration** - Biometric device sync for check-ins
- **Branding Settings** - Customize logo, colors, app name
- **System Configuration** - Configure database connections and server URLs from the UI
- **Windows Service Deployment** - Runs as background Windows Service

## Default Login

- **Username:** `admin`
- **Password:** `admin`

## Default URLs

- **Blazor UI:** http://localhost:5003
- **API Backend:** http://localhost:5002

## Quick Start (Development)

```bash
# 1. Restore dependencies
dotnet restore

# 2. Update database (uses DbMigrator)
dotnet run --project src/CanteenManagementSystem.DbMigrator

# 3. Start API
dotnet run --project src/CanteenManagementSystem.HttpApi.Host

# 4. Start Blazor (in new terminal)
dotnet run --project src/CanteenManagementSystem.Blazor
```

## Production Deployment (Windows Service)

### Prerequisites

- Windows Server or Windows 10/11
- SQL Server (any edition)
- .NET 10 Runtime

### Deploy Steps

1. **Publish applications:**
```powershell
dotnet publish src/CanteenManagementSystem.HttpApi.Host -c Release -o C:\CanteenServices\Api
dotnet publish src/CanteenManagementSystem.Blazor -c Release -o C:\CanteenServices\Blazor
dotnet publish src/CanteenManagementSystem.ServiceHost -c Release -o C:\CanteenServices\ServiceHost
```

2. **Run database migrations:**
```bash
cd C:\CanteenServices\Api
dotnet CanteenManagementSystem.DbMigrator.dll
```

3. **Install Windows Service:**
```powershell
sc create CanteenManagementService binPath= "C:\CanteenServices\ServiceHost\CanteenManagementSystem.ServiceHost.exe" start= auto
sc start CanteenManagementService
```

4. **Access the app:**
Open http://localhost:5003 in your browser

### Configuration (via Web UI)

After deployment, login as admin and go to **Administration → System Configuration** to:
- Change database connection strings
- Test database connectivity
- Update API/Blazor URLs
- Configure CORS origins

## Project Structure

| Project | Purpose |
|---------|---------|
| `HttpApi.Host` | ASP.NET Core API server |
| `Blazor` | Blazor WebAssembly host |
| `Blazor.Client` | Client-side UI components |
| `ServiceHost` | Windows Service wrapper |
| `DbMigrator` | Database migrations & seeding |
| `Application` | Business logic & services |
| `Domain` | Entities & domain logic |

## Environment-Specific Settings

### Development
- Edit `appsettings.json` in each project
- Uses `localhost` with Windows Auth for SQL

### Production
- Deploy to target machine
- Use **System Configuration** page to set:
  - SQL Server connection (IP/hostname, credentials)
  - Service URLs (if changing ports)
- Restart Windows Service after changes

## Architecture

- **Framework:** ABP Framework 10.4.1
- **UI:** Blazor WebAssembly + Blazorise + Bootstrap 5
- **Theme:** LeptonX Lite
- **Backend:** ASP.NET Core with OpenIddict auth
- **Database:** SQL Server with EF Core
- **Deployment:** Windows Service (.NET Generic Host)

## License

Built on ABP Framework - see [ABP Commercial License](https://abp.io/pricing) or [LGPL](https://github.com/abpframework/abp/blob/dev/LICENSE) for open source usage.
