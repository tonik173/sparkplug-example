namespace EdgeNode;

public class Config
{
    public Config(string nodeId)
    {
        NodeId = nodeId;

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

    public string BrokerUrl { get; set; }
    public int BrokerPort { get; set; }
    public string? User { get; set; }
    public string? Password { get; set; }
    public string? HostIdentifierId { get; set; } = "PrimaryDemoAppHostId";
    public string? MqttClientId { get; set; } = Guid.NewGuid().ToString();
    public string GroupId { get; set; } = "DemoGroup";
    public string NodeId { get; set; }
    public string? DeviceId { get; set; } = null;
}