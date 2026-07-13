namespace Smart_Factory_Management_System;

public enum PartCondition
{
    Excellent,
    Good,
    Critical
}

public abstract class MachinePart
{
    public MachinePart(string name, PartCondition? condition)
    {
        Name = name;
        Condition = condition;
    }

    public string? Name { get; private protected set; }
    public PartCondition? Condition { get; private protected set; }

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

public class Power_Supply : MachinePart
{
    public Power_Supply(string name, PartCondition? condition, int voltage) : base(name, condition)
    {
        Voltage = voltage;
    }

    public int Voltage { get; }

    public override string PrintPartInfo()
    {
        return $"Voltage Output: [cyan]{Voltage}V[/]";
    }
}

public class Cooling_System : MachinePart
{
    public Cooling_System(string name, PartCondition? condition, string type) : base(name, condition)
    {
        Type = type;
    }

    public string Type { get; private protected set; }

    public override string PrintPartInfo()
    {
        return $"Cooling System Type: [cyan]{Type}[/]";
    }
}

public class Control_Unit : MachinePart
{
    public Control_Unit(string name, PartCondition? condition, string processor) : base(name, condition)
    {
        Processor = processor;
    }

    public string Processor { get; private protected set; }

    public override string PrintPartInfo()
    {
        return $"Processor: [cyan]{Processor}[/]";
    }
}

public class AOI_System : MachinePart
{
    public AOI_System(string name, PartCondition? condition, string systemType) : base(name, condition)
    {
        SystemType = systemType;
    }

    public string SystemType { get; private protected set; }

    public override string PrintPartInfo()
    {
        return $"AOI System Type: [cyan]{SystemType}[/]";
    }
}