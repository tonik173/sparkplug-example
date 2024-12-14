using SparkplugNet.VersionB.Data;
using Domain;

namespace SpCommon;

public class NodeMetricsHelpers
{
    const string _metricName = "trafficInfo/" + nameof(SignalState);

    // Transforms a SignalState object to a Sparkplug Metric
    public static Metric From(SignalState signalState)
    {
        Metric lightStateMetric = new Metric(DataType.Int8, signalState.LightState);
        lightStateMetric.Name = nameof(SignalState.LightState);

        Metric vehicleCountMetric = new Metric(DataType.Int16, signalState.VehicleCount);
        vehicleCountMetric.Name = nameof(SignalState.VehicleCount);

        Template template = new();
        template.IsDefinition = false;
        template.TemplateRef = nameof(SignalState);
        template.Metrics = [lightStateMetric, vehicleCountMetric];

        Metric templateMetric = new(DataType.Template, template);
        templateMetric.Name = _metricName;

        return templateMetric;
    }

    public static SignalState From(IEnumerable<Metric> metrics)
    {
        SignalState signalState = new();
        foreach (Metric metric in metrics)
        {
            if (!_metricName.Equals(metric.Name)) break;

            Template? template = metric.Value as Template;
            if (template == null) break;

            foreach (Metric contentMetric in template.Metrics)
            {
                if (contentMetric.Name.Equals(nameof(SignalState.LightState)))
                    signalState.LightState = Enum.Parse<SignalStateType>(contentMetric.Value.ToString());

                if (contentMetric.Name.Equals(nameof(SignalState.VehicleCount)))
                    signalState.VehicleCount = int.Parse(contentMetric.Value.ToString());
            }
        }
        return signalState;
    }

    // Definition of a template to transport the user data type <SignalState>.
    public static Metric CreateSignalStateTemplate()
    {
        Metric lightStateMetric = new Metric(DataType.Int8, SignalStateType.Red);
        lightStateMetric.Name = nameof(SignalState.LightState);
        lightStateMetric.Properties = Common.GetPropertySet<SignalStateType>();

        Metric vehicleCountMetric = new Metric(DataType.Int16, 0);
        vehicleCountMetric.Name = nameof(SignalState.VehicleCount);

        Template template = new();
        template.IsDefinition = true;
        template.Metrics = [lightStateMetric, vehicleCountMetric];

        Metric metric = new(DataType.Template, template);
        metric.Name = _metricName;

        return metric;
    }
}