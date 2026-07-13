using System.Text.Json;

namespace Smart_Factory_Management_System;

public class JsonAuthRepository : IAuthRepository<Employee>
{
    private readonly string _filePath;

    public JsonAuthRepository()
    {
        var folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Smart-Factory-Management-System");
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        _filePath = Path.Combine(folderPath, "employees.json");
    }

    public List<Employee> LoadUsers()
    {
        if (!File.Exists(_filePath)) return SeedDefaultUsers();

        var json = File.ReadAllText(_filePath);
        var users = JsonSerializer.Deserialize<List<Employee>>(json) ?? new List<Employee>();

        var needsSave = false;
        foreach (var user in users)
            if (!user.IsPasswordHashed)
            {
                user.PasswordHash = SecurityHelper.HashPassword(user.PasswordHash);
                user.IsPasswordHashed = true;
                needsSave = true;
            }

        if (needsSave) SaveUsers(users);

        if (users.Count <= 0) return users;
        var maxId = users.Max(u => u.Id);
        Employee.InitializeIdCounter(maxId);

        return users;
    }

    public void SaveUsers(List<Employee> users)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(users, options);
        File.WriteAllText(_filePath, json);
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