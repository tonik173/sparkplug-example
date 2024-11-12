using SparkplugNet.VersionB.Data;
using Domain;

namespace SpCommon;

public class AppMetricsHelpers
{
    const string _metricName = "trafficControl/" + nameof(SignalCommand);

    public static Metric From(SignalCommand signalCommand)
    {
        Metric signalModeMetric = new Metric(DataType.Int8, signalCommand.SignalMode);
        signalModeMetric.Name = nameof(SignalCommand.SignalMode);

        Metric cyclePeriodMetric = new Metric(DataType.Int16, signalCommand.CyclePeriod);
        cyclePeriodMetric.Name = nameof(SignalCommand.CyclePeriod);

        Metric unitMetric = new Metric(DataType.Int8, signalCommand.Unit);
        unitMetric.Name = nameof(SignalCommand.Unit);

        Template template = new();
        template.IsDefinition = false;
        template.TemplateRef = nameof(SignalCommand);
        template.Metrics = [signalModeMetric, cyclePeriodMetric, unitMetric];

        Metric templateMetric = new(DataType.Template, template);
        templateMetric.Name = _metricName;

        return templateMetric;
    }

    public static SignalCommand From(IEnumerable<Metric> metrics)
    {
        SignalCommand signalCommand = new();
        foreach (Metric metric in metrics)
        {
            if (!_metricName.Equals(metric.Name)) break;

            Template? template = metric.Value as Template;
            if (template == null) break;

            foreach (Metric contentMetric in template.Metrics)
            {
                if (contentMetric.Name.Equals(nameof(SignalCommand.SignalMode)))
                    signalCommand.SignalMode = Enum.Parse<SignalModeType>(contentMetric.Value.ToString());

                if (contentMetric.Name.Equals(nameof(SignalCommand.Unit)))
                    signalCommand.Unit = Enum.Parse<UnitType>(contentMetric.Value.ToString());

                if (contentMetric.Name.Equals(nameof(SignalCommand.CyclePeriod)))
                    signalCommand.CyclePeriod = int.Parse(contentMetric.Value.ToString());
            }
        }
        return signalCommand;
    }

    // Definition of a template to transport the user data type <SignalCommand>.
    public static Metric CreateSignalCommandTemplate()
    {
        Metric signalModeMetric = new Metric(DataType.Int8, SignalModeType.Off);
        signalModeMetric.Name = nameof(SignalCommand.SignalMode);
        signalModeMetric.Properties = Common.GetPropertySet<SignalModeType>();

        Metric cyclePeriodMetric = new Metric(DataType.Int16, 30);
        cyclePeriodMetric.Name = nameof(SignalCommand.CyclePeriod);

        Metric unitMetric = new Metric(DataType.Int8, UnitType.Seconds);
        unitMetric.Name = nameof(SignalCommand.Unit);
        unitMetric.Properties = Common.GetPropertySet<UnitType>();

        Template template = new();
        template.IsDefinition = true;
        template.Metrics = [signalModeMetric, cyclePeriodMetric, unitMetric];

        Metric metric = new(DataType.Template, template);
        metric.Name = _metricName;

        return metric;
    }
}