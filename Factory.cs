namespace Smart_Factory_Management_System
{
    internal class Factory
    {
        public Employee[] Employees { get; private protected set; } = new Employee[100];
        public Machine[] Machines { get; private protected set; } = new Machine[50];
        public Product[] Inventory { get; private protected set; } = new Product[200];

        private int _employeeCount = 0;
        private int _machineCount = 0;
        private int _productCount = 0;

        public void AddEmployee(Employee emp)
        {
            if (_employeeCount < Employees.Length)
            {
                Employees[_employeeCount++] = emp;
            }
        }

        public void AddMachine(Machine mach)
        {
            if (_machineCount < Machines.Length)
            {
                Machines[_machineCount++] = mach;
            }
        }

    }
}
