namespace Smart_Factory_Management_System;

public class AccountService(IJsonRepository<Employee> repository) : IAccountService
{
    public bool UpdateFullName(Employee user, string newName)
    {
        if (string.IsNullOrWhiteSpace(newName)) throw new ArgumentException("Full Name cannot be empty.");

        user.UpdateName(newName);
        return PersistChanges(user);
    }

    public bool UpdateUsername(Employee user, string newUsername)
    {
        if (string.IsNullOrWhiteSpace(newUsername)) throw new ArgumentException("Username cannot be empty.");

        var users = repository.Load();
        if (users.Any(u => u.Id != user.Id && u.Username.Equals(newUsername, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("This username is already taken by another account.");

        user.UpdateUsername(newUsername);
        return PersistChanges(user);
    }

    public bool UpdatePassword(Employee user, string newPassword)
    {
        if (string.IsNullOrWhiteSpace(newPassword)) throw new ArgumentException("Password cannot be empty.");

        user.ChangePassword(SecurityHelper.HashPassword(newPassword));
        return PersistChanges(user);
    }

    private bool PersistChanges(Employee user)
    {
        var users = repository.Load();
        var dbUser = users.FirstOrDefault(u => u.Id == user.Id);
        if (dbUser != null)
        {
            dbUser.UpdateName(user.Name);
            dbUser.UpdateUsername(user.Username);
            dbUser.ChangePassword(user.PasswordHash);

            try
            {
                repository.Save(users);
                return true;
            }
            catch (IOException)
            {
                return false;
            }
        }

        return false;
    }
}