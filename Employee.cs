namespace Smart_Factory_Management_System
{
    public abstract class Employee
    {
        private static int idCounter = 0;

        public int Id { get; private set; }
        public string Name { get; set; }
        public string Role { get; protected set;  }
    

        protected Employee(string name)
         {
            idCounter++;
            Id = idCounter;
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
        public Director(string name) : base(name) { Role = "Director"; }
        public override void AfiseazaActivitate()
        {
            Console.WriteLine("Directorul " + Name + " stabileste strategia fabricii.");
        }
    }

    public class Technician : Employee
    {
        public Technician(string name) : base(name) { Role = "Tehnician"; }
        public override void AfiseazaActivitate()
        {
            Console.WriteLine("Tehnicianul " + Name + " repara echipamentele defecte.");
        }
    }

    public class SalesAgent : Employee
    {
        public SalesAgent(string name) : base(name) { Role = "Agent Vanzari"; }
        public override void AfiseazaActivitate()
        {
            Console.WriteLine("Agentul de Vanzari " + Name + " negociaza contracte cu clientii.");
        }
    }
    public class Accountant : Employee
    {
        public Accountant(string name) : base(name) { Role = "Contabil"; }
        public override void AfiseazaActivitate()
        {
            Console.WriteLine("Contabilul " + Name + " calculeaza profitul si intocmeste facturile.");
        }
    }


}
