namespace Smart_Factory_Management_System
{
    public abstract class Employee
    {
        public string Id { get; private set; }    //proprietatile
        public string Name { get; set; }
        public string Role { get; protected set;  }
    

        protected Employee(string id, string name)  //constructorul
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
    public class ProductionManager : Employee
    {
        public ProductionManager(string id, string name) : base(id, name)
        {
            Role = "Production Manager";
        }
        public override void AfiseazaActivitate()
        {
            Console.WriteLine("Managerul de Productie " + Name + " verifica planul de componente electronice.");
        }
    }
    //a doua clasa derivata
    public class MachineOperator : Employee
    {
        public MachineOperator(string id, string name) : base(id, name)
        {
            Role = "Machine Operator";
        }
        public override void AfiseazaActivitate()
        {
            Console.WriteLine("Operatorul " + Name + " monitorizeaza bratul robotizat.");
        }
    }
}
