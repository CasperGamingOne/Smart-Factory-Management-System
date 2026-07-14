namespace Smart_Factory_Management_System;

public enum LogOrigin
{
    SYSTEM,
    USER
}

public enum LogEvent
{
    LoginSuccess,
    LoginFailed,
    MachineStarted,
    ProductionStarted,
    ProductionCompleted,
    MaintenancePerformed
}

public class LoggerService(IFileSystemService fileService) : ILoggerService
{
    private const string LogFileName = "operations.log";

    public void LogInfo(LogOrigin origin, LogEvent eventType, string context = "")
    {
        var message = eventType switch
        {
            LogEvent.LoginSuccess => $"[{origin}] Successful login: User '{context}'",
            LogEvent.LoginFailed => $"[{origin}] Failed login attempt: Username '{context}'",
            LogEvent.MachineStarted => $"[{origin}] Machine started: {context}",
            LogEvent.ProductionStarted => $"[{origin}] Production operation started: {context}",
            LogEvent.ProductionCompleted => $"[{origin}] Production operation completed: {context}",
            LogEvent.MaintenancePerformed => $"[{origin}] Maintenance performed on: {context}",
            _ => "Unknown event occurred"
        };

        WriteToFile("INFO", message);
    }

    public void LogWarning(string message)
    {
        WriteToFile("[WARN] [SYSTEM] ", message);
    }

    public void LogError(string message)
    {
        WriteToFile("[ERROR] [SYSTEM] ", message);
    }

    private void WriteToFile(string level, string message)
    {
        var entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
        fileService.AppendToFile(LogFileName, entry);
    }
}