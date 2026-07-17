namespace Smart_Factory_Management_System;

public class DataSeeder(
    IJsonRepository<Employee> employeeRepo,
    IJsonRepository<Machine> machineRepo,
    IJsonRepository<Product> productRepo,
    IFileSystemService fileSystem)
{
    public void Seed()
    {
        var employeesFile = Path.Combine(fileSystem.BaseDirectory, "employees.json");
        var machinesFile = Path.Combine(fileSystem.BaseDirectory, "machines.json");
        var productsFile = Path.Combine(fileSystem.BaseDirectory, "products.json");

        if (!File.Exists(employeesFile))
        {
            var initialEmployees = new List<Employee>
            {
                new Director("Andrei Popescu", "andrei", SecurityHelper.HashPassword("password")),
                new Technician("Maria Ionescu", "maria", SecurityHelper.HashPassword("password")),
                new SalesAgent("Alexandru Dumitru", "alex", SecurityHelper.HashPassword("password")),
                new Accountant("Elena Vasilescu", "elena", SecurityHelper.HashPassword("password"))
            };
            employeeRepo.Save(initialEmployees);
        }

        if (!File.Exists(machinesFile))
        {
            MachinePart litoPower = new PowerSupply("ASML High-Voltage Grid", PartCondition.Excellent, 400);
            MachinePart litoCooling = new CoolingSystem("CryoHelix Sub-Zero", PartCondition.Excellent, "Liquid Helium");
            MachinePart litoControl =
                new ControlUnit("LithoMaster Core", PartCondition.Excellent, "Intel Xeon Scalable");
            MachinePart litoAoi = new AoiSystem("Wafer Laser Interferometer", PartCondition.Excellent,
                "EUV Nano-Alignment Scanner");

            List<MachinePart> litoParts = [litoPower, litoCooling, litoControl, litoAoi];

            MachinePart printerPower = new PowerSupply("Delta PSU-24V Module", PartCondition.Excellent, 24);
            MachinePart printerCooling =
                new CoolingSystem("Chassis Air Extraction", PartCondition.Excellent, "Forced Ambient Air");
            MachinePart printerControl =
                new ControlUnit("SolderPrint PLC Unit", PartCondition.Excellent, "ARM Cortex-M7");
            MachinePart printerAoi = new AoiSystem("Fiducial Alignment Cam", PartCondition.Excellent,
                "2D Solder Paste Inspection (SPI)");

            List<MachinePart> printerParts = [printerPower, printerCooling, printerControl, printerAoi];

            MachinePart papPower = new PowerSupply("Omron Servo Power Rail", PartCondition.Excellent, 230);
            MachinePart papCooling =
                new CoolingSystem("Dual Fan Heat Sink", PartCondition.Excellent, "Active Air Cooling");
            MachinePart papControl =
                new ControlUnit("Gantry Motion Matrix Card", PartCondition.Excellent, "AMD Ryzen Embedded");
            MachinePart papAoi = new AoiSystem("High-Speed Flying Vision Module", PartCondition.Excellent,
                "Component Orientation 2D Camera");

            List<MachinePart> papParts = [papPower, papCooling, papControl, papAoi];

            MachinePart ovenPower = new PowerSupply("Heavy Induction Heating Grid", PartCondition.Excellent, 415);
            MachinePart ovenCooling = new CoolingSystem("Nitrogen Exhaust Chiller", PartCondition.Excellent,
                "Forced Liquid Nitrogen");
            MachinePart ovenControl =
                new ControlUnit("Thermal Zone Governor", PartCondition.Excellent, "Siemens S7-1500 PLC");
            MachinePart ovenAoi = new AoiSystem("Exit Solder Defect Inspector", PartCondition.Excellent,
                "Post-Reflow Automated Optical Inspection");

            List<MachinePart> ovenParts = [ovenPower, ovenCooling, ovenControl, ovenAoi];

            var initialMachines = new List<Machine>
            {
                new LitographyMachine("LithoScan EUV-3600", "ASML", "SN-ASML-2024-88A9", litoParts,
                    MachineCondition.Excellent),
                new SmtMachine("Horizon SolderPrinter X5", "DEK International", "SN-DEK-77492-B7", printerParts,
                    MachineCondition.Excellent),
                new PaPMachine("NXT-III High-Speed Mounter", "Fuji Corporation", "SN-FUJI-991A-040", papParts,
                    MachineCondition.Excellent),
                new ReflowOven("OmniMax Thermal Tunnel", "Heller Industries", "SN-HLR-5542-Z9", ovenParts,
                    MachineCondition.Excellent)
            };
            machineRepo.Save(initialMachines);
        }

        if (!File.Exists(productsFile))
        {
            var initialProducts = new List<Product>
            {
                // Unsold stock (Total: 70 units - triggers the < 200 unit alert banner demo)
                new Microprocessor("ARM Cortex-M4", 50.00, 0, 10, 4, 2.5) { BatchId = "BATCH-01", IsSold = false },
                new Microprocessor("Intel Core i5", 120.00, 0, 15, 6, 3.2) { BatchId = "BATCH-02", IsSold = false },
                new Microprocessor("AMD Ryzen 5", 110.00, 0, 20, 6, 3.6) { BatchId = "BATCH-03", IsSold = false },
                new Motherboard("Motherboard ATX", 50.00, 0, 5, "AM4", "ATX") { BatchId = "BATCH-04", IsSold = false },
                new Motherboard("Asus ROG Strix", 150.00, 0, 8, "LGA1700", "ATX")
                    { BatchId = "BATCH-05", IsSold = false },
                new Motherboard("MSI Tomahawk", 90.00, 0, 12, "AM5", "ATX") { BatchId = "BATCH-06", IsSold = false },

                // Pre-sold history (for revenue/financial dashboard demonstration)
                new Microprocessor("ARM Cortex-M4", 50.00, 74.99, 50, 4, 2.5) { BatchId = "BATCH-S1", IsSold = true },
                new Microprocessor("Intel Core i5", 120.00, 199.99, 30, 6, 3.2) { BatchId = "BATCH-S2", IsSold = true },
                new Motherboard("Motherboard ATX", 50.00, 99.99, 25, "AM4", "ATX")
                    { BatchId = "BATCH-S3", IsSold = true }
            };
            productRepo.Save(initialProducts);
        }
    }
}