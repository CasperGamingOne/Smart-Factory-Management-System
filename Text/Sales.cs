namespace Smart_Factory_Management_System;

public static class Sales
{
    public const string Title = "SALES - Create Production Orders";
    public const string SelectProduct = "Select product to order:";
    public const string EnterQuantity = "Enter quantity for the batch:";
    public const string AssignToTechnician = "Assign to technician:";
    public const string SelectTechnician = "Select technician:";
    public const string NoTechnicians = "[red]No technicians registered. Ask an admin to add one.[/]";
    public const string OrderPlaced = "[green]✔ Order placed: {0} - {1} x{2} assigned to tech #{3}[/]";
    public const string OrderPlacedOnHold = "[green]✔ Order placed: {0} - {1} x{2} (Status: On Hold / Unassigned)[/]";
    public const string NoPendingOrders = "[yellow]No pending orders.[/]";
    public const string RecordSaleNoStock = "[yellow]No available inventory or batches to sell.[/]";
    public const string ChooseSaleSource = "Choose sale source:";
    public const string SelectBatchToSell = "Select batch to record sale for:";
    public const string BatchAlreadySold = "[yellow]This batch is already marked as sold.[/]";
    public const string EnterUnitSoldPrice = "Enter unit sold price ($):";
    public const string SaleRecordedBatch = "[green]✔ Recorded sale for batch {0} at ${1:F2} per unit.[/]";
    public const string NoStockedItems = "[yellow]No stocked items available to sell.[/]";
    public const string SelectProductToSell = "Select product to sell:";
    public const string SellQuantity = "Enter quantity to sell (available: {0}):";
    public const string QtyMustBePositive = "[red]Quantity to sell must be greater than zero.[/]";

    public const string InsufficientInventory =
        "[red]Error: Cannot sell {0} units. Only {1} units are available in inventory.[/]";

    public const string SaleRecordedInventory =
        "[green]✔ Sold {0} units of {1} at ${2:F2} per unit. Batch {3} recorded.[/]";

    public const string EnterCustomProductName = "Enter custom product name for the batch:";
    public const string EnterCpuCores = "Enter CPU cores (e.g., 4, 8, 16):";
    public const string EnterClockSpeed = "Enter clock speed in GHz (e.g., 3.5):";
    public const string EnterSocketStandard = "Enter motherboard socket standard (e.g., AM4, LGA1700):";
    public const string SelectFormFactor = "Select motherboard form factor:";

    public const string OrderCompleteHeader = "Production Order Completed";

    public const string OrderCompleteNotification =
        "Your order [cyan]#{0}[/] ({1} x{2}) has been successfully completed by production!";

    public const string OrderCompleteAcknowledge = "[grey]Press any key to acknowledge...[/]";
}