using System.Text.Json;

namespace Smart_Factory_Management_System;

public class JsonRepository<T>(IFileSystemService fileService, string fileName) : IJsonRepository<T>
{
    //private const string AuthFileName = "employees.json";

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };

    public List<T> Load()
    {
        var json = fileService.ReadFromFile(fileName);
        return string.IsNullOrEmpty(json)
            ? new List<T>()
            : JsonSerializer.Deserialize<List<T>>(json, SerializerOptions) ?? new List<T>();
    }

    public void Save(IEnumerable<T> items)
    {
        var json = JsonSerializer.Serialize(items, SerializerOptions);
        fileService.WriteToFile(fileName, json);
    }
}