using PackingLineMonitor.Infrastructure.Services;

namespace PackingLineMonitor.Presentation.BackgroundServices;

public sealed class LineEventBackgroundService : BackgroundService
{
    private readonly LineEventAggregationService _service;

    public LineEventBackgroundService(LineEventAggregationService service) => _service = service;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) =>
        await _service.ProcessEvents(stoppingToken);
}