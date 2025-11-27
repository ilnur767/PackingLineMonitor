namespace PackingLineMonitor.Domain;

public class LineEvent
{
    public LineEvent(LineStatus status, DateTime timestamp)
    {
        Status = status;
        Timestamp = timestamp;
    }

    public Guid Id { get; set; }
    public LineStatus Status { get; set; }
    public DateTime Timestamp { get; set; }
}