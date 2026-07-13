namespace Smart_Factory_Management_System;

public static class Authentication
{
    public static Employee? Authenticate(Factory factory, string employeeId)
    {
        for (var i = 0; i < factory.EmployeeCount; i++)
            if (factory.Employees[i].Id.ToString().Equals(employeeId, StringComparison.OrdinalIgnoreCase))
                return factory.Employees[i];

        return null;
    }
}