namespace Smart_Factory_Management_System
{
    public enum PartCondition { Excellent, Good, Critical }

    internal abstract class MachinePart
    {
        public string? Name { get; private protected set; }
        public PartCondition? Condition { get; private protected set; }

        public MachinePart(string name, PartCondition? condition)
        {
            Name = name;
            Condition = condition;
        }

        public abstract void PrintPartInfo();
    }

    internal class Power_Supply : MachinePart
    {
        public int Voltage { get; }
        public Power_Supply(string name, PartCondition? condition, int voltage) : base(name, condition)
        {
            Voltage = voltage;
        }
        public override void PrintPartInfo()
        {
            Console.WriteLine($"Power Supply Name: {Name}, Condition: {Condition}, Voltage: {Voltage}V");
        }
    }

    internal class Cooling_System : MachinePart
    {
        public string Type { get; private protected set; }
        public Cooling_System(string name, PartCondition? condition, string type) : base(name, condition)
        {
            Type = type;
        }
        public override void PrintPartInfo()
        {
            Console.WriteLine($"Cooling System Name: {Name}, Condition: {Condition}, Type: {Type}");
        }
    }

    internal class Control_Unit : MachinePart
    {
        public string Processor { get; private protected set; }
        public Control_Unit(string name, PartCondition? condition, string processor) : base(name, condition)
        {
            Processor = processor;
        }
        public override void PrintPartInfo()
        {
            Console.WriteLine($"Control Unit Name: {Name}, Condition: {Condition}, Processor: {Processor}");
        }
    }

    internal class AOI_System : MachinePart
    {
        public string SystemType { get; private protected set; }
        public AOI_System(string name, PartCondition? condition, string systemType) : base(name, condition)
        {
            SystemType = systemType;
        }
        public override void PrintPartInfo()
        {
            Console.WriteLine($"AOI System Name: {Name}, Condition: {Condition}, System Type: {SystemType}");
        }
    }
}
