using Spectre.Console;

namespace Smart_Factory_Management_System;

public class Factory
{
    private readonly List<ProductionBatch> _batches = new();

    private readonly List<Employee> _employees = new();

    private readonly List<Product> _inventory = new();

    private readonly List<Machine> _machines = new();

    private readonly Queue<ProductionOrder> _pendingOrders = new();

    private readonly Queue<ReportRequest> _pendingReportRequests = new();

    public IEnumerable<ProductionOrder> PendingOrders => _pendingOrders;
    public int OrderCount => _pendingOrders.Count;
    public IEnumerable<ReportRequest> PendingReportRequests => _pendingReportRequests;
    public int ReportRequestCount => _pendingReportRequests.Count;
    public IReadOnlyList<ProductionBatch> Batches => _batches;
    public int BatchCount => _batches.Count;
    public IReadOnlyList<Employee> Employees => _employees;
    public IReadOnlyList<Machine> Machines => _machines;
    public IReadOnlyList<Product> Inventory => _inventory;
    public int MaxBatches => 20;
    private int MaxUnitsPerBatch => 100;
    public int MaxCapacity => MaxBatches * MaxUnitsPerBatch;
    public int MinStockThreshold => 200;

    public int GetTotalUnsoldUnits()
    {
        return _batches.Where(b => !b.IsSold).Sum(b => b.Quantity);
    }

    public void LoadFromRepository(IEnumerable<Employee> employees, IEnumerable<Machine> machines,
        IEnumerable<Product> products, IEnumerable<ProductionOrder> orders, IEnumerable<ReportRequest> reportRequests)
    {
        // Reset counters to 0 BEFORE materializing the lists.
        // JsonSerializer calls the real constructor for each deserialized object, which increments
        // the static counter. Without this reset the counter would be inflated by the load itself,
        // causing the next genuinely new entity to receive a wrong (too-high) ID.
        Employee.InitializeIdCounter(0);
        Machine.InitializeIdCounter(0);

        var employeeList = employees.ToList();
        var machineList = machines.ToList();
        var productList = products.ToList();

        _employees.AddRange(employeeList);
        _machines.AddRange(machineList);
        _inventory.AddRange(productList);

        _pendingOrders.Clear();
        foreach (var order in orders) _pendingOrders.Enqueue(order);

        _pendingReportRequests.Clear();
        foreach (var request in reportRequests) _pendingReportRequests.Enqueue(request);

        // Reconstruct batches from the loaded products
        var productsByBatch = productList.Where(p => !string.IsNullOrEmpty(p.BatchId)).GroupBy(p => p.BatchId);
        foreach (var group in productsByBatch)
        {
            var firstProduct = group.First();
            var totalQuantity = group.Sum(p => p.Quantity);
            var totalCost = group.Sum(p => p.ProductionCost * p.Quantity);
            var unitCost = totalQuantity > 0 ? totalCost / totalQuantity : firstProduct.ProductionCost;

            var batch = new ProductionBatch(firstProduct.Name ?? "Loaded Product", totalQuantity, unitCost, group.Key);

            var maxSellingPrice = group.Max(p => p.SellingPrice);
            if (maxSellingPrice > 0) batch.SetUnitSellPrice(maxSellingPrice);

            // If all products in the batch are marked as sold, the batch is sold
            if (group.All(p => p.IsSold)) batch.MarkAsSold();

            // Link matching product indexes to the batch
            for (var i = 0; i < productList.Count; i++)
                if (productList[i].BatchId == group.Key)
                    batch.AddInventoryIndex(i);

            _batches.Add(batch);
        }

        // Now set counters to the highest persisted ID so the next new entity continues from there.
        if (employeeList.Count > 0)
        {
            var maxEmployeeId = employeeList.Max(e => e.Id);
            Employee.InitializeIdCounter(maxEmployeeId);
        }

        if (machineList.Count > 0)
        {
            var maxMachineId = machineList.Max(m => m.Id);
            Machine.InitializeIdCounter(maxMachineId);
        }
    }

    public void AddEmployee(Employee employee)
    {
        _employees.Add(employee);
    }

    public void AddProduct(Product product, string? batchId)
    {
        product.BatchId = batchId;

        if (!string.IsNullOrEmpty(batchId))
        {
            var existing = _inventory.FirstOrDefault(p =>
                p.BatchId == batchId && p.GetType() == product.GetType() && p.Name == product.Name &&
                p.IsSold == product.IsSold);
            if (existing != null)
            {
                existing.AddQuantity(product.Quantity);
                return;
            }
        }

        _inventory.Add(product);
        var index = _inventory.Count - 1;

        if (!string.IsNullOrEmpty(batchId))
        {
            var batch = _batches.Find(b => b.BatchId == batchId);
            batch?.AddInventoryIndex(index);
        }
    }

    public void AddOrder(ProductionOrder order)
    {
        _pendingOrders.Enqueue(order);
        var prioritized = _pendingOrders.OrderByDescending(o => o.GetPriorityScore()).ToList();
        _pendingOrders.Clear();
        foreach (var o in prioritized) _pendingOrders.Enqueue(o);
    }

    public void AddReportRequest(ReportRequest request)
    {
        _pendingReportRequests.Enqueue(request);
    }

    public void AddBatch(ProductionBatch batch)
    {
        _batches.Add(batch);
    }

    public static void ShowInventoryAlerts(IEnumerable<Product> inventory)
    {
        // Use the helper method defined in Product.cs
        var lowStockItems = inventory.Where(p => p.IsLowStock()).ToList();

        if (lowStockItems.Count == 0) return;
        var table = new Table().Border(TableBorder.Rounded);
        table.AddColumn("[red]Status[/]");
        table.AddColumn("Product");
        table.AddColumn("Current Stock");

        foreach (var item in lowStockItems)
            if (item.Name != null)
                table.AddRow("⚠️", item.Name, $"[bold red]{item.Quantity}[/]");

        AnsiConsole.Write(new Panel(table)
        {
            Header = new PanelHeader("[bold red] INVENTORY ALERT [/]"),
            Border = BoxBorder.Double
        });
    }

    public void RemoveProduct(Product product)
    {
        _inventory.Remove(product);
    }

    public void RemoveBatch(ProductionBatch batch)
    {
        _batches.Remove(batch);
    }

    public void RemoveEmployee(Employee employee)
    {
        _employees.Remove(employee);
    }
}