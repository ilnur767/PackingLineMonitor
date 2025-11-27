using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PackingLineMonitor.Application.Abstractions;
using PackingLineMonitor.Domain;
using PackingLineMonitor.Infrastructure.Database;

namespace PackingLineMonitor.Infrastructure.Repositories;

public class LineEventRepository : ILineEventRepository
{
    private readonly PackingLineMonitorDbContext _dbContext;
    private readonly ILogger<PackingLineMonitorDbContext> _logger;

    public LineEventRepository(PackingLineMonitorDbContext dbContext, ILogger<PackingLineMonitorDbContext> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task CreateEvent(LineEvent lineEvent, CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.LineEvents.AddAsync(lineEvent, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }

    public async Task<IList<LineEvent>> GetEvents(DateTime from, DateTime to, CancellationToken cancellationToken)
    {
        var events = await _dbContext.LineEvents.Where(l => l.Timestamp >= from && l.Timestamp <= to)
            .ToListAsync(cancellationToken);

        return events;
    }
}