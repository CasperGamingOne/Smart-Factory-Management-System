namespace Smart_Factory_Management_System;

public class Factory
{
    // Tracking counters stay private and safe

    // --- 1. THE CONSTRUCTOR HANDLES INITIAL DATA SEEDING ---
    public Factory()
    {
        SeedInitialData();
    }

    public ProductionOrder[] PendingOrders { get; } = new ProductionOrder[50];
    public int OrderCount { get; private set; }

    public ReportRequest[] PendingReportRequests { get; } = new ReportRequest[50];
    public int ReportRequestCount { get; private set; }

    // Track finished production batches for accounting and sales
    public ProductionBatch[] Batches { get; } = new ProductionBatch[100];
    public int BatchCount { get; private set; }

    // Core bounded arrays
    public Employee[] Employees { get; } = new Employee[100];
    public Machine[] Machines { get; } = new Machine[50];
    public Product[] Inventory { get; } = new Product[200];

    public int EmployeeCount { get; private set; }

    public int MachineCount { get; private set; }

    public int ProductCount { get; private set; }

    private void SeedInitialData()
    {
        AddEmployee(new Director("Andrei Popescu"));
        AddEmployee(new Technician("Maria Ionescu"));
        AddEmployee(new SalesAgent("Alexandru Dumitru"));
        AddEmployee(new Accountant("Elena Vasilescu"));

        AddProduct(new Microprocessor("ARM Cortex-M4", 50, 74.99, 10, 4, 2.5));
        AddProduct(new Motherboard("Motherboard ATX", 50.00, 99.99, 5, "AM4", "ATX"));

        MachinePart litoPower = new PowerSupply("ASML High-Voltage Grid", PartCondition.Excellent, 400);
        MachinePart litoCooling =
            new CoolingSystem("CryoHelix Sub-Zero", PartCondition.Excellent, "Liquid Helium");
        MachinePart litoControl =
            new ControlUnit("LithoMaster Core", PartCondition.Excellent, "Intel Xeon Scalable");
        MachinePart litoAoi = new AoiSystem("Wafer Laser Interferometer", PartCondition.Excellent,
            "EUV Nano-Alignment Scanner");

        MachinePart[] litoParts = [litoPower, litoCooling, litoControl, litoAoi];

        MachinePart printerPower = new PowerSupply("Delta PSU-24V Module", PartCondition.Excellent, 24);
        MachinePart printerCooling =
            new CoolingSystem("Chassis Air Extraction", PartCondition.Excellent, "Forced Ambient Air");
        MachinePart printerControl =
            new ControlUnit("SolderPrint PLC Unit", PartCondition.Excellent, "ARM Cortex-M7");
        MachinePart printerAoi = new AoiSystem("Fiducial Alignment Cam", PartCondition.Excellent,
            "2D Solder Paste Inspection (SPI)");

        MachinePart[] printerParts = [printerPower, printerCooling, printerControl, printerAoi];

        MachinePart papPower = new PowerSupply("Omron Servo Power Rail", PartCondition.Excellent, 230);
        MachinePart papCooling =
            new CoolingSystem("Dual Fan Heat Sink", PartCondition.Excellent, "Active Air Cooling");
        MachinePart papControl = new ControlUnit("Gantry Motion Matrix Card", PartCondition.Excellent,
            "AMD Ryzen Embedded");
        MachinePart papAoi = new AoiSystem("High-Speed Flying Vision Module", PartCondition.Excellent,
            "Component Orientation 2D Camera");

        MachinePart[] papParts = [papPower, papCooling, papControl, papAoi];

        MachinePart ovenPower = new PowerSupply("Heavy Induction Heating Grid", PartCondition.Excellent, 415);
        MachinePart ovenCooling = new CoolingSystem("Nitrogen Exhaust Chiller", PartCondition.Excellent,
            "Forced Liquid Nitrogen");
        MachinePart ovenControl =
            new ControlUnit("Thermal Zone Governor", PartCondition.Excellent, "Siemens S7-1500 PLC");
        MachinePart ovenAoi = new AoiSystem("Exit Solder Defect Inspector", PartCondition.Excellent,
            "Post-Reflow Automated Optical Inspection");

        MachinePart[] ovenParts = [ovenPower, ovenCooling, ovenControl, ovenAoi];

        AddMachine(new LitographyMachine("LithoScan EUV-3600", "ASML", "SN-ASML-2024-88A9", litoParts,
            MachineCondition.Excellent));
        AddMachine(new SmtMachine("Horizon SolderPrinter X5", "DEK International", "SN-DEK-77492-B7",
            printerParts, MachineCondition.Excellent));
        AddMachine(new PaPMachine("NXT-III High-Speed Mounter", "Fuji Corporation", "SN-FUJI-991A-040", papParts,
            MachineCondition.Excellent));
        AddMachine(new ReflowOven("OmniMax Thermal Tunnel", "Heller Industries", "SN-HLR-5542-Z9", ovenParts,
            MachineCondition.Critical));
    }

    public void AddEmployee(Employee employee)
    {
        if (EmployeeCount < Employees.Length)
        {
            Employees[EmployeeCount] = employee;
            EmployeeCount++;
        }
    }

    private void AddMachine(Machine machine)
    {
        if (MachineCount < Machines.Length)
        {
            Machines[MachineCount] = machine;
            MachineCount++;
        }
    }

    private void AddProduct(Product product)
    {
        if (ProductCount < Inventory.Length)
        {
            Inventory[ProductCount] = product;
            ProductCount++;
        }
    }

    // Add product and track association with a production batch
    public void AddProduct(Product product, string? batchId)
    {
        if (ProductCount < Inventory.Length)
        {
            Inventory[ProductCount] = product;

            // If a batch id is provided, find the batch and record the inventory index
            if (!string.IsNullOrEmpty(batchId))
                for (var i = 0; i < BatchCount; i++)
                    if (Batches[i].BatchId == batchId)
                    {
                        Batches[i].InventoryIndexes.Add(ProductCount);
                        break;
                    }

            ProductCount++;
        }
    }

    public void AddOrder(ProductionOrder order)
    {
        if (OrderCount < PendingOrders.Length)
        {
            PendingOrders[OrderCount] = order;
            OrderCount++;
        }
    }

    public void AddReportRequest(ReportRequest request)
    {
        if (ReportRequestCount < PendingReportRequests.Length)
        {
            PendingReportRequests[ReportRequestCount] = request;
            ReportRequestCount++;
        }
    }

    public void AddBatch(ProductionBatch batch)
    {
        if (BatchCount < Batches.Length)
        {
            Batches[BatchCount] = batch;
            BatchCount++;
        }
    }
}