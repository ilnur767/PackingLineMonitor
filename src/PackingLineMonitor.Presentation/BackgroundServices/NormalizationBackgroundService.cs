using PackingLineMonitor.Application.Abstractions;

namespace PackingLineMonitor.Presentation.BackgroundServices;

public sealed class NormalizationBackgroundService : BackgroundService
{
    private readonly INormalizationService _service;

    public NormalizationBackgroundService(INormalizationService service) => _service = service;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) =>
        await _service.ProcessMeasurements(stoppingToken);
}