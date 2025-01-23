using SparkplugNet.VersionB.Data;
using Domain;

namespace SpCommon;

public class AppMetricsHelpers
{
    const string _metricName = "trafficControl/" + nameof(SignalModeCommand);

    public static Metric From(SignalModeCommand signalCommand)
    {
        ulong timestamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        Metric signalModeMetric = new Metric(DataType.Int8, signalCommand.SignalMode);
        signalModeMetric.Name = nameof(SignalModeCommand.SignalMode);
        signalModeMetric.Timestamp = timestamp;

        Metric cyclePeriodMetric = new Metric(DataType.Int16, signalCommand.CyclePeriod);
        cyclePeriodMetric.Name = nameof(SignalModeCommand.CyclePeriod);
        cyclePeriodMetric.Timestamp = timestamp;

        Metric unitMetric = new Metric(DataType.Int8, signalCommand.Unit);
        unitMetric.Name = nameof(SignalModeCommand.Unit);
        unitMetric.Timestamp = timestamp;

        Template template = new();
        template.IsDefinition = false;
        template.TemplateRef = nameof(SignalModeCommand);
        template.Metrics = [signalModeMetric, cyclePeriodMetric, unitMetric];

        Metric templateMetric = new(DataType.Template, template);
        templateMetric.Name = _metricName;

        return templateMetric;
    }

    public static SignalModeCommand From(IEnumerable<Metric> metrics)
    {
        SignalModeCommand signalCommand = new();
        foreach (Metric metric in metrics)
        {
            if (!_metricName.Equals(metric.Name)) break;

            Template? template = metric.Value as Template;
            if (template == null) break;

            foreach (Metric contentMetric in template.Metrics)
            {
                if (contentMetric.Name.Equals(nameof(SignalModeCommand.SignalMode)))
                    signalCommand.SignalMode = Enum.Parse<SignalModeType>(contentMetric.Value.ToString());

                if (contentMetric.Name.Equals(nameof(SignalModeCommand.Unit)))
                    signalCommand.Unit = Enum.Parse<UnitType>(contentMetric.Value.ToString());

                if (contentMetric.Name.Equals(nameof(SignalModeCommand.CyclePeriod)))
                    signalCommand.CyclePeriod = int.Parse(contentMetric.Value.ToString());
            }
        }
        return signalCommand;
    }

    // Definition of a template to transport the user data type <SignalModeCommand>.
    public static Metric CreateSignalCommandTemplate()
    {
        ulong timestamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        Metric signalModeMetric = new Metric(DataType.Int8, SignalModeType.Off);
        signalModeMetric.Name = nameof(SignalModeCommand.SignalMode);
        signalModeMetric.Properties = Common.GetPropertySet<SignalModeType>();
        signalModeMetric.Timestamp = timestamp;

        Metric cyclePeriodMetric = new Metric(DataType.Int16, 30);
        cyclePeriodMetric.Name = nameof(SignalModeCommand.CyclePeriod);
        cyclePeriodMetric.Timestamp = timestamp;
        
        Metric unitMetric = new Metric(DataType.Int8, UnitType.Seconds);
        unitMetric.Name = nameof(SignalModeCommand.Unit);
        unitMetric.Properties = Common.GetPropertySet<UnitType>();
        unitMetric.Timestamp = timestamp;

        Template template = new();
        template.IsDefinition = true;
        template.Metrics = [signalModeMetric, cyclePeriodMetric, unitMetric];

        Metric metric = new(DataType.Template, template);
        metric.Name = _metricName;

        return metric;
    }
}