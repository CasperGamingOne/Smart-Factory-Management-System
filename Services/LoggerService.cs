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
    Logout,
    ProductionStarted,
    ProductionInterrupted,
    ProductionCompleted,
    MaintenancePerformed,
    ClosingApplication
}

public class LoggerService(IFileSystemService fileService) : ILoggerService
{
    private const string LogFileName = "operations.log";

    public void LogInfo(LogOrigin origin, LogEvent eventType, string? context = "")
    {
        var message = eventType switch
        {
            LogEvent.LoginSuccess => $"[{origin}] Successful login: User '{context}'",
            LogEvent.LoginFailed => $"[{origin}] Failed login attempt: Username '{context}'",
            LogEvent.Logout => $"[{origin}] User {context} logged out.",
            LogEvent.ProductionStarted => $"[{origin}] Production operation started for {context}",
            LogEvent.ProductionCompleted => $"[{origin}] Production operation completed for {context}",
            LogEvent.MaintenancePerformed => $"[{origin}] Maintenance performed on the machine {context}",
            LogEvent.ClosingApplication => $"[{origin}] Closing application...",
            _ => "Unknown event occurred"
        };

        WriteToFile("INFO", message);
    }

    public void LogWarning(LogOrigin origin, LogEvent eventType, string context = "")
    {
        var message = eventType switch
        {
            LogEvent.ProductionInterrupted => $"[{origin}] Production operation interrupted for {context}",
            _ => "Unknown event occurred"
        };

        WriteToFile("WARN", message);
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