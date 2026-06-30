using System;

namespace Smart_Factory_Management_System
{
    internal class Program
    {
        // Vectori declarati global (static) pentru a fi accesati din orice metoda
        static Employee[] angajati = new Employee[50];
        static int nrAngajati = 0;

        static Product[] inventarProduse = new Product[50];
        static int nrProduse = 0;

        static void Main(string[] args)
        {
            // Initializare 
            angajati[nrAngajati++] = new ProductionManager("M-101", "Andrei Popescu");
            angajati[nrAngajati++] = new MachineOperator("O-205", "Ionut Marin");

            inventarProduse[nrProduse++] = new Microcontroller("Arduino Uno R4", 15.50, 29.99, 150, "ARM Cortex-M4");
            inventarProduse[nrProduse++] = new SensorModule("Senzor DHT22", 4.20, 9.50, 300, "Umiditate si Temperatura");

            bool rulare = true;

            while (rulare)
            {
                //Console.Clear();
                Console.WriteLine(" \n --- MANAGEMENT FABRICA (ELECTRONICE) --- \n ");
                Console.WriteLine("1. Management Angajati");
                Console.WriteLine("2. Management Produse");
                Console.WriteLine("0. Iesire");

                Console.Write("\nOptiune: ");
                string input = Console.ReadLine();
                int opt;
                if (!int.TryParse(input, out opt)) continue;   //transforma textul din variabila input intr-un nr intreg; daca reuseste pune rez in variab "opt"
                //daca nu reuseste , trece peste

                switch (opt)
                {
                    case 1: SubmeniuAngajati(); break;
                    case 2: SubmeniuProduse(); break;
                    case 0: rulare = false; break;
                    default: Console.WriteLine("Optiune invalida!"); break;
                }

                if (rulare)
                {
                    Console.WriteLine("\nApasa o tasta pentru a continua...");
                    Console.ReadKey();
                }
            }
        }

        static void SubmeniuAngajati()
        {
            Console.WriteLine("\n--- SUBMENIU ANGAJATI ---");
            Console.WriteLine("1. Afiseaza echipa");
            Console.WriteLine("2. Adauga angajat");
            string opt = Console.ReadLine();

            if (opt == "1")
            {
                for (int i = 0; i < nrAngajati; i++)
                {
                    angajati[i].AfiseazaActivitate();
                }
            }
            else if (opt == "2")
            {
                Console.Write("ID: "); string id = Console.ReadLine();
                Console.Write("Nume: "); string nume = Console.ReadLine();
                AdaugaAngajat(new ProductionManager(id, nume));
            }
        }

        static void AdaugaAngajat(Employee e)
        {
            if (nrAngajati < angajati.Length)
            {
                angajati[nrAngajati] = e;
                nrAngajati++;
                Console.WriteLine("Angajat adaugat cu succes!");
            }
            else
            {
                Console.WriteLine("Eroare: Fabrica este plina!");
            }
        }

        static void SubmeniuProduse()
        {
            Console.WriteLine("\n--- STOC PRODUSE ---");
            for (int i = 0; i < nrProduse; i++)
            {
                Product p = inventarProduse[i];
                Console.WriteLine("Produs: " + p.Name + " | Pret: " + p.SellingPrice + " RON | Stoc: " + p.Quantity);
            }
        }
    }
}