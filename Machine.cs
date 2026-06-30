namespace Smart_Factory_Management_System
{
    public enum MachineStatus { Running, Stopped, Maintenance }

    public enum MachineCondition { Excellent, Good, Critical }

    internal abstract class Machine
    {
        public string? Name { get; private protected set; }

        public string? Manufacturer { get; private protected set; }

        public string? SerialNumber { get; private protected set; }

        public DateTime InstallationDate { get; private protected set; }

        public MachinePart[]? Parts { get; private protected set; }

        public MachineStatus Status { get; private protected set; } = MachineStatus.Stopped;

        public MachineCondition Condition { get; private protected set; }

        public Machine(string machine_name, string machine_manufacturer, string machine_serial, MachinePart[] parts, MachineCondition condition)
        {
            Name = machine_name;
            Manufacturer = machine_manufacturer;
            SerialNumber = machine_serial;
            InstallationDate = DateTime.Now.AddYears(-7);
            Parts = parts;
            Condition = condition;
        }

        public TimeSpan GetMachineAge()
        {
            return DateTime.Now - InstallationDate;
        }

        public abstract void Produce(Product product);       
    }

    internal class Litography_Machine : Machine
    {
        public Litography_Machine(string machine_name, string machine_manufacturer, string machine_serial, MachinePart[] parts, MachineCondition condition)
            : base(machine_name, machine_manufacturer, machine_serial, parts, condition)
        {
        }
        public override void Produce(Product product)
        {
            Console.WriteLine($"Litography Machine {Name} is producing {product.Name}.");
        }
    }

    internal class SMT_Machine : Machine  // Solder Paste Printer
    {
        public SMT_Machine(string machine_name, string machine_manufacturer, string machine_serial, MachinePart[] parts, MachineCondition condition)
            : base(machine_name, machine_manufacturer, machine_serial, parts, condition)
        {
        }
        public override void Produce(Product product)
        {
            Console.WriteLine($"SMT Machine {Name} is producing {product.Name}.");
        }
    }

    internal class PaP_Machine : Machine  // Pick and Place
    {
        public PaP_Machine(string machine_name, string machine_manufacturer, string machine_serial, MachinePart[] parts, MachineCondition condition)
            : base(machine_name, machine_manufacturer, machine_serial, parts, condition)
        {
        }
        public override void Produce(Product product)
        {
            Console.WriteLine($"PAP Machine {Name} is producing {product.Name}.");
        }
    }

    internal class Reflow_Oven : Machine
    {
        public Reflow_Oven(string machine_name, string machine_manufacturer, string machine_serial, MachinePart[] parts, MachineCondition condition)
            : base(machine_name, machine_manufacturer, machine_serial, parts, condition)
        {
        }
        public override void Produce(Product product)
        {
            Console.WriteLine($"Reflow Oven {Name} is producing {product.Name}.");
        }
    }
}
