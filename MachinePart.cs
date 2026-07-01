namespace Smart_Factory_Management_System
{
    public enum PartCondition { Excellent, Good, Critical }

    internal abstract class MachinePart
    {
        public string? Name { get; private protected set; }
        public PartCondition? Condition { get; private protected set; }

        private static readonly Random random = new Random()!;

        public MachinePart(string name, PartCondition? condition)
        {
            Name = name;
            Condition = condition;
        }

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

    internal class Power_Supply : MachinePart
    {
        public int Voltage { get; }
        public Power_Supply(string name, PartCondition? condition, int voltage) : base(name, condition)
        {
            Voltage = voltage;
        }
        public override string PrintPartInfo()
        {
            return $"Voltage Output: [cyan]{Voltage}V[/]";
        }
    }

    internal class Cooling_System : MachinePart
    {
        public string Type { get; private protected set; }
        public Cooling_System(string name, PartCondition? condition, string type) : base(name, condition)
        {
            Type = type;
        }
        public override string PrintPartInfo()
        {
            return $"Cooling System Type: [cyan]{Type}[/]";
        }
    }

    internal class Control_Unit : MachinePart
    {
        public string Processor { get; private protected set; }
        public Control_Unit(string name, PartCondition? condition, string processor) : base(name, condition)
        {
            Processor = processor;
        }
        public override string PrintPartInfo()
        {
            return $"Processor: [cyan]{Processor}[/]";
        }
    }

    internal class AOI_System : MachinePart
    {
        public string SystemType { get; private protected set; }
        public AOI_System(string name, PartCondition? condition, string systemType) : base(name, condition)
        {
            SystemType = systemType;
        }
        public override string PrintPartInfo()
        {
            return $"AOI System Type: [cyan]{SystemType}[/]";
        }
    }
}
