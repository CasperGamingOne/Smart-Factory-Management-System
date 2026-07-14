namespace Smart_Factory_Management_System;

public interface IAuthService
{
    // Your existing flawless auth logic goes behind this interface
    Employee Authenticate(string username, string rawPassword);
}

public interface IAuthRepository<T>
{
    List<T> LoadUsers();
    void SaveUsers(List<T> users);
}

public interface ILoggerService
{
    void LogInfo(LogOrigin origin, LogEvent eventType, string context = "");
    void LogWarning(LogOrigin origin, LogEvent eventType, string context = "");
    void LogError(string message);
}