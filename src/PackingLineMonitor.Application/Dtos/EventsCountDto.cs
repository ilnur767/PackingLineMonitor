namespace PackingLineMonitor.Application.Dtos;

public class EventsCountDto
{
    public int Running { get; set; }
    public int Stopped { get; set; }
    public int LowSpeed { get; set; }
    public int NoData { get; set; }
}