namespace PrimaryApp;

public class Config
{
    public string BrokerUrl { get; set; } = "localhost";
    public int BrokerPort { get; set; } = 1883;
    public string? User { get; set; }
    public string? Password { get; set; } 
    public string? HostIdentifierId { get; set; } = "PrimaryDemoAppHostId";
    public string? MqttClientId { get; set; } = "PrimaryDemoAppMqttId";
}