using System.Reflection.PortableExecutable;

namespace Smart_Factory_Management_System
{
    internal class Program
    {
        static void Main(string[] args)
        {

            MachinePart lito_power = new Power_Supply("ASML High-Voltage Grid", PartCondition.Excellent, 400);
            MachinePart lito_cooling = new Cooling_System("CryoHelix Sub-Zero", PartCondition.Excellent, "Liquid Helium");
            MachinePart lito_control = new Control_Unit("LithoMaster Core", PartCondition.Excellent, "Intel Xeon Scalable");
            MachinePart lito_AOI = new AOI_System("Wafer Laser Interferometer", PartCondition.Excellent, "EUV Nano-Alignment Scanner");

            MachinePart[] lito_parts = { lito_power, lito_cooling, lito_control, lito_AOI };

            MachinePart printer_power = new Power_Supply("Delta PSU-24V Module", PartCondition.Excellent, 24);
            MachinePart printer_cooling = new Cooling_System("Chassis Air Extraction", PartCondition.Excellent, "Forced Ambient Air");
            MachinePart printer_control = new Control_Unit("SolderPrint PLC Unit", PartCondition.Excellent, "ARM Cortex-M7");
            MachinePart printer_AOI = new AOI_System("Fiducial Alignment Cam", PartCondition.Excellent, "2D Solder Paste Inspection (SPI)");

            MachinePart[] printer_parts = { printer_power, printer_cooling, printer_control, printer_AOI };

            MachinePart pap_power = new Power_Supply("Omron Servo Power Rail", PartCondition.Excellent, 230);
            MachinePart pap_cooling = new Cooling_System("Dual Fan Heat Sink", PartCondition.Excellent, "Active Air Cooling");
            MachinePart pap_control = new Control_Unit("Gantry Motion Matrix Card", PartCondition.Excellent, "AMD Ryzen Embedded");
            MachinePart pap_AOI = new AOI_System("High-Speed Flying Vision Module", PartCondition.Excellent, "Component Orientation 2D Camera");

            MachinePart[] pap_parts = { pap_power, pap_cooling, pap_control, pap_AOI };

            MachinePart oven_power = new Power_Supply("Heavy Induction Heating Grid", PartCondition.Excellent, 415);
            MachinePart oven_cooling = new Cooling_System("Nitrogen Exhaust Chiller", PartCondition.Excellent, "Forced Liquid Nitrogen");
            MachinePart oven_control = new Control_Unit("Thermal Zone Governor", PartCondition.Excellent, "Siemens S7-1500 PLC");
            MachinePart oven_AOI = new AOI_System("Exit Solder Defect Inspector", PartCondition.Excellent, "Post-Reflow Automated Optical Inspection");

            MachinePart[] oven_parts = { oven_power, oven_cooling, oven_control, oven_AOI };

            Machine[] Machines =
            {
                new Litography_Machine("LithoScan EUV-3600", "ASML", "SN-ASML-2024-88A9", lito_parts, MachineCondition.Excellent),
                new SMT_Machine("Horizon SolderPrinter X5", "DEK International", "SN-DEK-77492-B7", printer_parts, MachineCondition.Excellent),
                new PaP_Machine("NXT-III High-Speed Mounter", "Fuji Corporation", "SN-FUJI-991A-040", pap_parts, MachineCondition.Excellent),
                new Reflow_Oven("OmniMax Thermal Tunnel", "Heller Industries", "SN-HLR-5542-Z9", oven_parts, MachineCondition.Critical)
            };

            

            // vecorul de angajati
            Employee[] angajati = new Employee[]
            {
                new ProductionManager("M-101", "Andrei Popescu"),
                new MachineOperator("O-205", "Ionut Marin")
            };

            //  Vectorul de produse 
            Product[] inventarProduse = new Product[]
            {
                new Microcontroller("Arduino Uno R4", 15.50, 29.99, 150, "ARM Cortex-M4"),
                new SensorModule("Senzor DHT22", 4.20, 9.50, 300, "Umiditate si Temperatura")
            };

            bool rulare = true;

            while (rulare)
            {
                Console.Clear();

                Console.WriteLine(" \n MANAGEMENT FABRICA (ANGAJATI SI PRODUSE)  \n ");
                Console.WriteLine("1. Afiseaza echipa de angajati");
                Console.WriteLine("2. Afiseaza inventarul de produse");
                Console.WriteLine("0. Iesire");

                Console.Write("\nOptiune: ");
                int opt = int.Parse(Console.ReadLine()!);

                switch (opt)
                {
                    case 1:
                        Console.WriteLine("\n- LISTA ANGAJATI -");
                        foreach (Employee e in angajati)
                        {
                            e.AfiseazaActivitate();
                        }
                        break;

                    case 2:
                        Console.WriteLine("\n-STOC PRODUSE -");
                        foreach (Product p in inventarProduse)
                        {
                            //afiseaza proprietatile produsului
                            Console.WriteLine("Produs: " + p.Name + " | Categorie: " + p.Category + " | Pret: " + p.SellingPrice + " RON | Stoc: " + p.Quantity + " buc.");
                        }
                        break;

                    case 0:
                        rulare = false;
                        break;

                    default:
                        Console.WriteLine("Optiune invalida!");
                        break;
                }

                Console.WriteLine("\nApasa o tasta pentru a continua...");
                Console.ReadKey();
            }
        }
    }
}
