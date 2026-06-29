namespace Smart_Factory_Management_System
{
    public enum MachineStatus { Running, Stopped, Maintenance }

    public enum MachineCondition { Excellent, Good, Critical }

    abstract internal class Machine
    {
        private protected string? machine_name, machine_manufacturer, machine_serial;

        private protected DateTime installation_date;

        private protected MachinePart[]? machine_parts;

        private protected MachineStatus status;

        private protected MachineCondition condition;

        public string? Name { get { return machine_name; } set { machine_name = value; } }

        public string? Manufacturer { get { return machine_manufacturer; } set { machine_manufacturer = value; } }

        public string? SerialNumber { get { return machine_serial; } set { machine_serial = value; } }

        public DateTime InstallationDate { get { return installation_date; } set { installation_date = value; } }

        public MachinePart[]? Parts { get { return machine_parts; } set { machine_parts = value; } }

        public MachineStatus Status { get { return status; } set { status = MachineStatus.Stopped; }  }

        public MachineCondition Condition { get { return condition; } set { condition = MachineCondition.Excellent; } }

        public Machine(string machine_name, string machine_manufacturer, string machine_serial, MachinePart[] parts)
        {
            Name = machine_name;
            Manufacturer = machine_manufacturer;
            SerialNumber = machine_serial;
            InstallationDate = DateTime.Now.AddYears(-5);
            Parts = parts;
        }

        
    }
}
