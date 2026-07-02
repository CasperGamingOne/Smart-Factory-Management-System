namespace Smart_Factory_Management_System
{
    public enum MachineStatus { Running, Stopped, Maintenance }    //pt regula 3  - pt a sti in ce stare se afla masina 

    public enum MachineCondition { Excellent, Good, Critical }

    internal abstract class Machine
    {
        public string? Name { get; private protected set; }

        public string? Manufacturer { get; private protected set; }

        public string? SerialNumber { get; private protected set; }

        public DateTime InstallationDate { get; private protected set; }

        public MachinePart[]? Parts { get; private protected set; }

        public MachineStatus Status { get; private protected set; } = MachineStatus.Stopped;

        public MachineCondition Condition { get; protected set; }

        public Machine(string machine_name, string machine_manufacturer, string machine_serial, MachinePart[] parts, MachineCondition condition)
        {
            Name = machine_name;
            Manufacturer = machine_manufacturer;
            SerialNumber = machine_serial;
            InstallationDate = DateTime.Now.AddYears(-7);
            Parts = parts;
            Condition = condition;
        }

        public TimeSpan GetMachineAge()
        {
            return DateTime.Now - InstallationDate;
        }

        public virtual void ExecuteProductionCycle(Product product)
        {
            if (Status != MachineStatus.Running)  //verificam starea inainte de a produce  -Reg 1
            {
                Console.WriteLine($"[ERROR] '{Name}' cannot produce: Machine is {Status}");
                return;
            }

            // polimorfism
            Produce(product);

            // Regula 8- Degradarea conditiei
            Console.WriteLine($"[SYSTEM] Production cycle complete. Machine '{Name}' condition slightly decreased.");
        }

        public abstract void Produce(Product product);

     
        public bool StartMachine()
        {
            if (Status == MachineStatus.Maintenance)
            {
                Console.WriteLine($"[ERROR] Cannot start '{Name}'! It is currently flagged for Maintenance.");   //regula 2 
                return false;
            }

            if (Parts == null || Parts.Length == 0)  //daca lipseste o piesa sau daca este o lista goala
            {
                Console.WriteLine($"[ERROR] Cannot start '{Name}' because it has no parts assigned.");
                Status = MachineStatus.Stopped;
                return false;
            }

            foreach (var part in Parts)
            {
                if (part == null) continue;  //daca exista intrari nule-previne prabusirea programului
                if (part.Condition == PartCondition.Critical)
                {
                    Console.WriteLine($"[CRITICAL] Cannot start '{Name}' because component '{part.Name}' is broken!");
                    Status = MachineStatus.Stopped;
                    return false;
                }
            }

            Status = MachineStatus.Running;
            Console.WriteLine($"[SYSTEM] Machine '{Name}' is now RUNNING production.");
            return true;
        }

        public void StopMachine()
        {
            if (Status == MachineStatus.Running)
            {
                Status = MachineStatus.Stopped;
                Console.WriteLine($"[SYSTEM] Machine '{Name}' has been safely STOPPED.");
            }
        }

        //nouuu(pt regula 3 )
        public virtual void Repair(Technician tech)
        {
            if (this.Status == MachineStatus.Running)
            {
                Console.WriteLine($"EROARE: {tech.Name}, nu poti repara masina in timp ce functioneaza!");
                return;
            }

            // daca ajunge aici inseamna ca masina este oprita si nu poate fi reparata in timp ce functioneaza, deci poate fi reparata de catre tehnician
            this.Status = MachineStatus.Maintenance;
            Console.WriteLine($"Masina {this.Name} este acum in mentenanta de catre  Tehnician fexe{tech.Name}.");
        }

        public virtual void Inspect()
        {
            Console.WriteLine($"[INSPECTION] Running diagnostics on {Name} (S/N: {SerialNumber})...");

            if (Parts == null)
            {
                Console.WriteLine($"[ERROR] No parts assigned to {Name}.");
                return;
            }

            foreach (var part in Parts)  {part.PrintPartInfo();}

      
 
        }

    }

    internal class Litography_Machine : Machine
    {
        public Litography_Machine(string machine_name, string machine_manufacturer, string machine_serial, MachinePart[] parts, MachineCondition condition)
            : base(machine_name, machine_manufacturer, machine_serial, parts, condition)
        {
        }
        public override void Produce(Product product)
        {
            Console.WriteLine($"Litography Machine {Name} is producing {product.Name}.");
            // Aici se sscade starea  (Regula 8)
            this.Condition = MachineCondition.Good; // Exemplu de schimbare stare
        }
        public override void Inspect()       //reg 19
        {
            base.Inspect(); // Apeleaza verificarea de baza
            Console.WriteLine($"-> [SPECIFIC] Litography {Name}: Verificare aliniere fascicul laser...");
        }
    }

    internal class SMT_Machine : Machine  // Solder Paste Printer
    {
        public SMT_Machine(string machine_name, string machine_manufacturer, string machine_serial, MachinePart[] parts, MachineCondition condition)
            : base(machine_name, machine_manufacturer, machine_serial, parts, condition)
        {
        }
        public override void Produce(Product product)
        {
            Console.WriteLine($"SMT Machine {Name} is producing {product.Name}.");
        }
        public override void Inspect()
        {
            base.Inspect();
            Console.WriteLine($"-> [SPECIFIC] SMT {Name}: Calibrare duze și presiune pastă lipire...");   //reg 19
        }
    }

    internal class PaP_Machine : Machine  // Pick and Place
    {
        public PaP_Machine(string machine_name, string machine_manufacturer, string machine_serial, MachinePart[] parts, MachineCondition condition)
            : base(machine_name, machine_manufacturer, machine_serial, parts, condition)
        {
        }
        public override void Produce(Product product)
        {
            Console.WriteLine($"PAP Machine {Name} is producing {product.Name}.");
        }
        public override void Inspect()
        {
            base.Inspect(); 
            Console.WriteLine($"-> [SPECIFIC] PaP {Name}: Calibrare sistem viziune si verificare axele X-Y-Z.");
        }
    }

    internal class Reflow_Oven : Machine
    {
        public Reflow_Oven(string machine_name, string machine_manufacturer, string machine_serial, MachinePart[] parts, MachineCondition condition)
            : base(machine_name, machine_manufacturer, machine_serial, parts, condition)
        {
        }
        public override void Produce(Product product)
        {
            Console.WriteLine($"Reflow Oven {Name} isthermally curing {product.Name}.");
          
        }
        public override void Inspect()
        {
            base.Inspect();
            Console.WriteLine($"-> [SPECIFIC] Reflow Oven {Name}: Test senzori temperatura si elemente incalzire...");
        }
    }


}

