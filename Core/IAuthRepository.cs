namespace Smart_Factory_Management_System;

public interface IAuthRepository<T> where T : Employee
{
    List<T> LoadUsers();
    void SaveUsers(List<T> users);
}