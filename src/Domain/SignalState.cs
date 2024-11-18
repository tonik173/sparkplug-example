namespace Domain;

public enum SignalStateType
{
    Green = 0,
    Yellow = 1,
    Red = 2
}

public class SignalState
{
    public SignalStateType LightState { get; set; } = SignalStateType.Red;
    public int VehicleCount { get; set; } = 0;

    public override string ToString()
    {
        string vehiclesInfo;
        switch (LightState)
        {
            case SignalStateType.Green:
                vehiclesInfo = $"{VehicleCount} cars waiting";
                break;
            case SignalStateType.Yellow:
                vehiclesInfo = $"{VehicleCount} cars will pass";
                break;
            case SignalStateType.Red:
                vehiclesInfo = $"{VehicleCount} cars passed";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return $"Signal state became {LightState.ToString()}. {vehiclesInfo}";
    }
}

