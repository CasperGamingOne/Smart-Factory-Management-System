namespace Smart_Factory_Management_System;

public enum ReportStatus
{
    Pending,
    Fulfilled
}

public class ReportRequest
{
    public ReportRequest()
    {
    }

    public ReportRequest(string reportType, string requestedByDirectorName)
    {
        ReportType = reportType;
        RequestedByDirectorName = requestedByDirectorName;
    }

    public string RequestId { get; set; } = Guid.NewGuid().ToString().Substring(0, 8);
    public string ReportType { get; set; } = string.Empty;
    public string RequestedByDirectorName { get; set; } = string.Empty;
    public bool IsNotifiedComplete { get; set; }
    public string ExportDestination { get; set; } = "ReportsFolder";
    public ReportStatus Status { get; set; } = ReportStatus.Pending;

    public void Fulfill()
    {
        Status = ReportStatus.Fulfilled;
    }

    public override string ToString()
    {
        return $"[Req {RequestId}] {ReportType} (Requested by: {RequestedByDirectorName}) - Status: {Status}";
    }
}