using SparkplugNet.VersionB.Data;

namespace SpCommon;

public class Common
{
    // Describes the meaning of an enum type
    public static PropertySet GetPropertySet<T>()
    {
        List<PropertyValue> propertyValues = new();
        foreach (T type in Enum.GetValues(typeof(T)).Cast<T>())
        {
            PropertyValue propertyValue = new(DataType.Int8, type);
            propertyValues.Add(propertyValue);
        }

        PropertySet propertySet = new()
        {
            Keys = Enum.GetNames(typeof(T)).ToList(),
            Values = propertyValues
        };

        return propertySet;
    }
}