namespace Smart_Factory_Management_System;

public static class Accounting
{
    public const string Title = "ACCOUNTING - Price Produced Batches";
    public const string NoBatches = "[yellow]No completed production batches yet.[/]";
    public const string NoBatchesToPrice = "[yellow]No batches available to price.[/]";
    public const string SelectBatchToPrice = "Select batch to price:";
    public const string SetUnitPricePrompt = "Set unit selling price for this batch ($):";
    public const string BatchPriced = "[green]✔ Batch {0} priced at ${1:F2} per unit.[/]";
    public const string ProcessReportRequestsTitle = "Process Report Requests";
    public const string NoPendingRequests = "[yellow]No pending report requests.[/]";
    public const string SelectRequestToFulfill = "Select request to fulfill:";
    public const string RequestFulfilled = "[green]✔ Report '{0}' (Req ID: {1}) fulfilled by {2}.[/]";

    public const string PendingReportHeader = "Pending Report Requests";
    public const string PendingReportNotification = "Report Request [cyan]#{0}[/] ({1}) is pending your fulfillment!";
    public const string PendingReportAcknowledge = "[grey]Press any key to acknowledge...[/]";
}