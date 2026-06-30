namespace Smart_Factory_Management_System
{
    public enum MachineStatus { Running, Stopped, Maintenance }

    public enum MachineCondition { Excellent, Good, Critical }

    abstract internal class Machine
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
            InstallationDate = DateTime.Now.AddYears(-5);
            Parts = parts;
            Condition = condition;
        }

        public TimeSpan GetMachineAge()
        {
            return DateTime.Now - InstallationDate;
        }

        public abstract void Produce(Product product);

    }
}
