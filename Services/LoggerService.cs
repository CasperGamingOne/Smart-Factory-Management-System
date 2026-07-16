using Spectre.Console;

namespace Smart_Factory_Management_System;

public enum LogOrigin
{
    SYSTEM,
    USER,
    UNDO
}

public enum LogEvent
{
    LoginSuccess,
    LoginFailed,
    Logout,
    ProductionStarted,
    ProductionInterrupted,
    ProductionCompleted,
    MaintenancePerformed,
    ClosingApplication,
    StaffViewed,
    EmployeeAdded,
    FleetStatusViewed,
    InventoryViewed,
    InventoryAnalyticsViewed,
    OrderPlaced,
    PendingOrdersViewed,
    SaleRecorded,
    BatchesViewed,
    BatchPriceSet,
    ReportRequestsProcessed,
    ProductionSummaryReportGenerated,
    EmployeeReportGenerated,
    BatchRevenueReportGenerated,
    OrderBacklogReportGenerated,
    PrintableReportRequested,
    FactoryOverviewReportGenerated,
    StaffingReportGenerated,
    MachineFleetReportGenerated,
    InventoryReportGenerated,
    FullNameUpdated,
    UsernameUpdated,
    PasswordUpdated,
    PasswordChangedFirstLogin,
    OperationUndone
}

public class LoggerService(IFileSystemService fileService) : ILoggerService
{
    private const string LogFileName = "operations.log";

    public void LogInfo(LogOrigin origin, LogEvent eventType, string? context = "")
    {
        var message = eventType switch
        {
            LogEvent.LoginSuccess => $"Successful login: User '{context}'",
            LogEvent.LoginFailed => $"Failed login attempt: Username '{context}'",
            LogEvent.Logout => $"User {context} logged out.",
            LogEvent.ProductionStarted => $"Production operation started for {context}",
            LogEvent.ProductionCompleted => $"Production operation completed for {context}",
            LogEvent.MaintenancePerformed => $"Maintenance performed on the machine {context}",
            LogEvent.ClosingApplication => "Closing application...",
            LogEvent.StaffViewed => "Staff list viewed.",
            LogEvent.EmployeeAdded => $"New employee registered: {context}",
            LogEvent.FleetStatusViewed => "Overall machine fleet status viewed.",
            LogEvent.InventoryViewed => "Finished goods stock viewed.",
            LogEvent.InventoryAnalyticsViewed => "Inventory financial & capacity analytics viewed.",
            LogEvent.OrderPlaced => $"Production order placed: {context}",
            LogEvent.PendingOrdersViewed => "Pending orders list viewed.",
            LogEvent.SaleRecorded => $"Sale recorded: {context}",
            LogEvent.BatchesViewed => "Production batches viewed.",
            LogEvent.BatchPriceSet => $"Unit sell price set for batch: {context}",
            LogEvent.ReportRequestsProcessed => $"Report request processed: {context}",
            LogEvent.ProductionSummaryReportGenerated => "Production summary report generated.",
            LogEvent.EmployeeReportGenerated => "Employee report generated.",
            LogEvent.BatchRevenueReportGenerated => "Batch revenue report generated.",
            LogEvent.OrderBacklogReportGenerated => "Order backlog report generated.",
            LogEvent.PrintableReportRequested => $"Printable report requested: {context}",
            LogEvent.FactoryOverviewReportGenerated => "Factory overview report generated.",
            LogEvent.StaffingReportGenerated => "Factory staffing report generated.",
            LogEvent.MachineFleetReportGenerated => "Factory machine fleet report generated.",
            LogEvent.InventoryReportGenerated => "Factory inventory report generated.",
            LogEvent.FullNameUpdated => $"Full name updated for user: {context}",
            LogEvent.UsernameUpdated => $"Username updated for user: {context}",
            LogEvent.PasswordUpdated => $"Password updated for user: {context}",
            LogEvent.PasswordChangedFirstLogin => $"First-time login password setup completed for user: {context}",
            LogEvent.OperationUndone => string.Format(UndoText.LogMessageFormat, context),
            _ => "Unknown event occurred"
        };

        WriteToFile("INFO", origin, message);
    }

    public void LogWarning(LogOrigin origin, LogEvent eventType, string context = "")
    {
        var message = eventType switch
        {
            LogEvent.ProductionInterrupted => $"Production operation interrupted for {context}",
            _ => "Unknown event occurred"
        };

        WriteToFile("WARN", origin, message);
    }

    public void LogError(string message)
    {
        WriteToFile("ERROR", LogOrigin.SYSTEM, message);
    }

    public void ShowOperationHistory()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule(History.Title).Centered());

        var logContent = fileService.ReadFromFile(LogFileName);
        if (string.IsNullOrEmpty(logContent))
        {
            AnsiConsole.MarkupLine(History.NoHistory);
            return;
        }

        var lines = logContent.Split([Environment.NewLine], StringSplitOptions.RemoveEmptyEntries);

        var table = new Table().Border(TableBorder.Rounded);
        table.AddColumn(History.ColumnTimestamp);
        table.AddColumn(History.ColumnLevel);
        table.AddColumn(History.ColumnOrigin);
        table.AddColumn(History.ColumnDescription);

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var firstClose = line.IndexOf(']');
            if (firstClose > 1)
            {
                var timestamp = line.Substring(1, firstClose - 1);
                var secondOpen = line.IndexOf('[', firstClose);
                var secondClose = line.IndexOf(']', firstClose + 1);
                if (secondOpen >= 0 && secondClose > secondOpen)
                {
                    var level = line.Substring(secondOpen + 1, secondClose - secondOpen - 1);

                    var thirdOpen = line.IndexOf('[', secondClose);
                    var thirdClose = line.IndexOf(']', secondClose + 1);
                    if (thirdOpen >= 0 && thirdClose > thirdOpen)
                    {
                        var origin = line.Substring(thirdOpen + 1, thirdClose - thirdOpen - 1);
                        var description = line.Substring(thirdClose + 1).Trim();
                        table.AddRow(Markup.Escape(timestamp), Markup.Escape(level), Markup.Escape(origin),
                            Markup.Escape(description));
                    }
                    else
                    {
                        var description = line.Substring(secondClose + 1).Trim();
                        table.AddRow(Markup.Escape(timestamp), Markup.Escape(level), "SYSTEM",
                            Markup.Escape(description));
                    }
                }
                else
                {
                    var description = line.Substring(firstClose + 1).Trim();
                    table.AddRow(Markup.Escape(timestamp), "INFO", "SYSTEM", Markup.Escape(description));
                }
            }
            else
            {
                table.AddRow("-", "-", "-", Markup.Escape(line));
            }
        }

        AnsiConsole.Write(table);
    }

    private void WriteToFile(string level, LogOrigin origin, string message)
    {
        var entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] [{origin}] {message}";
        fileService.AppendToFile(LogFileName, entry);
    }
}