using System.Text.Json;

namespace Smart_Factory_Management_System;

public class JsonRepository<T>(IFileSystemService fileService, string fileName) : IJsonRepository<T>
{
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = true
    };

    public List<T> Load()
    {
        var json = fileService.ReadFromFile(fileName);
        return string.IsNullOrEmpty(json)
            ? new List<T>()
            : JsonSerializer.Deserialize<List<T>>(json, _serializerOptions) ?? new List<T>();
    }

    public void Save(IEnumerable<T> items)
    {
        var json = JsonSerializer.Serialize(items, _serializerOptions);
        fileService.WriteToFile(fileName, json);
    }
}