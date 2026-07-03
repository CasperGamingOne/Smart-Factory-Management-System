namespace Smart_Factory_Management_System
{
    public abstract class Employee
    {
        private static int idCounter = 0;

        public int Id { get; private set; }
        public string Name { get; set; }
        public string Role { get; protected set;  }
    

        protected Employee(string name)
         {
            idCounter++;
            Id = idCounter;
            Name = name;
            Role = "Auxiliary";
         }

        public abstract string AfiseazaActivitate();

        // Each concrete employee class should implement the role-specific menu/action entry point
        internal abstract void OpenRoleMenu(Factory factory);
    }

    public class Director : Employee
    {
        public Director(string name) : base(name) { Role = "Director"; }
        public override string AfiseazaActivitate()
        {
            return "The Director " + Name + " verifies employees and has access to all reports.";
        }

        internal override void OpenRoleMenu(Factory factory)
        {
            EmployeeMenuHandler.Run(factory, this);
        }
    }

    public class Technician : Employee
    {
        public Technician(string name) : base(name) { Role = "Tehnician"; }
        public override string AfiseazaActivitate()
        {
            return "Technician " + Name + " supervises equipment and repairs defective parts.";
        }

        internal override void OpenRoleMenu(Factory factory)
        {
            MachineMenuHandler.Run(factory, this);
        }
    }

    public class SalesAgent : Employee
    {
        public SalesAgent(string name) : base(name) { Role = "Agent Vanzari"; }
        public override string AfiseazaActivitate()
        {
            return "Sales Agent " + Name + " places orders, sets prices, and tracks sales.";
        }

        internal override void OpenRoleMenu(Factory factory)
        {
            SalesMenuHandler.Run(factory, this);
        }
    }
    public class Accountant : Employee
    {
        public Accountant(string name) : base(name) { Role = "Contabil"; }
        public override string AfiseazaActivitate()
        {
            return "Accountant " + Name + " takes care of any type of reports and financial statements.";
        }

        internal override void OpenRoleMenu(Factory factory)
        {
            AccountingMenuHandler.Run(factory, this);
        }
    }


}
