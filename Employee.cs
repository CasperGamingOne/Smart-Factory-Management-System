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

        public abstract void AfiseazaActivitate();

        // Each concrete employee class should implement the role-specific menu/action entry point
        internal abstract void OpenRoleMenu(Factory factory);
    }
    //clasele derivate pentru diferite tipuri de angajati
    public class Director : Employee
    {
        public Director(string name) : base(name) { Role = "Director"; }
        public override void AfiseazaActivitate()
        {
            Console.WriteLine("The Director " + Name + " verifies employees and has access to all reports.");
        }

        internal override void OpenRoleMenu(Factory factory)
        {
            EmployeeMenuHandler.Run(factory, this);
        }
    }

    public class Technician : Employee
    {
        public Technician(string name) : base(name) { Role = "Tehnician"; }
        public override void AfiseazaActivitate()
        {
            Console.WriteLine("Technician " + Name + " supervises equipment and repairs defective parts.");
        }

        internal override void OpenRoleMenu(Factory factory)
        {
            MachineMenuHandler.Run(factory, this);
        }
    }

    public class SalesAgent : Employee
    {
        public SalesAgent(string name) : base(name) { Role = "Agent Vanzari"; }
        public override void AfiseazaActivitate()
        {
            Console.WriteLine("Sales Agent " + Name + " places orders, sets prices, and tracks sales.");
        }

        internal override void OpenRoleMenu(Factory factory)
        {
            SalesMenuHandler.Run(factory, this);
        }
    }
    public class Accountant : Employee
    {
        public Accountant(string name) : base(name) { Role = "Contabil"; }
        public override void AfiseazaActivitate()
        {
            Console.WriteLine("Accountant " + Name + " makes financial reports and sets products sell price.");
        }

        internal override void OpenRoleMenu(Factory factory)
        {
            AccountingMenuHandler.Run(factory, this);
        }
    }


}
