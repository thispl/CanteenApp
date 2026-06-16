namespace CanteenManagementSystem.CanteenManagement.Dtos;

public class SystemConfigDto
{
    public string ApiUrl { get; set; } = "http://localhost:5002";
    public string BlazorUrl { get; set; } = "http://localhost:5003";
    public string DefaultConnectionString { get; set; } = string.Empty;
    public string ZkTecoConnectionString { get; set; } = string.Empty;
    public string CorsOrigins { get; set; } = string.Empty;
}

public class SystemConfigResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool RestartRequired { get; set; }
}
