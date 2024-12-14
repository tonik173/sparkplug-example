using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SimulationHost;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureAppConfiguration((hostBuilderContext, configurationBuilder) =>
{
    configurationBuilder.AddCommandLine(args);
    configurationBuilder.AddEnvironmentVariables();
}).ConfigureServices((context, services) =>
{
    services.AddHostedService<Simulation>();

    // Logging configuration for Demo.Host
    services.AddLogging(loggingBuilder => loggingBuilder
        .AddSimpleConsole()
        .SetMinimumLevel(LogLevel.Trace));
});

IHost host = builder.Build();


await host.RunAsync();
return 0;