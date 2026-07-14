namespace Smart_Factory_Management_System;

public interface IFileSystemService
{
    string BaseDirectory { get; }
    void WriteToFile(string fileName, string content);
    string ReadFromFile(string fileName);
    void AppendToFile(string fileName, string content);
}

public class FileSystemService : IFileSystemService
{
    public FileSystemService(string folderName = "SmartFactoryData")
    {
        // 1. Get the path to 'My Documents'
        var docsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        // 2. Create the full path: Documents/SmartFactoryData
        BaseDirectory = Path.Combine(docsPath, folderName);

        // 3. Ensure it exists
        if (!Directory.Exists(BaseDirectory)) Directory.CreateDirectory(BaseDirectory);
    }

    public string BaseDirectory { get; }

    public void WriteToFile(string fileName, string content)
    {
        File.WriteAllText(Path.Combine(BaseDirectory, fileName), content);
    }

    public string ReadFromFile(string fileName)
    {
        var path = Path.Combine(BaseDirectory, fileName);
        return File.Exists(path) ? File.ReadAllText(path) : string.Empty;
    }

    public void AppendToFile(string fileName, string content)
    {
        File.AppendAllText(Path.Combine(BaseDirectory, fileName), content + Environment.NewLine);
    }
}