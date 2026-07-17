namespace Smart_Factory_Management_System;

public static class MenuOptions
{
    public static readonly string[] ProductTypes = ["Microprocessor", "Motherboard"];

    public static readonly string[] FormFactors = ["ATX", "Micro-ATX", "Mini-ITX", "E-ATX"];

    public static readonly string[] TechAssignChoices = ["Auto-assign (first available)", "Choose specific technician"];

    public static readonly string[] MainMenu =
    [
        "Quick Actions",
        "Employee Management",
        "Machine Management",
        "Product Management",
        "Accounting",
        "Reports",
        "Log Out / Exit Session"
    ];

    public static readonly string[] MachineMenu =
    [
        "Overall Fleet Status Overview",
        "Run Deep Component Inspection",
        "Fulfill Pending Orders",
        "Return to Main Menu"
    ];

    public static readonly string[] ProductMenu =
    [
        "View Finished Goods Stock",
        "View Inventory Financial & Capacity Analytics",
        "Sales & Orders",
        "Return to Main Menu"
    ];

    public static readonly string[] FactoryReportMenu =
    [
        "Factory Overview",
        "Staffing Report",
        "Machine Fleet Report",
        "Inventory Report",
        "Return to Main Menu"
    ];

    public static readonly string[] ReportMenu =
    [
        "Production Summary",
        "Employee Report",
        "Order Backlog Summary",
        "Return to Main Menu"
    ];

    public static readonly string[] SalesMenu =
    [
        "Place Production Order",
        "View Pending Orders",
        "Record Sale for Batch",
        "Return to Main Menu"
    ];

    public static readonly string[] AccountingMenu =
    [
        "View Batches",
        "Process Report Requests",
        "Return to Main Menu"
    ];

    public static readonly string[] EmployeeRoles = ["Technician", "Sales Agent", "Accountant"];

    public static readonly string[] EmployeeManagementMenu =
    [
        "View All Registered Staff",
        "Add New Employee",
        "Remove Employee",
        "Return to Main Menu"
    ];
}