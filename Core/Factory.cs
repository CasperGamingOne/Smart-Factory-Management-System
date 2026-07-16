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
    public int InventoryCapacity => _inventory.Capacity;

    public void LoadFromRepository(IEnumerable<Employee> employees, IEnumerable<Machine> machines,
        IEnumerable<Product> products)
    {
        var employeeList = employees.ToList();
        var machineList = machines.ToList();
        var productList = products.ToList();

        _employees.AddRange(employeeList);
        _machines.AddRange(machineList);
        _inventory.AddRange(productList);

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

    public void RemoveProduct(Product product)
    {
        _inventory.Remove(product);
    }

    public void RemoveBatch(ProductionBatch batch)
    {
        _batches.Remove(batch);
    }
}