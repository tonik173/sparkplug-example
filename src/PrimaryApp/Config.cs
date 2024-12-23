namespace PrimaryApp;

public class Config
{
    public Config()
    {
        _ = Environment.GetEnvironmentVariable("MQTT_BROKER_URL") is string brokerUrl && !string.IsNullOrWhiteSpace(brokerUrl)
            ? BrokerUrl = brokerUrl
            : BrokerUrl = "localhost";

        _ = Environment.GetEnvironmentVariable("MQTT_BROKER_PORT") is string brokerPort && !string.IsNullOrWhiteSpace(brokerPort)
            ? BrokerPort = int.Parse(brokerPort)
            : BrokerPort = 1885;

        _ = Environment.GetEnvironmentVariable("MQTT_USER") is string user && !string.IsNullOrWhiteSpace(user)  
            ? User = user
            : User = "spuser";

        _ = Environment.GetEnvironmentVariable("MQTT_PASSWORD") is string password && !string.IsNullOrWhiteSpace(password)
            ? Password = password
            : Password = "1234.abcd";
    }

    public string BrokerUrl { get; set; } = "localhost";
    public int BrokerPort { get; set; } = 1885;
    public string? User { get; set; } = "spuser";
    public string? Password { get; set; } = "1234.abcd";
    public string? HostIdentifierId { get; set; } = "PrimaryDemoAppHostId";
    public string? MqttClientId { get; set; } = "PrimaryDemoAppMqttId";
}