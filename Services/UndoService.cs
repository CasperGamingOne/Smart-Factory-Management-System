namespace Smart_Factory_Management_System;

public interface ICommand
{
    string Description { get; }
    string ExecutedBy { get; }
    DateTime ExecutedAt { get; }
    void Undo();
}

public interface IUndoService
{
    void RegisterCommand(ICommand command);
    IReadOnlyList<ICommand> GetUndoableCommands(Employee user);
    void UndoCommand(ICommand command);
}

public class UndoService : IUndoService
{
    private static readonly List<ICommand> Commands = new();

    private UndoService()
    {
    }

    public static UndoService Instance { get; } = new();

    public void RegisterCommand(ICommand command)
    {
        Commands.Add(command);
        while (Commands.Count > 10) Commands.RemoveAt(0);
    }

    public IReadOnlyList<ICommand> GetUndoableCommands(Employee user)
    {
        if (user is Director) return Commands;

        return Commands.FindAll(c => c.ExecutedBy.Equals(user.Username, StringComparison.OrdinalIgnoreCase));
    }

    public void UndoCommand(ICommand command)
    {
        command.Undo();
        Commands.Remove(command);
    }
}

public class ChangeFullNameCommand(
    Employee employee,
    string oldName,
    string newName,
    IAccountService accountService,
    string executedBy) : ICommand
{
    public string Description => $"Changed full name of '{employee.Username}' from '{oldName}' to '{newName}'";
    public string ExecutedBy => executedBy;
    public DateTime ExecutedAt { get; } = DateTime.Now;

    public void Undo()
    {
        accountService.UpdateFullName(employee, oldName);
    }
}

public class ChangeUsernameCommand(
    Employee employee,
    string oldUsername,
    string newUsername,
    IAccountService accountService,
    string executedBy) : ICommand
{
    public string Description => $"Changed username of '{oldUsername}' to '{newUsername}'";
    public string ExecutedBy => executedBy;
    public DateTime ExecutedAt { get; } = DateTime.Now;

    public void Undo()
    {
        accountService.UpdateUsername(employee, oldUsername);
    }
}

public class SellFromBatchCommand(
    ProductionBatch batch,
    Factory factory,
    double soldPrice,
    List<int> inventoryIndexes,
    IJsonRepository<Product> productRepo,
    string executedBy) : ICommand
{
    public string Description => $"Sold batch '{batch.ProductName}' (ID: {batch.BatchId}) at ${soldPrice:F2}/unit";
    public string ExecutedBy => executedBy;
    public DateTime ExecutedAt { get; } = DateTime.Now;

    public void Undo()
    {
        batch.MarkAsUnsold();
        batch.SetUnitSellPrice(0);
        foreach (var idx in inventoryIndexes)
            if (idx >= 0 && idx < factory.Inventory.Count)
            {
                factory.Inventory[idx].UpdateSellingPrice(0);
                factory.Inventory[idx].IsSold = false;
            }

        productRepo.Save(factory.Inventory);
    }
}

public class SellFromInventoryCommand(
    Product chosen,
    int qty,
    double soldPrice,
    Product soldProduct,
    ProductionBatch batch,
    Factory factory,
    IJsonRepository<Product> productRepo,
    string executedBy) : ICommand
{
    public string Description => $"Sold {qty} units of '{chosen.Name}' from inventory at ${soldPrice:F2}/unit";
    public string ExecutedBy => executedBy;
    public DateTime ExecutedAt { get; } = DateTime.Now;

    public void Undo()
    {
        chosen.AddQuantity(qty);
        factory.RemoveProduct(soldProduct);
        factory.RemoveBatch(batch);

        var originalBatch = factory.Batches.FirstOrDefault(b => b.BatchId == chosen.BatchId);
        if (originalBatch != null)
        {
            originalBatch.Quantity += qty;
            originalBatch.MarkAsUnsold();
        }

        productRepo.Save(factory.Inventory);
    }
}

public class RemoveEmployeeCommand(
    Employee employee,
    Factory factory,
    IJsonRepository<Employee> employeeRepo,
    string executedBy) : ICommand
{
    public string Description => $"Removed employee '{employee.Name}' ({employee.Role})";
    public string ExecutedBy => executedBy;
    public DateTime ExecutedAt { get; } = DateTime.Now;

    public void Undo()
    {
        factory.AddEmployee(employee);

        var users = employeeRepo.Load();
        if (!users.Any(u => u.Id == employee.Id))
        {
            users.Add(employee);
            employeeRepo.Save(users);
        }
    }
}