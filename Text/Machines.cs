namespace Smart_Factory_Management_System;

public static class Machines
{
    public const string BootSequence = "System Boot Sequences: {0}";
    public const string BootSpinnerAnalyzing = "Analyzing diagnostic circuit registers...";
    public const string BootSpinnerChecking = "Checking component hardware array counters...";

    public const string CriticalInitError =
        "[red]❌ [bold]CRITICAL INITIALIZATION ERROR:[/] Component [underline]{0}[/] has suffered a complete breakdown!\n[grey]Action Required:[/] Dispatch an authorized engineer to run maintenance protocols.[/]";

    public const string BootFailureHeader = " BOOT FAILURE ";

    public const string OnlineMessage =
        "[green]✔ [bold]ONLINE:[/] {0} is fully calibrated and processing manufacturing lines.[/]";

    public const string ShutdownSequence = "System Shutdown Sequence: {0}";

    public const string AlreadyStopped =
        "[yellow]⚠ [bold]SYSTEM IDLE:[/] [underline]{0}[/] is already stopped and sitting securely in standby mode.[/]";

    public const string SafeDecelerationSpinner = "Initiating safe assembly line deceleration...";
    public const string SpoolDownSpinner = "Spooling down dynamic mechanical sub-structures...";
    public const string IsolateRelaysSpinner = "Isolating secondary high-voltage power relays...";

    public const string ShutdownComplete =
        "[red]🛑 [bold]SHUTDOWN COMPLETE:[/] {0} has been safely isolated and powered down.\n[grey]Operational State updated to:[/] [yellow bold]STOPPED (STANDBY)[/]";

    public const string SystemOfflineHeader = " SYSTEM OFFLINE ";

    public const string AlertCriticalPart =
        "[red bold]⚡ALERT:[/] [underline]{0}[/] has suffered a total breakdown! Machine safety override has been tripped!\n";

    public const string WarningDegradation =
        "[yellow]⚠ SYSTEM NOTICE:[/] [underline]{0}[/] showing performance degradation (Moved to {1} condition).\n";

    public const string DiagnosticHub = "Diagnostics Hub: {0}";
    public const string HardwarePropertyColumn = "[grey]Hardware Property[/]";
    public const string AssignedValueColumn = "[grey]Assigned Value[/]";
    public const string ManufacturerIdentity = "Manufacturer Identity";
    public const string FactorySerialRef = "Factory Serial Reference";
    public const string AssetLifeAge = "Asset Total Life Age";
    public const string AssetLifeAgeValue = "{0:F1} Years / {1} Days";
    public const string CurrentAssetState = "Current Asset State";
    public const string AssetIdentityProfileHeader = " Asset Identity Profile ";
    public const string TrackedComponentHeader = "[bold]Tracked Component Item[/]";
    public const string HealthStatusHeader = "[bold]Health Status[/]";
    public const string TechSpecsHeader = "[bold]Technical Specifications & Diagnostics[/]";
    public const string DoesNotNeedRepairs = "[green]{0} does not need repairs right now.[/]";
    public const string MaintenanceBay = "Maintenance Bay: {0}";

    public const string RepairCompleteMsg =
        "[green]✔ Repair complete for [bold]{0}[/].[/]\n[grey]Parts restored:[/] {1}\n[grey]Machine state:[/] {2} / {3}";

    public const string RepairCompleteHeader = " REPAIR COMPLETE ";
    public const string ProcessErrorUnsupported = "Error: This machine cannot process {0}";
    public const string ProcessWarningUnexpectedState = "Warning: {0} is not in the expected {1} state for {2}.";
    public const string ProcessSuccessState = "Successfully completed {2} for {0}. New state: {1}";
    public const string FleetMonitoringTitle = "MACHINE MONITORING & MAINTENANCE";
    public const string SelectDiagnosticOption = "[yellow]Select a diagnostic option:[/]";

    public const string MachineAssetColumn = "[cyan]Machine Asset[/]";
    public const string ManufacturerColumn = "[cyan]Manufacturer[/]";
    public const string OperationalStatusColumn = "[cyan]Operational Status[/]";
    public const string StructuralConditionColumn = "[cyan]Structural Condition[/]";

    public const string NoMachinesAssetIndex = "[red]No machines are currently provisioned in the asset index.[/]";
    public const string SelectMachineToManage = "Select a machine to manage:";
    public const string MachineSelectorConverter = "[{0}]{1}[/] - [dim]Status: {2}[/]";
    public const string AuditingComponentStack = "\n[bold underline]Auditing Component Stack for: {0}[/]";
    public const string PartsNotExcellent = "[yellow]This machine has parts that are not in excellent condition.[/]";
    public const string RepairConfirm = "Repair this machine now?";
    public const string RepairNotNeeded = "[green]This machine does not currently need repairs.[/]";
    public const string LitographySuccess = "[green]✔ Successfully manufactured: {0}[/]\n";

    public const string LitographyOffline =
        "[red]❌ Cannot produce {0}. Machine is offline. Please boot or repair it first.[/]\n";

    public const string LitographyStart = "[cyan]🏭 Starting processing sequence for: [underline]{0}[/][/]\n";
    public const string LitographySpinner = "Exposing wafer structure using optical masks...";
    public const string RepairSpinnerOpeningPanels = "Opening maintenance access panels for {0}...";
    public const string RepairSpinnerServicingPart = "Repairing/calibrating component: {0}...";
    public const string RepairSpinnerDiagnostics = "Running diagnostic calibration cycles...";
}