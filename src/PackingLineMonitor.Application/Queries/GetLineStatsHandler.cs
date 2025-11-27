using PackingLineMonitor.Application.Abstractions;
using PackingLineMonitor.Application.Dtos;
using PackingLineMonitor.Domain;

namespace PackingLineMonitor.Application.Queries;

public sealed class GetLineStatsHandler : IGetLineStatsHandler
{
    private readonly ILineEventRepository _repository;
    private readonly TimeProvider _timeProvider;


    public GetLineStatsHandler(ILineEventRepository repository,
        TimeProvider timeProvider)
    {
        _repository = repository;
        _timeProvider = timeProvider;
    }

    public async Task<LineStatsDto> Handle(GetLineStatsQuery query, CancellationToken cancellationToken)
    {
        var to = _timeProvider.GetUtcNow().UtcDateTime;
        var from = to.AddHours(-query.Period);
        var result = await _repository.GetEvents(from, to, cancellationToken);

        var eventsDictionary =
            result.GroupBy(l => l.Status).ToDictionary(g => g.Key, g => g.Count());

        eventsDictionary.TryGetValue(LineStatus.Running, out var running);
        eventsDictionary.TryGetValue(LineStatus.LowSpeed, out var lowSpeed);
        eventsDictionary.TryGetValue(LineStatus.Stopped, out var stopped);
        eventsDictionary.TryGetValue(LineStatus.NoData, out var noData);

        return new LineStatsDto
        {
            From = from,
            To = to,
            EventsCount = new EventsCountDto
            {
                Running = running, LowSpeed = lowSpeed, Stopped = stopped, NoData = noData
            }
        };
    }
}

public record GetLineStatsQuery(int Period);