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
    }

    
    public class Technician : Employee
    {
        public Technician(int id, string name) : base(id, name) { Role = "Tehnician"; }
        public override void AfiseazaActivitate()
        {
            Console.WriteLine("Tehnicianul " + Name + " repara echipamentele defecte.");
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
