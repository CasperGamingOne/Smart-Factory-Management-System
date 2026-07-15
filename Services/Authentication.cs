namespace Smart_Factory_Management_System;

public static class Authentication
{
    public static Employee? Authenticate(IJsonRepository<Employee> repository, string username, string password)
    {
        var users = repository.Load();
        var employee = users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

        if (employee != null && SecurityHelper.VerifyPassword(password, employee.PasswordHash)) return employee;

        return null;
    }
}