using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using CanteenManagementSystem.CanteenManagement.Entities;
using CanteenManagementSystem.CanteenManagement.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Guids;
using Volo.Abp.Threading;

namespace CanteenManagementSystem.CanteenManagement.SyncWorker;

public class BioTimeOptions
{
    public bool Enabled { get; set; }
    public string BaseUrl { get; set; } = "http://localhost:8090";
    public string DeviceIpAddress { get; set; } = "192.168.29.195";
    public string? TerminalAlias { get; set; }
    public string? Token { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public int PollIntervalMilliseconds { get; set; } = 5000;
}

public class BioTimeSyncWorker : AsyncPeriodicBackgroundWorkerBase
{
    public const string SyncStateKey = "Last_BioTime_Transaction_Id";

    private readonly BioTimeOptions _options;
    private readonly HttpClient _client;
    private string? _token;
    private string? _terminalSerialNumber;

    public BioTimeSyncWorker(
        AbpAsyncTimer timer,
        IServiceScopeFactory serviceScopeFactory,
        IOptions<BioTimeOptions> options) : base(timer, serviceScopeFactory)
    {
        _options = options.Value;
        _client = new HttpClient
        {
            BaseAddress = new Uri(_options.BaseUrl.TrimEnd('/') + "/"),
            Timeout = TimeSpan.FromSeconds(15)
        };
        Timer.Period = Math.Max(_options.PollIntervalMilliseconds, 1000);
    }

    protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
    {
        if (!_options.Enabled)
        {
            return;
        }

        var logger = workerContext.ServiceProvider.GetRequiredService<ILogger<BioTimeSyncWorker>>();
        if (string.IsNullOrWhiteSpace(_options.Token) &&
            (string.IsNullOrWhiteSpace(_options.Username) || string.IsNullOrWhiteSpace(_options.Password)))
        {
            logger.LogWarning("BioTime sync is enabled but no token or service-account credentials are configured.");
            return;
        }

        try
        {
            var cancellationToken = workerContext.CancellationToken;
            await AuthenticateAsync(_client, cancellationToken);
            _terminalSerialNumber ??= await ResolveTerminalSerialNumberAsync(_client, cancellationToken);

            var syncStateRepository = workerContext.ServiceProvider.GetRequiredService<ISyncStateRepository>();
            var syncState = await syncStateRepository.GetOrCreateAsync(SyncStateKey, 0, cancellationToken);
            var transactions = await GetTransactionsAsync(_client, _terminalSerialNumber, cancellationToken);
            var newTransactions = transactions
                .Where(t => t.Id > syncState.LastProcessedValue)
                .OrderBy(t => t.Id)
                .ToList();

            if (newTransactions.Count == 0)
            {
                return;
            }

            var employeeRepository = workerContext.ServiceProvider.GetRequiredService<IEmployeeRepository>();
            var checkInRepository = workerContext.ServiceProvider.GetRequiredService<ICanteenCheckInRepository>();
            var guidGenerator = workerContext.ServiceProvider.GetRequiredService<IGuidGenerator>();
            var maxProcessedId = syncState.LastProcessedValue;
            var createdCount = 0;

            foreach (var transaction in newTransactions)
            {
                if (string.IsNullOrWhiteSpace(transaction.EmployeeCode) || !TryParsePunchTime(transaction.PunchTime, out var punchTime))
                {
                    maxProcessedId = Math.Max(maxProcessedId, transaction.Id);
                    continue;
                }

                var employee = await employeeRepository.FindByEmployeeIdAsync(transaction.EmployeeCode, cancellationToken);
                if (employee == null)
                {
                    employee = new Employee(guidGenerator.Create(), transaction.EmployeeCode, $"Unknown ({transaction.EmployeeCode})");
                    await employeeRepository.InsertAsync(employee, autoSave: true, cancellationToken);
                }

                var deviceId = transaction.TerminalAlias ?? transaction.TerminalSerialNumber ?? _options.DeviceIpAddress;
                if (!await checkInRepository.ExistsAsync(transaction.EmployeeCode, deviceId, punchTime, cancellationToken))
                {
                    var checkIn = new CanteenCheckIn(
                        guidGenerator.Create(),
                        transaction.EmployeeCode,
                        deviceId,
                        punchTime);
                    await checkInRepository.InsertAsync(checkIn, autoSave: false, cancellationToken);
                    createdCount++;
                }

                maxProcessedId = Math.Max(maxProcessedId, transaction.Id);
            }

            if (maxProcessedId > syncState.LastProcessedValue)
            {
                syncState.UpdateLastProcessedValue(maxProcessedId);
                await syncStateRepository.UpdateAsync(syncState, autoSave: true, cancellationToken);
                logger.LogInformation(
                    "BioTime sync imported {CreatedCount} punches from device {DeviceIp}; last transaction ID is {TransactionId}.",
                    createdCount,
                    _options.DeviceIpAddress,
                    maxProcessedId);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "BioTime punch synchronization failed.");
        }
    }

    private async Task AuthenticateAsync(HttpClient client, CancellationToken cancellationToken)
    {
        _token ??= _options.Token;
        if (string.IsNullOrWhiteSpace(_token))
        {
            using var response = await client.PostAsJsonAsync(
                "api-token-auth/",
                new { username = _options.Username, password = _options.Password },
                cancellationToken);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<BioTimeTokenResponse>(cancellationToken: cancellationToken);
            _token = result?.Token ?? throw new InvalidOperationException("BioTime did not return an authentication token.");
        }

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", _token);
    }

    private async Task<string> ResolveTerminalSerialNumberAsync(HttpClient client, CancellationToken cancellationToken)
    {
        var path = $"iclock/api/terminals/?limit=100&ip_address={Uri.EscapeDataString(_options.DeviceIpAddress)}";
        using var response = await client.GetAsync(path, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<BioTimePage<BioTimeTerminal>>(cancellationToken: cancellationToken);
        var terminal = result?.Data.FirstOrDefault(t =>
            string.Equals(t.IpAddress, _options.DeviceIpAddress, StringComparison.OrdinalIgnoreCase));
        return terminal?.SerialNumber ?? throw new InvalidOperationException(
            $"BioTime has no terminal registered with IP address {_options.DeviceIpAddress}.");
    }

    private async Task<List<BioTimeTransaction>> GetTransactionsAsync(
        HttpClient client,
        string terminalSerialNumber,
        CancellationToken cancellationToken)
    {
        var path = $"iclock/api/transactions/?page=1&limit=100&ordering=-id&terminal_sn={Uri.EscapeDataString(terminalSerialNumber)}";
        using var response = await client.GetAsync(path, cancellationToken);
        if (response.StatusCode == HttpStatusCode.Unauthorized && string.IsNullOrWhiteSpace(_options.Token))
        {
            _token = null;
            await AuthenticateAsync(client, cancellationToken);
            using var retryResponse = await client.GetAsync(path, cancellationToken);
            retryResponse.EnsureSuccessStatusCode();
            var retryResult = await retryResponse.Content.ReadFromJsonAsync<BioTimePage<BioTimeTransaction>>(cancellationToken: cancellationToken);
            return retryResult?.Data ?? new List<BioTimeTransaction>();
        }

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<BioTimePage<BioTimeTransaction>>(cancellationToken: cancellationToken);
        return result?.Data ?? new List<BioTimeTransaction>();
    }

    private static bool TryParsePunchTime(string? value, out DateTime punchTime)
    {
        return DateTime.TryParseExact(
                   value,
                   new[] { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm:ss.ffffff" },
                   CultureInfo.InvariantCulture,
                   DateTimeStyles.AssumeLocal,
                   out punchTime) ||
               DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out punchTime);
    }

    private sealed class BioTimeTokenResponse
    {
        [JsonPropertyName("token")]
        public string? Token { get; set; }
    }

    private sealed class BioTimePage<T>
    {
        [JsonPropertyName("data")]
        public List<T> Data { get; set; } = new();
    }

    private sealed class BioTimeTerminal
    {
        [JsonPropertyName("sn")]
        public string? SerialNumber { get; set; }

        [JsonPropertyName("ip_address")]
        public string? IpAddress { get; set; }
    }

    private sealed class BioTimeTransaction
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("emp_code")]
        public string? EmployeeCode { get; set; }

        [JsonPropertyName("punch_time")]
        public string? PunchTime { get; set; }

        [JsonPropertyName("terminal_sn")]
        public string? TerminalSerialNumber { get; set; }

        [JsonPropertyName("terminal_alias")]
        public string? TerminalAlias { get; set; }
    }
}
