namespace Smart_Factory_Management_System
{
    internal class Factory
    {
        public Employee[] Employees { get; private protected set; } = new Employee[100];
        public Machine[] Machines { get; private protected set; } = new Machine[50];
        public Product[] Inventory { get; private protected set; } = new Product[200];

        private int employeeCount = 0;
        private int machineCount = 0;
        private int productCount = 0;

        public void AddEmployee(Employee emp)
        {
            if (employeeCount < Employees.Length)
            {
                Employees[employeeCount++] = emp;
            }
        }

        public void AddMachine(Machine mach)
        {
            if (machineCount < Machines.Length)
            {
                Machines[machineCount++] = mach;
            }
        }

    }
}
