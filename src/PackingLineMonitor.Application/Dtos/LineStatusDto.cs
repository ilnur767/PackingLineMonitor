namespace PackingLineMonitor.Application.Dtos;

public record LineStatusDto(string Status, DateTime Timestamp, double Speed);