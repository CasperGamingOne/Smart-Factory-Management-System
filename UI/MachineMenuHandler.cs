using Spectre.Console;

namespace Smart_Factory_Management_System;

internal static class MachineMenuHandler
{
    public static void Run(Factory factory, Employee loggedInUser, ILoggerService loggerService,
        IJsonRepository<Machine> machineRepo, IJsonRepository<Product> productRepo)
    {
        var inRoom = true;
        while (inRoom)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule($"[cyan]{Machines.FleetMonitoringTitle}[/]").Centered());

            var menuOptions = MenuOptions.MachineMenu.Select((item, index) => $"{index + 1}. {item}").ToList();
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title(Machines.SelectDiagnosticOption)
                    .AddChoices(menuOptions));

            var option = choice.Split(". ", 2)[1];
            switch (option)
            {
                case "Overall Fleet Status Overview":
                    DisplayFleetOverview(factory);
                    loggerService.LogInfo(LogOrigin.USER, LogEvent.FleetStatusViewed, loggedInUser.Username);
                    break;

                case "Run Deep Component Inspection":
                    if (loggedInUser is not Technician)
                    {
                        AnsiConsole.MarkupLine(
                            string.Format(Common.AccessDenied, loggedInUser.Role));
                    }
                    else
                    {
                        RunInspection(factory, loggerService);
                        machineRepo.Save(factory.Machines);
                    }

                    break;

                case "Fulfill Pending Orders":
                    ProductionMenuHandler.Run(factory, loggedInUser, loggerService, machineRepo, productRepo);
                    break;
                case "Return to Main Menu":
                    inRoom = false;
                    break;
            }

            if (inRoom)
            {
                AnsiConsole.MarkupLine(Common.PressKeyToContinue);
                Console.ReadKey(true);
            }
        }
    }

    private static void DisplayFleetOverview(Factory factory)
    {
        var table = new Table().Border(TableBorder.Square);
        table.AddColumn(Machines.MachineAssetColumn);
        table.AddColumn(Machines.ManufacturerColumn);
        table.AddColumn(Machines.OperationalStatusColumn);
        table.AddColumn(Machines.StructuralConditionColumn);

        foreach (var mach in factory.Machines)
        {
            table.AddRow(mach.Name ?? "-", mach.Manufacturer ?? "-", mach.Status.ToString(),
                mach.Condition.ToString());
        }

        AnsiConsole.Write(table);
    }

    private static void RunInspection(Factory factory, ILoggerService loggerService)
    {
        if (factory.Machines.Count == 0)
        {
            AnsiConsole.MarkupLine(Machines.NoMachinesAssetIndex);
            return;
        }

        var selector = new SelectionPrompt<Machine>()
            .Title(Machines.SelectMachineToManage)
            .PageSize(10)
            .UseConverter(m =>
            {
                var statusColor = m.Condition == MachineCondition.Critical ? "red" : "green";
                return string.Format(Machines.MachineSelectorConverter, statusColor, m.Name, m.Status);
            });
        foreach (var t in factory.Machines)
            selector.AddChoice(t);

        var chosenMachine = AnsiConsole.Prompt(selector);

        AnsiConsole.MarkupLine(string.Format(Machines.AuditingComponentStack, chosenMachine.Name));

        chosenMachine.InspectMachine();

        if (chosenMachine.NeedsRepair())
        {
            AnsiConsole.MarkupLine(Machines.PartsNotExcellent);

            if (AnsiConsole.Confirm(Machines.RepairConfirm)) chosenMachine.RepairMachine();
        }
        else
        {
            AnsiConsole.MarkupLine(Machines.RepairNotNeeded);
        }

        loggerService.LogInfo(LogOrigin.USER, LogEvent.MaintenancePerformed,
            $"{chosenMachine.Id} - {chosenMachine.Name}");
    }
}