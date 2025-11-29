using Microsoft.Extensions.Logging;
using PackingLineMonitor.Application.Abstractions;
using PackingLineMonitor.Application.Dtos;
using PackingLineMonitor.Application.Messaging;
using PackingLineMonitor.DataEmulator;
using PackingLineMonitor.Domain;
using PackingLineMonitor.Infrastructure.Events;
using PackingLineMonitor.Infrastructure.Queues;

namespace PackingLineMonitor.Infrastructure.Services;

public sealed class NormalizationService : INormalizationService
{
    private const int MaxCount = 5;
    private const double NominalSpeed = 10000;
    private const double NominalSpeedLimit = NominalSpeed * 0.95;
    private readonly IMessageQueue<LineStatusChangedEvent> _eventMessageQueue;
    private readonly object _lock = new();
    private readonly ILogger<NormalizationService> _logger;
    private readonly IMessageQueue<LineCounterMeasurement> _measurementMessageQueue;
    private readonly LineCounterMeasurementQueue _queue = new(MaxCount);
    private readonly TimeProvider _timeProvider;
    private LineStatus _prevStatus;
    private double _speed;
    private LineStatus _status;

    public NormalizationService(
        IMessageQueue<LineCounterMeasurement> measurementMessageQueue,
        IMessageQueue<LineStatusChangedEvent> eventMessageQueue,
        ILogger<NormalizationService> logger, TimeProvider timeProvider)
    {
        _measurementMessageQueue = measurementMessageQueue;
        _logger = logger;
        _timeProvider = timeProvider;
        _eventMessageQueue = eventMessageQueue;
    }

    public LineStatusDto GetLineStatus() => new(_status.ToString(), _timeProvider.GetUtcNow().UtcDateTime, _speed);

    public async Task ProcessMeasurements(CancellationToken cancellationToken)
    {
        var calcTask = Task.Run(() => ExecutePeriodicCalculations(cancellationToken), cancellationToken);

        while (!cancellationToken.IsCancellationRequested)
        {
            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            try
            {
                var measurement = await _measurementMessageQueue.ReadAsync(timeoutCts.Token);
                _queue.Enqueue(measurement);

                Recalculate();
            }
            catch (OperationCanceledException)
            {
                RecalculateIfNoData();
            }
        }
    }

    private void RecalculateIfNoData()
    {
        lock (_lock)
        {
            _status = LineStatus.NoData;
            _speed = 0;
            _queue.Clear();
            TrackStatusChange();
        }
    }

    private void Recalculate()
    {
        if (_status == LineStatus.NoData)
        {
            Calculate(false);
            TrackStatusChange();
        }
    }

    private async Task? ExecutePeriodicCalculations(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            Calculate(true);
            TrackStatusChange();
            await Task.Delay(1000, cancellationToken);
        }
    }

    private void Calculate(bool periodic)
    {
        lock (_lock)
        {
            if (_status == LineStatus.NoData && periodic)
            {
                return;
            }

            var items = _queue.GetItems().DistinctBy(l => l.Counter).ToList();
            if (items.Count < MaxCount)
            {
                return;
            }

            if (_queue.AllEqual())
            {
                _status = LineStatus.Stopped;
                _speed = 0;
                return;
            }

            var elapsed = _timeProvider.GetUtcNow().UtcDateTime - _queue.GetFirst()!.Timestamp;
            _speed = Math.Round(items.Count / elapsed.TotalSeconds * 3600.0, 1);

            _logger.LogDebug("Line speed: {Speed}", _speed);

            if (_speed < NominalSpeedLimit)
            {
                _status = LineStatus.LowSpeed;
                return;
            }

            _status = LineStatus.Running;
        }
    }

    private void TrackStatusChange()
    {
        if (_status != _prevStatus)
        {
            _eventMessageQueue.WriteAsync(
                new LineStatusChangedEvent(_status, _queue.GetLast()?.Timestamp),
                CancellationToken.None);
            _prevStatus = _status;
        }
    }
}