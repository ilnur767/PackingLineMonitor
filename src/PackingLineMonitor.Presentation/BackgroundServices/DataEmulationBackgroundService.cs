using PackingLineMonitor.DataEmulator;

namespace PackingLineMonitor.Presentation.BackgroundServices;

public sealed class DataEmulationBackgroundService : BackgroundService
{
    private readonly DataEmulationService _service;

    public DataEmulationBackgroundService(DataEmulationService service) => _service = service;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) =>
        await _service.StartEmulation(stoppingToken);
}