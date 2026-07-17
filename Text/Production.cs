namespace Smart_Factory_Management_System;

public static class Production
{
    public const string Title = "🏭 Active Production Control Deck";

    public const string NoMachinerySeeded =
        "[yellow]⚠ No production machinery has been seeded in the factory layout yet.[/]";

    public const string PressKeyToGoBack = "[grey]Press any key to go back...[/]";
    public const string SelectOrderToProcess = "Select a pending order to process:";
    public const string OnlyAssignedTech = "[red]Only the assigned technician can start this order.[/]";
    public const string SelectMachineToUse = "Select machine to use:";
    public const string MachineNotRunningBoot = "Machine {0} is not running. Boot it?";
    public const string UnknownProductType = "[red]Unknown product type for this order.[/]";
    public const string StartProduction = "[cyan]Starting production for order {0} - {1} x{2}[/]";
    public const string ProducedOneUnit = "[green]Produced 1 unit ({0}). Completed {1}/{2}[/]";
    public const string MachineTripped = "[red]Machine tripped. Production paused.[/]";
    public const string BatchCompleted = "[green]Batch complete. Created batch {0} with {1} items.[/]";
    public const string OrderIncomplete = "[yellow]Order incomplete. Produced {0}/{1} so far.[/]";

    public const string MotherboardWorkflowNoMachines =
        "[red]Motherboard workflow requires SMT, Pick-and-Place, and Reflow machines to be registered.[/]";

    public const string StartMotherboardWorkflow = "[cyan]Starting motherboard workflow for order {0} - {1} x{2}[/]";
    public const string MotherboardCompleted = "[green]Motherboard completed {0}/{1}.[/]";
    public const string SolderPastePrinting = "Solder Paste Printing";
    public const string SolderPastePrintingSpinner = "Exposing board to solder paste mesh...";
    public const string SolderPastePrintingFail = "Motherboard workflow stopped during solder paste printing.";
    public const string PickAndPlace = "Pick and Place Assembly";
    public const string PickAndPlaceSpinner = "Aligning and mounting components...";
    public const string PickAndPlaceFail = "Motherboard workflow stopped during pick-and-place assembly.";
    public const string ReflowBaking = "Reflow Baking";
    public const string ReflowBakingSpinner = "Heating solder joints to fuse the board...";
    public const string ReflowBakingFail = "Motherboard workflow stopped during reflow baking.";

    public const string AutoAssignHeader = "Auto-Assignment Notification";

    public const string AutoAssignNotification =
        "Order [cyan]#{0}[/] ({1} x{2}) has been issued and auto-assigned to you!";

    public const string AutoAssignAcknowledge = "[grey]Press any key to acknowledge...[/]";

    public const string CostAdvice =
        "[yellow]ℹ️  ADVICE: When determining unit production cost, please account for any maintenance, repairs, or machine break-downs that occurred during this cycle.[/]";

    public const string CostPrompt = "Enter the final unit production cost ($):";
    public const string CostValidationInvalidNumber = "[red]Please enter a valid positive number.[/]";
    public const string CostValidationNegative = "[red]Cost cannot be negative.[/]";

    public const string ProductionLineTitle = "[yellow]Production Line: {0}[/]";

    public const string MachineTrippedAlert =
        "[bold red]⚠️ MACHINE TRIPPED: {0} has stopped due to a critical part breakdown! Please perform repairs. ⚠️[/]";

    public const string CompletedUnitsStatus = "[bold green]Completed: {0} / {1} units[/]";
    public const string MotherboardLineTitle = "[yellow]Motherboard Production Line[/]";

    public const string WorkflowTrippedAlert =
        "[bold red]⚠️ WORKFLOW TRIPPED: One or more machines have broken down! Please perform repairs. ⚠️[/]";

    public const string StepSmt = "1. Solder Paste Printing (SMT)";
    public const string StepPap = "2. Pick & Place Assembly (PaP)";
    public const string StepOven = "3. Reflow Baking (Oven)";
}