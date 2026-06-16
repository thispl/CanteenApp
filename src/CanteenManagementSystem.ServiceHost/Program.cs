using CanteenManagementSystem.ServiceHost;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/service-host.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Starting Canteen Management Service Host");
    
    var builder = Host.CreateApplicationBuilder(args);
    
    builder.Services.AddWindowsService(options =>
    {
        options.ServiceName = "CanteenManagementService";
    });
    
    builder.Services.AddHostedService<CanteenServiceWorker>();
    builder.Services.AddHttpClient();
    
    var host = builder.Build();
    host.Run();
    
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Service host terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}
