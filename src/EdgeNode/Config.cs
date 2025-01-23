namespace EdgeNode;

public class Config
{
    public Config(string nodeId, string deviceId = null)
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

        _ = Environment.GetEnvironmentVariable("MQTT_CLIENT_ID") is string mqttClientId && !string.IsNullOrWhiteSpace(mqttClientId)
            ? MqttClientId = "node." + mqttClientId
            : MqttClientId = "node." + Guid.NewGuid().ToString();

        _ = Environment.GetEnvironmentVariable("SP_PRIMARY_HOST_ID") is string hostIdentifierId && !string.IsNullOrWhiteSpace(hostIdentifierId)
            ? HostIdentifierId = hostIdentifierId
            : HostIdentifierId = "PrimaryDemoAppHostId";

        _ = Environment.GetEnvironmentVariable("SP_GROUP_ID") is string groupId && !string.IsNullOrWhiteSpace(groupId)
            ? GroupId = groupId
            : GroupId = "DemoGroup";

        NodeId = nodeId;
        DeviceId = deviceId;
    }

    public string BrokerUrl { get; set; }
    public int BrokerPort { get; set; }
    public string? User { get; set; }
    public string? Password { get; set; }
    public string? HostIdentifierId { get; set; }
    public string? MqttClientId { get; set; }
    public string GroupId { get; set; }
    public string NodeId { get; set; }
    public string? DeviceId { get; set; }
}