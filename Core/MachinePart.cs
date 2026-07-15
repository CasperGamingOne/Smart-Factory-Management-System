using System.Text.Json.Serialization;

namespace Smart_Factory_Management_System;

public enum PartCondition
{
    Excellent,
    Good,
    Critical
}

[JsonDerivedType(typeof(PowerSupply), "powerSupply")]
[JsonDerivedType(typeof(CoolingSystem), "coolingSystem")]
[JsonDerivedType(typeof(ControlUnit), "controlUnit")]
[JsonDerivedType(typeof(AoiSystem), "aoiSystem")]
public abstract class MachinePart(string name, PartCondition? condition)
{
    public string? Name { get; private protected set; } = name;
    public PartCondition? Condition { get; private set; } = condition;

    public void BreakDown()
    {
        Condition = PartCondition.Critical;
    }

    public void Repair(PartCondition restoredCondition)
    {
        Condition = restoredCondition;
    }

    public void DegradeStep()
    {
        if (Condition == PartCondition.Excellent)
            Condition = PartCondition.Good;
        else if (Condition == PartCondition.Good)
            Condition = PartCondition.Critical;
    }

    public abstract string PrintPartInfo();
}

public class PowerSupply(string name, PartCondition? condition, int voltage) : MachinePart(name, condition)
{
    public int Voltage { get; init; } = voltage;

    public override string PrintPartInfo()
    {
        return $"Voltage Output: [cyan]{Voltage}V[/]";
    }
}

public class CoolingSystem(string name, PartCondition? condition, string type) : MachinePart(name, condition)
{
    public string Type { get; init; } = type;

    public override string PrintPartInfo()
    {
        return $"Cooling System Type: [cyan]{Type}[/]";
    }
}

public class ControlUnit(string name, PartCondition? condition, string processor) : MachinePart(name, condition)
{
    public string Processor { get; init; } = processor;

    public override string PrintPartInfo()
    {
        return $"Processor: [cyan]{Processor}[/]";
    }
}

public class AoiSystem(string name, PartCondition? condition, string systemType) : MachinePart(name, condition)
{
    public string SystemType { get; init; } = systemType;

    public override string PrintPartInfo()
    {
        return $"AOI System Type: [cyan]{SystemType}[/]";
    }
}