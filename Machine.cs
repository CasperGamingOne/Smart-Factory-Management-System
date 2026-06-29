namespace Smart_Factory_Management_System
{
    public enum MachineStatus { Running, Stopped, Maintenance }

    public enum MachineCondition { Excellent, Good, Critical }

    abstract internal class Machine
    {
        private protected string? machine_name, machine_manufacturer, machine_serial;

        private protected MachineStatus status;

        private protected MachineCondition condition;

        public string? Name { get { return machine_name; } set { machine_name = value; } }

        public string? Manufacturer { get { return machine_manufacturer; } set { machine_manufacturer = value; } }

        public string? SerialNumber { get { return machine_serial; } set { machine_serial = value; } }

        public MachineStatus Status { get { return status; } set { status = MachineStatus.Stopped; }  }

        public MachineCondition Condition { get { return condition; } set { condition = MachineCondition.Excellent; } }

        public Machine(string machine_name, string machine_manufacturer, string machine_serial)
        {
            Name = machine_name;
            Manufacturer = machine_manufacturer;
            SerialNumber = machine_serial;
        }
    }
}
