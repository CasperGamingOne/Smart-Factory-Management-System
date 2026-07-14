namespace Smart_Factory_Management_System;

public interface IJsonRepository<T>
{
    List<T> Load();
    void Save(List<T> data);
}

public interface ILoggerService
{
    void LogInfo(LogOrigin origin, LogEvent eventType, string context = "");
    void LogWarning(LogOrigin origin, LogEvent eventType, string context = "");
    void LogError(string message);
}