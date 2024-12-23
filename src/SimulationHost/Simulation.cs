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

    private readonly Random _random = new();

    private async Task StartSparkplugNetworkSimulation()
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
        _node1.SignalModeCommandReceived += async newSignalCommand =>
        {
            await ProcessSignalModeCommand(_node1, newSignalCommand);
        };
        await _node1.StartAsync(new EdgeNode.Config("DemoNode1")).ConfigureAwait(false);


        // Sparkplug Node 2
        _node2 = new(loggerFactory.CreateLogger<SpBNode>());
        _node2.SignalModeCommandReceived += async newSignalCommand =>
        {
            await ProcessSignalModeCommand(_node2, newSignalCommand);
        };
        await _node2.StartAsync(new EdgeNode.Config("DemoNode2")).ConfigureAwait(false);


        // Start simulation
        int count = 0;
        while (true)
        {
            logger.LogInformation($"******************************** Network simulation {++count} ********************************");

            try {
                await Task.Delay(5000);
                await Command(SignalModeType.Blinking, 2, UnitType.Seconds, _node1);
                await Command(SignalModeType.Blinking, 2, UnitType.Seconds, _node2);

                await Task.Delay(5000);
                await Command(SignalModeType.Operation, 5, UnitType.Seconds, _node1);
                await Command(SignalModeType.Operation, 5, UnitType.Seconds, _node2);

                await Task.Delay(5000);
                await Command(SignalModeType.Off, 0, UnitType.Seconds, _node1);
                await Command(SignalModeType.Off, 0, UnitType.Seconds, _node2);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in simulation");
            }
        }
    }

    private async Task Command(SignalModeType mode, int cyclePeriod, UnitType unit, SpBNode node)
    {
        SignalModeCommand command = new()
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

    private async Task ProcessSignalModeCommand(SpBNode node, SignalModeCommand newSignalCommand)
    {
        logger.LogInformation($"<<<=== {node.GroupId}/NCMD/{node.NodeId} received: " + newSignalCommand);
        switch (newSignalCommand.SignalMode)
        {
            case SignalModeType.Operation:
                await Publish(node, SignalStateType.Red, _random.Next(0, 10));
                break;
            case SignalModeType.Blinking:
                await Publish(node, SignalStateType.Yellow, _random.Next(0, 10));
                break;
            case SignalModeType.Off:
                await Publish(node, SignalStateType.Off, _random.Next(0, 10));
                break;
        }
    }

    private async Task StartSparkplugEdgeNodesSimulation()
    {
        // Sparkplug Node 1
        _node1 = new(loggerFactory.CreateLogger<SpBNode>());
        _node1.SignalModeCommandReceived += async newSignalCommand =>
        {
            await ProcessSignalModeCommand(_node1, newSignalCommand);
        };
        await _node1.StartAsync(new EdgeNode.Config("DemoNode1")).ConfigureAwait(false);


        // Sparkplug Node 2
        _node2 = new(loggerFactory.CreateLogger<SpBNode>());
        _node2.SignalModeCommandReceived += async newSignalCommand =>
        {
            await ProcessSignalModeCommand(_node2, newSignalCommand);
        };
        await _node2.StartAsync(new EdgeNode.Config("DemoNode2")).ConfigureAwait(false);


        // Publish signal state
        int count = 0;
        while (true)
        {
            logger.LogInformation($"******************************** Edge node simulation {++count} ********************************");

            try {
                await Task.Delay(1000);
                await Publish(_node1, SignalStateType.Green, _random.Next(0, 10));
                await Publish(_node2, SignalStateType.Red, _random.Next(0, 10));

                await Task.Delay(5000);
                await Publish(_node1, SignalStateType.Yellow, _random.Next(0, 2));
                await Publish(_node2, SignalStateType.Yellow, _random.Next(0, 2));

                await Task.Delay(1000);
                await Publish(_node1, SignalStateType.Red, _random.Next(0, 10));
                await Publish(_node2, SignalStateType.Green, _random.Next(0, 10));

                await Task.Delay(5000);
                await Publish(_node1, SignalStateType.Yellow, _random.Next(0, 2));
                await Publish(_node2, SignalStateType.Yellow, _random.Next(0, 2));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in simulation");
            }
        }
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

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        string simulationMode = Environment.GetEnvironmentVariable("SIMULATION_MODE") ?? string.Empty;
        logger.LogInformation($"Simulation mode: {simulationMode}");
        if (simulationMode == "edgeNodesOnly")
            await StartSparkplugEdgeNodesSimulation();
        else
            await StartSparkplugNetworkSimulation();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _node1.StopAsync();
        await _node2.StopAsync();
        await _app.StopAsync();
    }
}
