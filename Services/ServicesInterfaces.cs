namespace Smart_Factory_Management_System;

public interface IJsonRepository<T>
{
    List<T> Load();
    void Save(IEnumerable<T> data);
}

public interface INotificationService
{
    void CheckAndShowNotifications(Employee user, Factory factory);
}

public interface ILoggerService
{
    void LogInfo(LogOrigin origin, LogEvent eventType, string context = "");
    void LogWarning(LogOrigin origin, LogEvent eventType, string context = "");
    void LogError(string message);
    void ShowOperationHistory();
}

public interface IAccountService
{
    bool UpdateFullName(Employee user, string newName);
    bool UpdateUsername(Employee user, string newUsername);
    bool UpdatePassword(Employee user, string newPassword);
}