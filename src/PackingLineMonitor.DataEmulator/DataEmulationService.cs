using Microsoft.Extensions.Logging;
using PackingLineMonitor.Application.Messaging;
using PackingLineMonitor.Domain;

namespace PackingLineMonitor.DataEmulator;

public sealed class DataEmulationService
{
    private readonly ILogger<DataEmulationService> _logger;
    private readonly IMessageQueue<LineCounterMeasurement> _messageQueue;
    private readonly int _noDataDelay = 40000;
    private readonly Random _random = new();
    private readonly TimeProvider _timeProvider;
    private int _currentDelay;
    private LineStatus _lineStatus = LineStatus.Running;
    private int _statusRepeate;

    public DataEmulationService(
        IMessageQueue<LineCounterMeasurement> messageQueue,
        TimeProvider timeProvider,
        ILogger<DataEmulationService> logger)
    {
        _messageQueue = messageQueue;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task StartEmulation(CancellationToken cancellationToken)
    {
        var counter = 1;
        const int counterMax = 10000;

        while (!cancellationToken.IsCancellationRequested)
        {
            var measurement = GenerateMeasurement(ref counter);
            if (measurement == null)
            {
                await Task.Delay(_noDataDelay, cancellationToken);

                continue;
            }

            await _messageQueue.WriteAsync(measurement, cancellationToken);

            _logger.LogDebug("Generated measurement: counter={Counter}, timestamp={Timestamp}",
                measurement.Counter,
                measurement.Timestamp);

            if (counter > counterMax)
            {
                counter = 1;
            }

            await Task.Delay(_currentDelay, cancellationToken);
        }
    }

    private LineCounterMeasurement? GenerateMeasurement(ref int counter)
    {
        if (_statusRepeate > 0)
        {
            _statusRepeate--;
            return GenerateForcedMeasurement(ref counter);
        }

        var r = _random.NextDouble();

        // NormalSpeed
        if (r < 0.50)
        {
            EnterStatusWithRepeat(LineStatus.Running);
            return CreateNormalSpeedMeasurement(ref counter);
        }

        // LowSpeed
        if (r < 0.65)
        {
            EnterStatusWithRepeat(LineStatus.LowSpeed);
            return CreateLowSpeedMeasurement(ref counter);
        }

        // Stop
        if (r < 0.8)
        {
            EnterStatusWithRepeat(LineStatus.Stopped);
            return CreateStopMeasurement(counter);
        }

        // NoData
        return null;
    }

    private LineCounterMeasurement GenerateForcedMeasurement(ref int counter) =>
        _lineStatus switch
        {
            LineStatus.Running => CreateNormalSpeedMeasurement(ref counter),
            LineStatus.LowSpeed => CreateLowSpeedMeasurement(ref counter),
            LineStatus.Stopped => CreateStopMeasurement(counter),
            _ => throw new ArgumentOutOfRangeException()
        };

    private LineCounterMeasurement CreateNormalSpeedMeasurement(ref int counter)
    {
        counter++;
        _currentDelay = _random.Next(390, 400);
        return new LineCounterMeasurement(counter, _timeProvider.GetUtcNow().UtcDateTime);
    }

    private LineCounterMeasurement CreateLowSpeedMeasurement(ref int counter)
    {
        counter++;
        _currentDelay = _random.Next(1500, 2001);
        return new LineCounterMeasurement(counter, _timeProvider.GetUtcNow().UtcDateTime);
    }

    private LineCounterMeasurement CreateStopMeasurement(int counter)
    {
        _currentDelay = _random.Next(300, 1001);
        return new LineCounterMeasurement(counter, _timeProvider.GetUtcNow().UtcDateTime);
    }

    private void EnterStatusWithRepeat(LineStatus newLineStatus)
    {
        _lineStatus = newLineStatus;
        _statusRepeate = _random.Next(10, 16);
    }
}

public record LineCounterMeasurement(int Counter, DateTime Timestamp);