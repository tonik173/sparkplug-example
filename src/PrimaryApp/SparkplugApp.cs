using Domain;
using Microsoft.Extensions.Logging;
using SparkplugNet.Core;
using SparkplugNet.Core.Application;
using SparkplugNet.Core.Enumerations;
using SparkplugNet.VersionB;
using SparkplugNet.VersionB.Data;
using SpCommon;

namespace PrimaryApp;

public delegate void SignalStateReceivedDelegate(SignalState newSignal, string groupId, string nodeId);

public class SparkplugApp(ILogger<SparkplugApp> logger)
{
    private SparkplugApplication _application = null!;
    public event SignalStateReceivedDelegate? OnSignalStateReceivedDelegate;

    public async Task StartAsync(Config config)
    {
        logger.LogInformation("Starting SparkplugApp");
        await Initialize(config);
    }

    public async Task StopAsync()
    {
        logger.LogInformation("Stopping SparkplugApp");
        await Close();
    }

    private async Task Initialize(Config config)
    {
        SparkplugApplicationOptions applicationOptions = new(
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
        true,
            CancellationToken.None);

        IEnumerable<Metric> metrics = CreateAnnounceMetrics();
        _application = new(metrics, SparkplugSpecificationVersion.Version30);

        // Handles the application's connected and disconnected events.
        _application.Connected += OnApplicationConnected;
        _application.Disconnected += OnApplicationDisconnected;

        // Handles the application's device related events.
        _application.DeviceBirthReceived += OnApplicationDeviceBirthReceived;
        _application.DeviceDataReceived += OnApplicationDeviceDataReceived;
        _application.DeviceDeathReceived += OnApplicationDeviceDeathReceived;

        // Handles the application's node related events.
        _application.NodeBirthReceived += OnApplicationNodeBirthReceived;
        _application.NodeDataReceived += OnApplicationNodeDataReceived;
        _application.NodeDeathReceived += OnApplicationNodeDeathReceived;

        await _application.Start(applicationOptions);
        logger.LogInformation("SparkplugApp has been started...");
    }

    public async Task Close()
    {
        logger.LogDebug("SparkplugApp is about to close");
        await _application.Stop();
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

    public async Task PublishNodeCommand(IEnumerable<Metric> metrics, string groupId, string nodeId)
    {
        await _application.PublishNodeCommand(metrics, groupId, nodeId);
    }

    public async Task PublishDeviceCommand(IEnumerable<Metric> metrics, string groupId, string nodeId, string deviceId)
    {
        await _application.PublishDeviceCommand(metrics, groupId, nodeId, deviceId);
    }

    #region Event handlers

    private Task OnApplicationConnected(SparkplugBase<Metric>.SparkplugEventArgs args)
    {
        logger.LogDebug($"SP.OnApplicationConnected");
        return Task.CompletedTask;
    }

    private Task OnApplicationDisconnected(SparkplugApplication.SparkplugEventArgs args)
    {
        logger.LogDebug($"SparkplugApp has been disconnected");
        return Task.CompletedTask;
    }

    private Task OnApplicationDeviceBirthReceived(SparkplugBase<Metric>.DeviceBirthEventArgs args)
    {
        logger.LogDebug($"SP.OnApplicationDeviceBirthReceived");
        return Task.CompletedTask;
    }

    private Task OnApplicationDeviceDataReceived(SparkplugApplication.DeviceDataEventArgs args)
    {
        logger.LogDebug($"SP.OnApplicationDeviceDataReceived");
        return Task.CompletedTask;
    }

    private Task OnApplicationDeviceDeathReceived(SparkplugBase<Metric>.DeviceEventArgs args)
    {
        logger.LogDebug($"SP.OnApplicationDeviceDeathReceived");
        return Task.CompletedTask;
    }

    private Task OnApplicationNodeBirthReceived(SparkplugBase<Metric>.NodeBirthEventArgs args)
    {
        logger.LogDebug($"SP.OnApplicationNodeBirthReceived");
        return Task.CompletedTask;
    }

    private Task OnApplicationNodeDataReceived(SparkplugApplication.NodeDataEventArgs args)
    {
        logger.LogDebug($"SP.OnApplicationNodeDataReceived");

        foreach (var metric in args.Metrics)
        {
            logger.LogDebug(metric.ToString());
        }
        if (OnSignalStateReceivedDelegate != null)
        {
            SignalState signalState = NodeMetricsHelpers.From(args.Metrics);
            OnSignalStateReceivedDelegate(signalState, args.GroupIdentifier, args.EdgeNodeIdentifier);
        }
        return Task.CompletedTask;
    }

    private Task OnApplicationNodeDeathReceived(SparkplugBase<Metric>.NodeDeathEventArgs args)
    {
        logger.LogDebug($"SP.OnApplicationNodeDeathReceived");
        return Task.CompletedTask;
    }

    #endregion
}