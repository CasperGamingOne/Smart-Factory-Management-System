namespace Smart_Factory_Management_System
{
    internal static class MenuOptions
    {
        public static readonly string[] ProductTypes = new[] { "Microprocessor", "Motherboard" };

        public static readonly string[] FormFactors = new[] { "ATX", "Micro-ATX", "Mini-ITX", "E-ATX" };

        public static readonly string[] TechAssignChoices = new[] { "Auto-assign (first available)", "Choose specific technician" };
        
        public static readonly string[] MainMenu = new[] {
            "1. Quick Actions",
            "2. Employee Management",
            "3. Machine Management",
            "4. Product Management",
            "5. Accounting",
            "6. Reports",
            "7. Factory Information",
            "8. Log Out / Exit Session"
        };

        public static readonly string[] MachineMenu = new[] {
            "1. Overall Fleet Status Overview",
            "2. Run Deep Component Inspection",
            "3. Fulfill Pending Orders",
            "4. Return to Main Menu"
        };

        public static readonly string[] ProductMenu = new[] {
            "1. View Finished Goods Stock",
            "2. View Inventory Financial & Capacity Analytics",
            "3. Sales & Orders",
            "4. Return to Main Menu"
        };

        public static readonly string[] FactoryReportMenu = new[] {
            "1. Factory Overview",
            "2. Staffing Report",
            "3. Machine Fleet Report",
            "4. Inventory Report",
            "5. Return to Main Menu"
        };

        public static readonly string[] SalesMenu = new[] {
            "1. Place Production Order",
            "2. View Pending Orders",
            "3. Record Sale for Batch",
            "4. Return to Main Menu"
        };

        public static readonly string[] ProductionActions = new[] {
            "1. Fulfill Pending Order",
            "2. Return to Main Menu"
        };

        public static readonly string[] AccountingMenu = new[] {
            "1. View Batches",
            "2. Set Unit Sell Price for Batch",
            "3. Return to Main Menu"
        };

        public static readonly string[] EmployeeRoles = new[] { "Technician", "Sales Agent", "Accountant" };
        
        public static readonly string[] EmployeeManagementMenu = new[] {
            "1. View All Registered Staff",
            "2. Add New Employee",
            "3. Return to Main Menu"
        };
    }
}
