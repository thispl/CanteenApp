using System.Diagnostics;
using System.Text.Json;

namespace CanteenManagementSystem.ServiceHost;

public class CanteenServiceWorker : BackgroundService
{
    private readonly ILogger<CanteenServiceWorker> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _installPath;
    private Process? _apiProcess;
    private Process? _blazorProcess;
    private bool _isStopping = false;

    public CanteenServiceWorker(
        ILogger<CanteenServiceWorker> logger,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _installPath = Path.Combine(AppContext.BaseDirectory, "..");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Canteen Service Worker starting...");
        _logger.LogInformation("Installation path: {Path}", _installPath);

        try
        {
            // Step 1: Start API
            _logger.LogInformation("Starting API...");
            await StartApiAsync(stoppingToken);

            // Step 2: Wait for API to be healthy
            _logger.LogInformation("Waiting for API to be ready...");
            await WaitForApiHealthyAsync(stoppingToken);

            // Step 3: Start Blazor UI
            _logger.LogInformation("Starting Blazor UI...");
            await StartBlazorAsync(stoppingToken);

            _logger.LogInformation("All services started successfully!");

            // Step 4: Monitor processes
            await MonitorProcessesAsync(stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Service shutdown requested");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in service worker");
        }
        finally
        {
            await StopProcessesAsync();
        }
    }

    private async Task StartApiAsync(CancellationToken cancellationToken)
    {
        var apiPath = Path.Combine(_installPath, "Api", "CanteenManagementSystem.HttpApi.Host.exe");
        
        if (!File.Exists(apiPath))
        {
            throw new FileNotFoundException($"API executable not found at: {apiPath}");
        }

        _apiProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = apiPath,
                WorkingDirectory = Path.Combine(_installPath, "Api"),
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            },
            EnableRaisingEvents = true
        };

        _apiProcess.OutputDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                _logger.LogInformation("[API] {Message}", e.Data);
        };

        _apiProcess.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                _logger.LogError("[API] {Message}", e.Data);
        };

        _apiProcess.Exited += (sender, e) =>
        {
            _logger.LogWarning("API process exited with code: {ExitCode}", _apiProcess.ExitCode);
            if (!_isStopping)
            {
                _logger.LogInformation("Attempting to restart API...");
                _ = Task.Run(async () =>
                {
                    await Task.Delay(5000);
                    if (!_isStopping)
                    {
                        await StartApiAsync(CancellationToken.None);
                    }
                });
            }
        };

        _apiProcess.Start();
        _apiProcess.BeginOutputReadLine();
        _apiProcess.BeginErrorReadLine();

        _logger.LogInformation("API process started (PID: {PID})", _apiProcess.Id);
    }

    private async Task WaitForApiHealthyAsync(CancellationToken cancellationToken)
    {
        using var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(5);

        var maxRetries = 60; // 60 attempts, 2 seconds each = 2 minutes max
        var delay = TimeSpan.FromSeconds(2);

        for (int i = 0; i < maxRetries; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var response = await client.GetAsync("http://localhost:5002/health-status", cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("API is healthy and ready!");
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug("API not ready yet (attempt {Attempt}/{Max}): {Message}", 
                    i + 1, maxRetries, ex.Message);
            }

            await Task.Delay(delay, cancellationToken);
        }

        throw new TimeoutException("API did not become healthy within the expected time");
    }

    private async Task StartBlazorAsync(CancellationToken cancellationToken)
    {
        var blazorPath = Path.Combine(_installPath, "Blazor", "CanteenManagementSystem.Blazor.exe");
        
        if (!File.Exists(blazorPath))
        {
            throw new FileNotFoundException($"Blazor executable not found at: {blazorPath}");
        }

        _blazorProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = blazorPath,
                WorkingDirectory = Path.Combine(_installPath, "Blazor"),
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            },
            EnableRaisingEvents = true
        };

        _blazorProcess.OutputDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                _logger.LogInformation("[Blazor] {Message}", e.Data);
        };

        _blazorProcess.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                _logger.LogError("[Blazor] {Message}", e.Data);
        };

        _blazorProcess.Exited += (sender, e) =>
        {
            _logger.LogWarning("Blazor process exited with code: {ExitCode}", _blazorProcess.ExitCode);
            if (!_isStopping)
            {
                _logger.LogInformation("Attempting to restart Blazor...");
                _ = Task.Run(async () =>
                {
                    await Task.Delay(5000);
                    if (!_isStopping)
                    {
                        await StartBlazorAsync(CancellationToken.None);
                    }
                });
            }
        };

        _blazorProcess.Start();
        _blazorProcess.BeginOutputReadLine();
        _blazorProcess.BeginErrorReadLine();

        _logger.LogInformation("Blazor process started (PID: {PID})", _blazorProcess.Id);
    }

    private async Task MonitorProcessesAsync(CancellationToken cancellationToken)
    {
        // Keep the service running and periodically check process health
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                // Check API health every 30 seconds
                using var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(5);
                
                try
                {
                    var response = await client.GetAsync("http://localhost:5002/health-status", cancellationToken);
                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogWarning("API health check returned non-success status: {StatusCode}", 
                            response.StatusCode);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "API health check failed");
                }

                // Log current status
                var apiRunning = _apiProcess?.HasExited == false;
                var blazorRunning = _blazorProcess?.HasExited == false;
                _logger.LogInformation("Service status - API: {ApiStatus}, Blazor: {BlazorStatus}",
                    apiRunning ? "Running" : "Stopped",
                    blazorRunning ? "Running" : "Stopped");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error monitoring processes");
            }

            await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
        }
    }

    private async Task StopProcessesAsync()
    {
        _isStopping = true;
        _logger.LogInformation("Stopping all processes...");

        // Stop Blazor first (depends on API)
        if (_blazorProcess != null && !_blazorProcess.HasExited)
        {
            try
            {
                _logger.LogInformation("Stopping Blazor process...");
                _blazorProcess.Kill(entireProcessTree: true);
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                await _blazorProcess.WaitForExitAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Timeout waiting for Blazor process to exit");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping Blazor process");
            }
        }

        // Stop API
        if (_apiProcess != null && !_apiProcess.HasExited)
        {
            try
            {
                _logger.LogInformation("Stopping API process...");
                _apiProcess.Kill(entireProcessTree: true);
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                await _apiProcess.WaitForExitAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Timeout waiting for API process to exit");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping API process");
            }
        }

        _logger.LogInformation("All processes stopped");
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stop requested, shutting down services...");
        await StopProcessesAsync();
        await base.StopAsync(cancellationToken);
    }
}
