using Microsoft.Extensions.DependencyInjection;
using PackingLineMonitor.Application.Abstractions;
using PackingLineMonitor.Application.Messaging;
using PackingLineMonitor.Domain;
using PackingLineMonitor.Infrastructure.Events;

namespace PackingLineMonitor.Infrastructure.Services;

public sealed class LineEventAggregationService
{
    private readonly IMessageQueue<LineStatusChangedEvent> _eventMessageQueue;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly TimeProvider _timeProvider;

    public LineEventAggregationService(
        IMessageQueue<LineStatusChangedEvent> eventMessageQueue,
        TimeProvider timeProvider,
        IServiceScopeFactory serviceScopeFactory)
    {
        _eventMessageQueue = eventMessageQueue;
        _timeProvider = timeProvider;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task ProcessEvents(CancellationToken cancellationToken)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();

        var lineEventRepository = scope.ServiceProvider.GetRequiredService<ILineEventRepository>();

        while (!cancellationToken.IsCancellationRequested)
        {
            var line = await _eventMessageQueue.ReadAsync(cancellationToken);

            await lineEventRepository.CreateEvent(new LineEvent(
                line.Status,
                line.Timestamp ?? _timeProvider.GetUtcNow().UtcDateTime), cancellationToken);
        }
    }
}