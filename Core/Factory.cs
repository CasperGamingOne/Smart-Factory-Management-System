namespace Smart_Factory_Management_System;

public class Factory
{
    // Tracking counters stay private and safe

    // --- 1. THE CONSTRUCTOR HANDLES INITIAL DATA SEEDING ---
    public Factory()
    {
        SeedInitialData();
    }

    //public ProductionOrder[] PendingOrders { get; } = new ProductionOrder[50];
    public Queue<ProductionOrder> PendingOrders { get; private set; } = new Queue<ProductionOrder>();
    public int OrderCount => PendingOrders.Count;//public int OrderCount { get; private set; }

    // Track finished production batches for accounting and sales
    //public ProductionBatch[] Batches { get; } = new ProductionBatch[100];
    public List<ProductionBatch> Batches { get; } = new List<ProductionBatch>();
    public int BatchCount => Batches.Count; //public int BatchCount { get; private set; }

    // Core bounded arrays
    public List<Employee> Employees { get; } = new List<Employee>();
    public int EmployeeCount => Employees.Count;

    public List<Machine> Machines { get; } = new List<Machine>();
    public int MachineCount => Machines.Count;

    public List<Product> Inventory { get; } = new List<Product>();
    public int ProductCount => Inventory.Count;


    private void SeedInitialData()
    {
        AddEmployee(new Director("Andrei Popescu"));
        AddEmployee(new Technician("Maria Ionescu"));
        AddEmployee(new SalesAgent("Alexandru Dumitru"));
        AddEmployee(new Accountant("Elena Vasilescu"));

        AddProduct(new Microprocessor("ARM Cortex-M4", 50, 74.99, 10, 4, 2.5));
        AddProduct(new Motherboard("Motherboard ATX", 50.00, 99.99, 5, "AM4", "ATX"));

        MachinePart lito_power = new Power_Supply("ASML High-Voltage Grid", PartCondition.Excellent, 400);
        MachinePart lito_cooling =
            new Cooling_System("CryoHelix Sub-Zero", PartCondition.Excellent, "Liquid Helium");
        MachinePart lito_control =
            new Control_Unit("LithoMaster Core", PartCondition.Excellent, "Intel Xeon Scalable");
        MachinePart lito_AOI = new AOI_System("Wafer Laser Interferometer", PartCondition.Excellent,
            "EUV Nano-Alignment Scanner");

        MachinePart[] lito_parts = { lito_power, lito_cooling, lito_control, lito_AOI };

        MachinePart printer_power = new Power_Supply("Delta PSU-24V Module", PartCondition.Excellent, 24);
        MachinePart printer_cooling =
            new Cooling_System("Chassis Air Extraction", PartCondition.Excellent, "Forced Ambient Air");
        MachinePart printer_control =
            new Control_Unit("SolderPrint PLC Unit", PartCondition.Excellent, "ARM Cortex-M7");
        MachinePart printer_AOI = new AOI_System("Fiducial Alignment Cam", PartCondition.Excellent,
            "2D Solder Paste Inspection (SPI)");

        MachinePart[] printer_parts = { printer_power, printer_cooling, printer_control, printer_AOI };

        MachinePart pap_power = new Power_Supply("Omron Servo Power Rail", PartCondition.Excellent, 230);
        MachinePart pap_cooling =
            new Cooling_System("Dual Fan Heat Sink", PartCondition.Excellent, "Active Air Cooling");
        MachinePart pap_control = new Control_Unit("Gantry Motion Matrix Card", PartCondition.Excellent,
            "AMD Ryzen Embedded");
        MachinePart pap_AOI = new AOI_System("High-Speed Flying Vision Module", PartCondition.Excellent,
            "Component Orientation 2D Camera");

        MachinePart[] pap_parts = { pap_power, pap_cooling, pap_control, pap_AOI };

        MachinePart oven_power = new Power_Supply("Heavy Induction Heating Grid", PartCondition.Excellent, 415);
        MachinePart oven_cooling = new Cooling_System("Nitrogen Exhaust Chiller", PartCondition.Excellent,
            "Forced Liquid Nitrogen");
        MachinePart oven_control =
            new Control_Unit("Thermal Zone Governor", PartCondition.Excellent, "Siemens S7-1500 PLC");
        MachinePart oven_AOI = new AOI_System("Exit Solder Defect Inspector", PartCondition.Excellent,
            "Post-Reflow Automated Optical Inspection");

        MachinePart[] oven_parts = { oven_power, oven_cooling, oven_control, oven_AOI };

        AddMachine(new LitographyMachine("LithoScan EUV-3600", "ASML", "SN-ASML-2024-88A9", lito_parts,
            MachineCondition.Excellent));
        AddMachine(new SmtMachine("Horizon SolderPrinter X5", "DEK International", "SN-DEK-77492-B7",
            printer_parts, MachineCondition.Excellent));
        AddMachine(new PaPMachine("NXT-III High-Speed Mounter", "Fuji Corporation", "SN-FUJI-991A-040", pap_parts,
            MachineCondition.Excellent));
        AddMachine(new ReflowOven("OmniMax Thermal Tunnel", "Heller Industries", "SN-HLR-5542-Z9", oven_parts,
            MachineCondition.Critical));
    }

    //public void AddEmployee(Employee employee)
    //{
    //    if (EmployeeCount < Employees.Length)
    //    {
    //        Employees[EmployeeCount] = employee;
    //        EmployeeCount++;
    //    }
    //}
    public void AddEmployee(Employee employee) => Employees.Add(employee);

    public void AddMachine(Machine machine) => Machines.Add(machine);

    public void AddProduct(Product product) => Inventory.Add(product);

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

    //public void AddOrder(ProductionOrder order)
    //{
    //    if (OrderCount < PendingOrders.Length)
    //    {
    //        PendingOrders[OrderCount] = order;
    //        OrderCount++;
    //    }
    //}

    public void AddOrder(ProductionOrder order) => PendingOrders.Enqueue(order);

    public void AddBatch(ProductionBatch batch) => Batches.Add(batch);
}
