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

        public bool StartMachine()
        {
            if (Status == MachineStatus.Maintenance)
            {
                Console.WriteLine($"[ERROR] Cannot start '{Name}'! It is currently flagged for Maintenance.");
                return false;
            }

            if (Parts == null)
            {
                Console.WriteLine($"[ERROR] Cannot start '{Name}' because it has no parts assigned.");
                Status = MachineStatus.Stopped;
                return false;
            }

            foreach (var part in Parts)
            {
                if (part.Condition == PartCondition.Critical)
                {
                    Console.WriteLine($"[CRITICAL] Cannot start '{Name}' because component '{part.Name}' is broken!");
                    Status = MachineStatus.Stopped;
                    return false;
                }
            }

            Status = MachineStatus.Running;
            Console.WriteLine($"[SYSTEM] Machine '{Name}' is now RUNNING production.");
            return true;
        }

        public void StopMachine()
        {
            if (Status == MachineStatus.Running)
            {
                Status = MachineStatus.Stopped;
                Console.WriteLine($"[SYSTEM] Machine '{Name}' has been safely STOPPED.");
            }
        }

        public void InspectMachine()
        {
            Console.WriteLine($"[INSPECTION] Running diagnostics on {Name} (S/N: {SerialNumber})...");
            bool Faults = false;

            if (Parts == null)
            {
                Console.WriteLine($"[ERROR] Cannot inspect '{Name}' because it has no parts assigned.");
                Condition = MachineCondition.Critical;
                Status = MachineStatus.Maintenance;
                return;
            }


            foreach (var part in Parts)
            {
                part.PrintPartInfo();
                if (part.Condition == PartCondition.Critical)
                {
                    Faults = true;
                }
            }

            if (Faults)
            {
                Condition = MachineCondition.Critical;
                Status = MachineStatus.Maintenance;
                Console.WriteLine($"[DIAGNOSTIC] '{Name}' failed inspection! Status forced to MAINTENANCE.");
            }
            else
            {
                Console.WriteLine($"[DIAGNOSTIC] '{Name}' passed inspection cleanly.");
            }
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
