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
                new Director("Andrei Popescu", "andrei", SecurityHelper.HashPassword("password"),
                    false),
                new Technician("Maria Ionescu", "maria", SecurityHelper.HashPassword("password"),
                    false),
                new SalesAgent("Alexandru Dumitru", "alex", SecurityHelper.HashPassword("password"),
                    false),
                new Accountant("Elena Vasilescu", "elena", SecurityHelper.HashPassword("password"),
                    true)
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
            MachinePart papControl =
                new ControlUnit("Gantry Motion Matrix Card", PartCondition.Excellent, "AMD Ryzen Embedded");
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

            var initialMachines = new List<Machine>
            {
                new LitographyMachine("LithoScan EUV-3600", "ASML", "SN-ASML-2024-88A9", litoParts,
                    MachineCondition.Excellent),
                new SmtMachine("Horizon SolderPrinter X5", "DEK International", "SN-DEK-77492-B7", printerParts,
                    MachineCondition.Excellent),
                new PaPMachine("NXT-III High-Speed Mounter", "Fuji Corporation", "SN-FUJI-991A-040", papParts,
                    MachineCondition.Excellent),
                new ReflowOven("OmniMax Thermal Tunnel", "Heller Industries", "SN-HLR-5542-Z9", ovenParts,
                    MachineCondition.Critical)
            };
            machineRepo.Save(initialMachines);
        }

        if (!File.Exists(productsFile))
        {
            var initialProducts = new List<Product>
            {
                new Microprocessor("ARM Cortex-M4", 50, 74.99, 10, 4, 2.5),
                new Motherboard("Motherboard ATX", 50.00, 99.99, 5, "AM4", "ATX")
            };
            productRepo.Save(initialProducts);
        }
    }
}