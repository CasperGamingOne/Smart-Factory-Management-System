namespace Smart_Factory_Management_System;

public abstract class Employee
{
    private static int _idCounter;


    protected Employee(string name)
    {
        _idCounter++;
        Id = _idCounter;
        Name = name;
        Role = "Auxiliary";
    }

    public int Id { get; private set; }
    public string Name { get; set; }
    public string Role { get; protected init; }
    public abstract string QuickActionName { get; }

    public abstract string ShowActivity();
    public abstract List<string> GetAvailableMenuOptions();
}

public class Director : Employee
{
    public Director(string name) : base(name)
    {
        Role = "Director";
    }

    public override string QuickActionName => "Quick Actions";

    public override string ShowActivity()
    {
        return "The Director " + Name + " verifies employees and has access to all reports.";
    }

    public override List<string> GetAvailableMenuOptions()
    {
        return
        [
            "Quick Actions",
            "Employee Management",
            "Reports",
            "Log Out / Exit Session"
        ];
    }
}

public class Technician : Employee
{
    public Technician(string name) : base(name)
    {
        Role = "Technician";
    }

    public override string QuickActionName => "Quick Actions";

    public override string ShowActivity()
    {
        return "Technician " + Name + " supervises equipment and repairs defective parts.";
    }

    public override List<string> GetAvailableMenuOptions()
    {
        return
        [
            "Quick Actions",
            "Machine Management",
            "Reports",
            "Factory Information",
            "Log Out / Exit Session"
        ];
    }
}

public class SalesAgent : Employee
{
    public SalesAgent(string name) : base(name)
    {
        Role = "Sales Agent";
    }

    public override string QuickActionName => "Quick Actions";

    public override string ShowActivity()
    {
        return "Sales Agent " + Name + " places orders, sets prices, and tracks sales.";
    }

    public override List<string> GetAvailableMenuOptions()
    {
        return
        [
            "Quick Actions",
            "Product Management",
            "Log Out / Exit Session"
        ];
    }
}

public class Accountant : Employee
{
    public Accountant(string name) : base(name)
    {
        Role = "Accountant";
    }

    public override string QuickActionName => "N/A";

    public override string ShowActivity()
    {
        return "Accountant " + Name + " takes care of any type of reports and financial statements.";
    }

    public override List<string> GetAvailableMenuOptions()
    {
        return
        [
            "Accounting",
            "Reports",
            "Log Out / Exit Session"
        ];
    }
}