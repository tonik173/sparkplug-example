namespace Domain;

public enum SignalModeType
{
    Off = 0,
    Blinking = 1,
    Operation = 2
}

public enum UnitType
{
    Seconds = 0,
    Minutes = 1,
}

public class SignalCommand
{
    public SignalModeType SignalMode { get; set; } = SignalModeType.Off;

    public int CyclePeriod { get; set; } = 30;

    public UnitType Unit { get; set; } = UnitType.Seconds;


    public override string ToString()
    {
        return $"New signal mode {SignalMode.ToString()}. Cycle period {CyclePeriod} {Unit.ToString()}";
    }
}