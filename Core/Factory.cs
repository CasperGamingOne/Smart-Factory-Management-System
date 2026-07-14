namespace Smart_Factory_Management_System;

public class Factory
{
    // --- 1. THE CONSTRUCTOR HANDLES INITIAL DATA SEEDING ---
    public Factory()
    {
        SeedInitialData();
    }

    // --- 2. ADVANCED COLLECTIONS (Replacing Arrays) ---
    // Using Queue for Orders and Reports (First-In, First-Out)
    public Queue<ProductionOrder> PendingOrders { get; private set; } = new Queue<ProductionOrder>();
    public int OrderCount => PendingOrders.Count;

    public Queue<ReportRequest> PendingReportRequests { get; } = new();
    public int ReportRequestCount => PendingReportRequests.Count;

    // Using Lists for dynamic data storage
    public List<ProductionBatch> Batches { get; } = new List<ProductionBatch>();
    public int BatchCount => Batches.Count;

    public List<Employee> Employees { get; } = new List<Employee>();
    public int EmployeeCount => Employees.Count;

    public List<Machine> Machines { get; } = new List<Machine>();
    public int MachineCount => Machines.Count;

    public List<Product> Inventory { get; } = new List<Product>();
    public int ProductCount => Inventory.Count;

    // --- 3. SEEDING LOGIC ---
    private void SeedInitialData()
    {
        AddProduct(new Microprocessor("ARM Cortex-M4", 50, 74.99, 10, 4, 2.5));
        AddProduct(new Motherboard("Motherboard ATX", 50.00, 99.99, 5, "AM4", "ATX"));

        MachinePart litoPower = new PowerSupply("ASML High-Voltage Grid", PartCondition.Excellent, 400);
        MachinePart litoCooling = new CoolingSystem("CryoHelix Sub-Zero", PartCondition.Excellent, "Liquid Helium");
        MachinePart litoControl = new ControlUnit("LithoMaster Core", PartCondition.Excellent, "Intel Xeon Scalable");
        MachinePart litoAoi = new AoiSystem("Wafer Laser Interferometer", PartCondition.Excellent,
            "EUV Nano-Alignment Scanner");

        MachinePart[] litoParts = [litoPower, litoCooling, litoControl, litoAoi];

        MachinePart printerPower = new PowerSupply("Delta PSU-24V Module", PartCondition.Excellent, 24);
        MachinePart printerCooling =
            new CoolingSystem("Chassis Air Extraction", PartCondition.Excellent, "Forced Ambient Air");
        MachinePart printerControl = new ControlUnit("SolderPrint PLC Unit", PartCondition.Excellent, "ARM Cortex-M7");
        MachinePart printerAoi = new AoiSystem("Fiducial Alignment Cam", PartCondition.Excellent,
            "2D Solder Paste Inspection (SPI)");

        MachinePart[] printerParts = [printerPower, printerCooling, printerControl, printerAoi];

        MachinePart papPower = new PowerSupply("Omron Servo Power Rail", PartCondition.Excellent, 230);
        MachinePart papCooling = new CoolingSystem("Dual Fan Heat Sink", PartCondition.Excellent, "Active Air Cooling");
        MachinePart papControl =
            new ControlUnit("Gantry Motion Matrix Card", PartCondition.Excellent, "AMD Ryzen Embedded");
        MachinePart papAoi = new AoiSystem("High-Speed Flying Vision Module", PartCondition.Excellent,
            "Component Orientation 2D Camera");

        MachinePart[] papParts = [papPower, papCooling, papControl, papAoi];

        MachinePart ovenPower = new PowerSupply("Heavy Induction Heating Grid", PartCondition.Excellent, 415);
        MachinePart ovenCooling =
            new CoolingSystem("Nitrogen Exhaust Chiller", PartCondition.Excellent, "Forced Liquid Nitrogen");
        MachinePart ovenControl =
            new ControlUnit("Thermal Zone Governor", PartCondition.Excellent, "Siemens S7-1500 PLC");
        MachinePart ovenAoi = new AoiSystem("Exit Solder Defect Inspector", PartCondition.Excellent,
            "Post-Reflow Automated Optical Inspection");

        MachinePart[] ovenParts = [ovenPower, ovenCooling, ovenControl, ovenAoi];

        AddMachine(new LitographyMachine("LithoScan EUV-3600", "ASML", "SN-ASML-2024-88A9", litoParts,
            MachineCondition.Excellent));
        AddMachine(new SmtMachine("Horizon SolderPrinter X5", "DEK International", "SN-DEK-77492-B7", printerParts,
            MachineCondition.Excellent));
        AddMachine(new PaPMachine("NXT-III High-Speed Mounter", "Fuji Corporation", "SN-FUJI-991A-040", papParts,
            MachineCondition.Excellent));
        AddMachine(new ReflowOven("OmniMax Thermal Tunnel", "Heller Industries", "SN-HLR-5542-Z9", ovenParts,
            MachineCondition.Critical));
    }

    // --- 4. STREAMLINED ADD METHODS ---
    // With generic collections, you no longer need capacity checks (if count < length). 
    // They dynamically expand, wiping out an entire category of potential bugs!

    public void AddEmployee(Employee employee) => Employees.Add(employee);

    private void AddMachine(Machine machine)
    {
        Machines.Add(machine);
    }

    private void AddProduct(Product product)
    {
        Inventory.Add(product);
    }

    // Add product and track association with a production batch
    public void AddProduct(Product product, string? batchId)
    {
        Inventory.Add(product);
        int index = Inventory.Count - 1;

        if (!string.IsNullOrEmpty(batchId))
        {
            var batch = Batches.Find(b => b.BatchId == batchId);
            batch?.InventoryIndexes.Add(index);
        }
    }

    public void AddOrder(ProductionOrder order) => PendingOrders.Enqueue(order);

    public void AddReportRequest(ReportRequest request)
    {
        PendingReportRequests.Enqueue(request);
    }

    public void AddBatch(ProductionBatch batch)
    {
        Batches.Add(batch);
    }
}