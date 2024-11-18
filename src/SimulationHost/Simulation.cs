using Domain;
using EdgeNode;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PrimaryApp;
using SparkplugNet.VersionB.Data;
using SpCommon;

namespace SimulationHost;

public class Simulation(ILogger<Simulation> logger, ILoggerFactory loggerFactory) : IHostedService
{
    private SparkplugApp _app;
    private SpBNode _node1;
    private SpBNode _node2;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // Sparkplug App
        _app = new(loggerFactory.CreateLogger<SparkplugApp>());
        _app.OnSignalStateReceivedDelegate += (newSignalState, groupId, nodeId) =>
        {
            logger.LogInformation($"<<<=== App received {groupId}/NDATA/{nodeId}: " + newSignalState);
        };
        await _app.StartAsync(new PrimaryApp.Config());


        // Sparkplug Node 1
        _node1 = new(loggerFactory.CreateLogger<SpBNode>());
        _node1.SignalCommandReceived += async newSignalCommand =>
        {
            logger.LogInformation($"<<<=== {_node1.GroupId}/NCMD/{_node1.NodeId} received: " + newSignalCommand);

            await Publish(_node1, SignalStateType.Green, 10);
            await Task.Delay(2000);
            await Publish(_node1, SignalStateType.Yellow, 2);
        };
        _node1.StartAsync(new EdgeNode.Config("DemoNode1"));


        // Sparkplug Node 2
        _node2 = new(loggerFactory.CreateLogger<SpBNode>());
        _node2.SignalCommandReceived += async newSignalCommand =>
        {
            logger.LogInformation($"<<<=== {_node2.GroupId}/NCMD/{_node2.NodeId} received: " + newSignalCommand);

            await Publish(_node2, SignalStateType.Yellow, 5);
        };
        _node2.StartAsync(new EdgeNode.Config("DemoNode2"));


        // Start simulation
        await Task.Delay(2000);
        await Command(SignalModeType.Operation, 2, UnitType.Minutes, _node1);

        await Task.Delay(2000);
        await Command(SignalModeType.Blinking, 8, UnitType.Minutes, _node2);
    }

    private async Task Command(SignalModeType mode, int cyclePeriod, UnitType unit, SpBNode node)
    {
        SignalCommand command = new()
        {
            SignalMode = mode,
            CyclePeriod = cyclePeriod,
            Unit = unit
        };
        
        Metric metric = AppMetricsHelpers.From(command);
        List<Metric> metrics = new() { metric };
        await _app.PublishNodeCommand(metrics, node.GroupId, node.NodeId);

        logger.LogInformation($"===>>> App sending command to {node.GroupId}/{node.NodeId}: " + command.ToString());
    }

    private async Task Publish(SpBNode node, SignalStateType signalState, int vehicleCount)
    {
        SignalState signal = new()
        {
            LightState = signalState,
            VehicleCount = vehicleCount
        };

        Metric metric = NodeMetricsHelpers.From(signal);
        List<Metric> metrics = new() { metric };
        await node.Publish(metrics);

        logger.LogInformation($"===>>> {node.GroupId}/NDATA/{node.NodeId} publishing: " + signal.ToString());
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _node1.StopAsync();
        _node2.StopAsync();
        _app.StopAsync();
    }
}
