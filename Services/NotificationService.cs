using System.Text;
using Spectre.Console;

namespace Smart_Factory_Management_System;

public class NotificationService : INotificationService
{
    public void CheckAndShowNotifications(Employee user, Factory factory)
    {
        if (user is Technician tech)
        {
            var unassigned = factory.PendingOrders.Where(o => o.AssignedTechnicianId == -1).ToList();
            if (unassigned.Count > 0)
            {
                AnsiConsole.WriteLine();
                var content = new StringBuilder();
                foreach (var o in unassigned)
                {
                    o.AssignedTechnicianId = tech.Id;
                    var displayName = string.IsNullOrEmpty(o.Name) ? o.ProductName : o.Name;
                    content.AppendLine(string.Format(Production.AutoAssignNotification, o.OrderId, displayName,
                        o.Quantity));
                }

                var notifyPanel = new Panel(content.ToString().TrimEnd())
                {
                    Border = BoxBorder.Rounded,
                    Header = new PanelHeader($"[yellow]{Production.AutoAssignHeader}[/]")
                };
                AnsiConsole.Write(notifyPanel);
                AnsiConsole.MarkupLine(Production.AutoAssignAcknowledge);
                Console.ReadKey(true);
            }
        }
        else if (user is Accountant)
        {
            var pendingRequests = factory.PendingReportRequests.Where(r => r.Status == ReportStatus.Pending).ToList();
            if (pendingRequests.Count > 0)
            {
                AnsiConsole.WriteLine();
                var content = new StringBuilder();
                foreach (var r in pendingRequests)
                    content.AppendLine(string.Format(Accounting.PendingReportNotification, r.RequestId, r.ReportType));

                var notifyPanel = new Panel(content.ToString().TrimEnd())
                {
                    Border = BoxBorder.Rounded,
                    Header = new PanelHeader($"[yellow]{Accounting.PendingReportHeader}[/]")
                };
                AnsiConsole.Write(notifyPanel);
                AnsiConsole.MarkupLine(Accounting.PendingReportAcknowledge);
                Console.ReadKey(true);
            }
        }
        else if (user is SalesAgent sales)
        {
            var completedOrders = factory.PendingOrders
                .Where(o => o.IsComplete && o.PlacedBy.Equals(sales.Username, StringComparison.OrdinalIgnoreCase) &&
                            !o.IsNotifiedComplete)
                .ToList();

            if (completedOrders.Count > 0)
            {
                AnsiConsole.WriteLine();
                var content = new StringBuilder();
                foreach (var o in completedOrders)
                {
                    o.IsNotifiedComplete = true;
                    var displayName = string.IsNullOrEmpty(o.Name) ? o.ProductName : o.Name;
                    content.AppendLine(string.Format(Sales.OrderCompleteNotification, o.OrderId, displayName,
                        o.Quantity));
                }

                var notifyPanel = new Panel(content.ToString().TrimEnd())
                {
                    Border = BoxBorder.Rounded,
                    Header = new PanelHeader($"[yellow]{Sales.OrderCompleteHeader}[/]")
                };
                AnsiConsole.Write(notifyPanel);
                AnsiConsole.MarkupLine(Sales.OrderCompleteAcknowledge);
                Console.ReadKey(true);
            }
        }
        else if (user is Director director)
        {
            var readyReports = factory.PendingReportRequests
                .Where(r => r.Status == ReportStatus.Fulfilled &&
                            r.RequestedByDirectorName.Equals(director.Name, StringComparison.OrdinalIgnoreCase) &&
                            !r.IsNotifiedComplete)
                .ToList();

            if (readyReports.Count > 0)
            {
                AnsiConsole.WriteLine();
                var content = new StringBuilder();
                foreach (var r in readyReports)
                {
                    r.IsNotifiedComplete = true;
                    content.AppendLine(string.Format(Reports.ReportFulfilledNotification, r.ReportType, r.RequestId));
                }

                var notifyPanel = new Panel(content.ToString().TrimEnd())
                {
                    Border = BoxBorder.Rounded,
                    Header = new PanelHeader($"[yellow]{Reports.ReportFulfilledHeader}[/]")
                };
                AnsiConsole.Write(notifyPanel);
                AnsiConsole.MarkupLine(Reports.ReportFulfilledAcknowledge);
                Console.ReadKey(true);
            }
        }
    }
}