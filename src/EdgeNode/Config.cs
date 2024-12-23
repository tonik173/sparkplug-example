namespace EdgeNode;

public class Config
{
    public Config(string nodeId)
    {
        NodeId = nodeId;
    }

    public string BrokerUrl { get; set; } = "localhost";
    public int BrokerPort { get; set; } = 1885;
    public string? User { get; set; } = null;
    public string? Password { get; set; } = null;
    public string? HostIdentifierId { get; set; } = "PrimaryDemoAppHostId";
    public string? MqttClientId { get; set; } = Guid.NewGuid().ToString();
    public string GroupId { get; set; } = "DemoGroup";
    public string NodeId { get; set; }
    public string? DeviceId { get; set; } = null;
}