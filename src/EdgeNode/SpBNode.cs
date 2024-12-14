using Microsoft.Extensions.Logging;
using MQTTnet.Client;
using SparkplugNet.Core;
using SparkplugNet.Core.Enumerations;
using SparkplugNet.Core.Node;
using SparkplugNet.VersionB;
using SparkplugNet.VersionB.Data;
using Domain;
using SpCommon;

namespace EdgeNode;

public delegate void SignalCommandReceivedDelegate(SignalCommand newCommand);

public class SpBNode(ILogger<SpBNode> logger)
{
    private SparkplugNode _node = null!;
    public string GroupId { get; private set; } = "aGroup";
    public string NodeId { get; private set; } = "aNode";

    public event SignalCommandReceivedDelegate? SignalCommandReceived;

    public async Task StartAsync(Config config)
    {
        logger.LogInformation("Starting SpBNode");
        await Initialize(config);
    }

    public async Task StopAsync()
    {
        logger.LogInformation("Stopping SpBNode");
        await Close();
    }

    internal async Task Initialize(Config config)
    {
        var nodeOptions = new SparkplugNodeOptions(
            config.BrokerUrl,
            config.BrokerPort,
            config.MqttClientId,
            config.User,
            config.Password,
            config.HostIdentifierId,
            TimeSpan.FromSeconds(30),
            SparkplugMqttProtocolVersion.V311,
            null,
            null,
            config.GroupId,
            config.NodeId,
            CancellationToken.None);

        GroupId = config.GroupId;
        NodeId = config.NodeId;

        IEnumerable<Metric> metrics = CreateAnnounceMetrics();
        _node = new(metrics, SparkplugSpecificationVersion.Version30);

        _node.Connected += OnNodeConnected;
        _node.Disconnected += OnNodeDisconnected;

        // Handles device events
        _node.DeviceBirthPublishing += OnNodeDeviceBirthPublishing;
        _node.DeviceCommandReceived += OnNodeDeviceCommandReceived;
        _node.DeviceDeathPublishing += OnNodeDeviceDeathPublishing;

        // Handles node events
        _node.NodeCommandReceived += OnNodeNodeCommandReceived;
        _node.StatusMessageReceived += OnNodeStatusMessageReceived;

        await _node.Start(nodeOptions);
        logger.LogInformation("Sparkplug node has been started...");
    }

    public async Task Close()
    {
        await _node.Stop();
    }

    public async Task Publish(IEnumerable<Metric> metrics)
    {
        if (_node.IsConnected)
        {
            MqttClientPublishResult mqttResult = await _node.PublishMetrics(metrics);
            if (!mqttResult.IsSuccess)
                logger.LogError(mqttResult.ReasonString);
        }
    }

    private IEnumerable<Metric> CreateAnnounceMetrics()
    {
        IEnumerable<Metric> metrics = new List<Metric>();

        Metric signalCommandTemplateMetric = AppMetricsHelpers.CreateSignalCommandTemplate();
        Metric signalStateTemplateMetric = NodeMetricsHelpers.CreateSignalStateTemplate();

        metrics = metrics.Append(signalCommandTemplateMetric)
                         .Append(signalStateTemplateMetric);

        return metrics;
    }

    #region Events

    private Task OnNodeConnected(SparkplugBase<Metric>.SparkplugEventArgs args)
    {
        logger.LogDebug($"SP.OnNodeConnected");
        return Task.CompletedTask;
    }

    private Task OnNodeDisconnected(SparkplugNode.SparkplugEventArgs args)
    {
        logger.LogDebug($"SP.OnNodeDisconnected");
        return Task.CompletedTask;
    }

    private Task OnNodeDeviceBirthPublishing(SparkplugNode.DeviceBirthEventArgs args)
    {
        logger.LogDebug($"SP.OnNodeDeviceBirthPublishing");
        return Task.CompletedTask;
    }

    private Task OnNodeDeviceCommandReceived(SparkplugNode.DeviceCommandEventArgs args)
    {
        logger.LogDebug($"SP.DeviceCommandEventArgs");
        return Task.CompletedTask;
    }

    private Task OnNodeDeviceDeathPublishing(SparkplugNode.DeviceEventArgs args)
    {
        logger.LogDebug($"SP.OnNodeDeviceDeathPublishing");
        return Task.CompletedTask;
    }

    private Task OnNodeNodeCommandReceived(SparkplugNode.NodeCommandEventArgs args)
    {
        logger.LogDebug($"SP.NodeCommandEventArgs");
        if (SignalCommandReceived  != null)
        {
            SignalCommand command = AppMetricsHelpers.From(args.Metrics);
            SignalCommandReceived(command);
        }

        return Task.CompletedTask;
    }
    private Task OnNodeStatusMessageReceived(SparkplugNode.StatusMessageEventArgs args)
    {
        logger.LogDebug($"SP.OnNodeStatusMessageReceived");
        return Task.CompletedTask;
    }

    #endregion
}

