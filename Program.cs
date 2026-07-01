using Spectre.Console;

namespace Smart_Factory_Management_System
{

    internal class Program
    {
        static void Main(string[] args)
        {
            // Initialize your core factory engine and mock data once at startup
            Factory factory = new Factory();

            // The Outer Application Loop: Keeps the system alive for new logins
            while (true)
            {
                AnsiConsole.Clear();
                AnsiConsole.Write(new Rule("[yellow]SMART FACTORY SYSTEM - LOGIN GATEWAY[/]").Centered());

                // 1. Invoke the LoginHandler to authenticate the user
                // It prompts for credentials and returns the validated Employee object
                Employee loggedInUser = LoginHandler.ShowLoginScreen(factory);

                if (loggedInUser == null)
                {
                    // If your login system returns null (e.g., if you choose to exit from login screen)
                    AnsiConsole.MarkupLine("[red]Application shutting down...[/]");
                    break;
                }

                // Welcome the authenticated employee
                AnsiConsole.MarkupLine($"[green]Welcome back, {loggedInUser.Name} ({loggedInUser.Role})![/]");
                AnsiConsole.Status().Start("Booting production environment...", ctx => { System.Threading.Thread.Sleep(1000); });

                // 2. The Inner Session Loop: Active while a user is authenticated
                bool sessionActive = true;
                while (sessionActive)
                {
                    AnsiConsole.Clear();
                    AnsiConsole.Write(new Rule($"[blue]FACTORY CONTROL PANEL - Session: {loggedInUser.Name}[/]").Centered());

                    // Render the mandatory application menu layout from your specification sheet
                    var choice = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("[yellow]Select an operational module:[/]")
                            .PageSize(10)
                            .AddChoices(new[] {
                                "1. Employee Management",
                                "2. Machine Management",
                                "3. Product Management",
                                "4. Production",
                                "5. Reports",
                                "6. Factory Information",
                                "7. Log Out / Exit Session"
                            }));

                    // 3. Central routing to independent, decoupled handler classes
                    switch (choice)
                    {
                        case "1. Employee Management":
                            // Spin up the independent room and pass factory references
                            EmployeeMenuHandler.Run(factory, loggedInUser);
                            break;

                        case "2. Machine Management":
                            MachineMenuHandler.Run(factory, loggedInUser);
                            break;

                        case "3. Product Management":
                            // Product management handler logic here
                            break;

                        case "4. Production":
                            // Production simulation launcher logic here
                            break;

                        case "5. Reports":
                            ReportMenuHandler.Run(factory, loggedInUser);
                            break;

                        case "6. Factory Information":
                            // Display quick factory specifications/counters
                            AnsiConsole.MarkupLine($"[bold]Factory Capacity Metrics:[/]");
                            AnsiConsole.MarkupLine($"Machines Configured: [cyan]{factory.MachineCount}[/]");
                            AnsiConsole.MarkupLine($"Active Staff: [cyan]{factory.EmployeeCount}[/]");
                            AnsiConsole.MarkupLine("\nPress any key to return...");
                            Console.ReadKey();
                            break;

                        case "7. Log Out / Exit Session":
                            // To exit the inner loop cleanly, toggle the state boundary flags
                            AnsiConsole.MarkupLine("[yellow]Logging out of current profile...[/]");
                            System.Threading.Thread.Sleep(800);
                            sessionActive = false;
                            break;
                    }
                }

                // When sessionActive becomes false, control breaks out of the inner loop
                // and naturally flows back to the top of the while(true) loop, showing the login screen again!
            }

            /*
            bool rulare = true;

            while (rulare)
            {
                //Console.Clear();
                Console.WriteLine(" \n --- MANAGEMENT FABRICA (ELECTRONICE) --- \n ");
                Console.WriteLine("1. Management Angajati");
                Console.WriteLine("2. Management Produse");
                Console.WriteLine("3. Management Masinarii");
                Console.WriteLine("4. Management Piese Componente");
                Console.WriteLine("0. Iesire");

                Console.Write("\nOptiune: ");
                string input = Console.ReadLine()!;
                int opt;
                if (!int.TryParse(input, out opt)) continue;   //transforma textul din variabila input intr-un nr intreg; daca reuseste pune rez in variab "opt"
                                                               //daca nu reuseste , trece peste

                switch (opt)
                {
                    case 1: SubmeniuAngajati(); break;
                    case 2: SubmeniuProduse(); break;
                    case 3: SubmeniuMasinarii(); break;
                    case 4: SubmeniuPiese(); break;
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

        ///1
        static void SubmeniuAngajati()
        {
            Console.WriteLine("\n--- SUBMENIU ANGAJATI ---");
            Console.WriteLine("1. Afiseaza echipa");
            Console.WriteLine("2. Adauga angajat");
            string opt = Console.ReadLine()!;

            if (opt == "1")
            {
                for (int i = 0; i < nrAngajati; i++)
                {
                    angajati[i].AfiseazaActivitate();
                }
            }
            else if (opt == "2")
            {
                Console.WriteLine("Alege tipul angajatului:");
                Console.WriteLine("1. Director, 2. Inginer, 3. Tehnician, 4. Contabil, 5. Manager Productie, 6. Operator");
                string tip = Console.ReadLine()!;


                Console.Write("ID: "); string id = Console.ReadLine()!;
                Console.Write("Nume: "); string nume = Console.ReadLine()!;


                Employee nou = null; // initializam cu null

                // Decidem ce tip

                switch (tip)
                {

                    case "1": nou = new Director(id, nume); break;

                    case "2": nou = new Engineer(id, nume); break;

                    case "3": nou = new Technician(id, nume); break;

                    case "4": nou = new Accountant(id, nume); break;

                    case "5": nou = new ProductionManager(id, nume); break;

                    case "6": nou = new MachineOperator(id, nume); break;

                    default: Console.WriteLine("Tip invalid!"); break;

                }


                // il adaugam in vectorul globalif (nou != null)

                {

                    AdaugaAngajat(nou);

                    Console.WriteLine("\nAngajat adugat cu succes:");

                    nou.AfiseazaActivitate();

                }
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

        //2
        static void SubmeniuProduse()
        {
            Console.WriteLine("\n--- STOC PRODUSE ---");
            for (int i = 0; i < nrProduse; i++)
            {
                Product p = inventarProduse[i];
                Console.WriteLine("Produs: " + p.Name + " | Pret: " + p.SellingPrice + " RON | Stoc: " + p.Quantity);
            }
        }

        //3
        static void SubmeniuMasinarii()
        {
            Console.WriteLine("\n- MANAGEMENT MASINARII -");
            Console.WriteLine("1. Afiseaza toate masinile");
            Console.WriteLine("2. Adauga masina noua");
            string opt = Console.ReadLine()!;


            switch (opt)
            {
                case "1":
                    Console.WriteLine("\n--- LISTA MASINI ---");
                    foreach (Machine m in machines)
                    {
                        if (m != null)
                        {
                            Console.WriteLine($"- {m.Name} (Producator: {m.Manufacturer}) - Serial Number: {m.SerialNumber} - Installation Date: {m.InstallationDate} - Stare: {m.Condition} - Status: {m.Status}");
                        }
                    }
                    break;

                case "2":
                    Console.WriteLine("Functionalitate de adaugare in curs de dezvoltare...");
                    break;

                default:
                    Console.WriteLine("Optiune invalida!");
                    break;
            }
        }

        //4
        static void SubmeniuPiese()
        {
            Console.WriteLine("\n- GESTIUNE PIESE COMPONENTE -");
            Console.WriteLine("Alege masina pentru a vedea piesele componente:");

            for (int i = 0; i < nr_machines; i++)
            {
                Console.WriteLine($"{i + 1}. {machines[i].Name}");
            }

            Console.Write("Optiune: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= nr_machines)
            {
                Machine m = machines[index - 1];
                Console.WriteLine($"\nComponentele masinii {m.Name}:");

                if (m.Parts != null)
                    foreach (MachinePart p in m.Parts)
                        Console.WriteLine($"- {p.Name} | Stare: {p.Condition}");
            }
            else
                Console.WriteLine("The selected machine does not have components.");
        }
            */
        }

    }
}