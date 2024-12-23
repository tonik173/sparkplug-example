namespace PrimaryApp;

public class Config
{
    public string BrokerUrl { get; set; } = "localhost";
    public int BrokerPort { get; set; } = 1885;
    public string? User { get; set; } = null;
    public string? Password { get; set; } = null;
    public string? HostIdentifierId { get; set; } = "PrimaryDemoAppHostId";
    public string? MqttClientId { get; set; } = "PrimaryDemoAppMqttId";
}