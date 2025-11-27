namespace PackingLineMonitor.Application.Dtos;

public class LineStatsDto
{
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public required EventsCountDto EventsCount { get; set; }
}