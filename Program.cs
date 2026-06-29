
namespace Smart_Factory_Management_System
{
    internal class Program
    {
        static void Main(string[] args)
        {
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
