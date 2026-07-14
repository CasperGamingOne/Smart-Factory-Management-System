using System.Text.Json;

namespace Smart_Factory_Management_System;

public class JsonAuthRepository(IFileSystemService fileService) : IAuthRepository<Employee>
{
    private const string AuthFileName = "employees.json";

    public List<Employee> LoadUsers()
    {
        // Now you use your service instead of File.Exists directly!
        var json = fileService.ReadFromFile(AuthFileName);

        if (string.IsNullOrEmpty(json))
            return SeedDefaultUsers();

        return JsonSerializer.Deserialize<List<Employee>>(json) ?? new List<Employee>();
    }

    public void SaveUsers(List<Employee> users)
    {
        var json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
        fileService.WriteToFile(AuthFileName, json);
    }

    private List<Employee> SeedDefaultUsers()
    {
        var users = new List<Employee>
        {
            new Director("Andrei Popescu", "andrei", SecurityHelper.HashPassword("password123")),
            new Technician("Maria Ionescu", "maria", SecurityHelper.HashPassword("password123")),
            new SalesAgent("Alexandru Dumitru", "alex", SecurityHelper.HashPassword("password123")),
            new Accountant("Elena Vasilescu", "elena", SecurityHelper.HashPassword("password123"))
        };
        SaveUsers(users);
        return users;
    }
}