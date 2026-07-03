namespace Smart_Factory_Management_System
{
    public abstract class Employee
    {
        public int Id { get; private set; }    //proprietatile
        public string Name { get; set; }
        public string Role { get; protected set;  }
    

        protected Employee(int id, string name)  //constructorul
         {
            Id = id;
            Name = name;
            Role = "Angajat Simplu";
         }

        public virtual void AfiseazaActivitate()
        {
            Console.WriteLine("Angajatul " + Name + " (ID: " + Id + ") isi indeplineste sarcinile generale.");
        }
    }
    //clasele derivate pentru diferite tipuri de angajati
    public class Director : Employee
    {
        public Director(int id, string name) : base(id, name) { Role = "Director"; }
        public override void AfiseazaActivitate()
        {
            Console.WriteLine("Directorul " + Name + " stabileste strategia fabricii.");
        }
        public void CreeazaComandaProductie(string numeProdus, int cantitate)  //reg 5
        {
            Console.WriteLine($"[DIRECTOR] {Name} a lansat o noua comandă de productie:");
            Console.WriteLine($"-> Produs: {numeProdus} | Cantitate: {cantitate} unitati.");

        }
    }

    
    public class Technician : Employee
    {
        public Technician(string name) : base(name) { Role = "Tehnician"; }
        public override void AfiseazaActivitate()
        {
            Console.WriteLine("Tehnicianul " + Name + " repara echipamentele defecte.");
        }
        internal void InspecteazaMasina(Machine m)
        {
            if (IsCertifiedInspector)
            {
                Console.WriteLine($"[INSPECȚIE] Tehnicianul {Name} (Certificat) inspectează mașina {m.Name}.");
                m.Inspect(); // Apelează logica de diagnosticare
                m.InspectataDeInginer = true; // Setează flag-ul pentru Regula 4
            }
            else
            {
                Console.WriteLine($"[EROARE] Tehnicianul {Name} nu are certificare de inginer pentru inspecții!");
            }
        }
    }
    
    public class SalesAgent : Employee
    {
        public SalesAgent(int id, string name) : base(id, name) { Role = "Agent Vanzari"; }
        public override void AfiseazaActivitate()
        {
            Console.WriteLine("Agentul de Vanzari " + Name + " negociaza contracte cu clientii.");
        }
    }
    public class Accountant : Employee
    {
        public Accountant(int id, string name) : base(id, name) { Role = "Contabil"; }
        public override void AfiseazaActivitate()
        {
            Console.WriteLine("Contabilul " + Name + " calculeaza profitul si intocmeste facturile.");
        }
    }


}
