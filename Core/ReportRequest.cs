namespace Smart_Factory_Management_System;

public enum ReportStatus
{
    Pending,
    Fulfilled
}

public class ReportRequest(string reportType, string requestedByDirectorName)
{
    public string RequestId { get; } = Guid.NewGuid().ToString().Substring(0, 8);
    public string ReportType { get; init; } = reportType;
    private string RequestedByDirectorName { get; } = requestedByDirectorName;
    public ReportStatus Status { get; private set; } = ReportStatus.Pending;

    public void Fulfill()
    {
        Status = ReportStatus.Fulfilled;
    }

    public override string ToString()
    {
        return $"[Req {RequestId}] {ReportType} (Requested by: {RequestedByDirectorName}) - Status: {Status}";
    }
}