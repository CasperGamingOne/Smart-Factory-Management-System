using System.Text;
using System.Text.Json;

namespace Smart_Factory_Management_System;

public class JsonRepository<T>(IFileSystemService fileService, string fileName) : IJsonRepository<T>
{
    //private const string AuthFileName = "employees.json";

    public List<T> Load()
    {
        var json = fileService.ReadFromFile(fileName);
        return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
    }

    public void Save(List<T> items)
    {
        var json = JsonSerializer.Serialize(items);
        fileService.WriteToFile(fileName, json);
    }

    public void ExportToCsv(string filePath)
    {
        var items = Load();
        var csv = new StringBuilder();

        // Use reflection to get property names for headers
        var properties = typeof(T).GetProperties();
        csv.AppendLine(string.Join(",", properties.Select(p => p.Name)));

        foreach (var item in items)
        {
            var values = properties.Select(p => p.GetValue(item)?.ToString() ?? "");
            csv.AppendLine(string.Join(",", values));
        }

        fileService.WriteToFile(filePath, csv.ToString());
    }
}