using PackingLineMonitor.DataEmulator;

namespace PackingLineMonitor.Infrastructure.Queues;

public class LineCounterMeasurementQueue
{
    private readonly object _lock = new();
    private readonly int _maxSize;
    private readonly Queue<LineCounterMeasurement> _queue = new();

    public LineCounterMeasurementQueue(int maxSize) => _maxSize = maxSize;

    public void Enqueue(LineCounterMeasurement item)
    {
        lock (_lock)
        {
            if (_queue.Count == _maxSize)
            {
                _queue.Dequeue();
            }

            _queue.Enqueue(item);
        }
    }

    public LineCounterMeasurement[] GetItems()
    {
        lock (_lock)
        {
            return _queue.ToArray();
        }
    }

    public LineCounterMeasurement? GetLast()
    {
        lock (_lock)
        {
            return _queue.Count > 0 ? _queue.Last() : null;
        }
    }

    public LineCounterMeasurement? GetFirst()
    {
        lock (_lock)
        {
            return _queue.Count > 0 ? _queue.First() : null;
        }
    }

    public bool AllEqual()
    {
        lock (_lock)
        {
            if (_queue.Count <= 1)
            {
                return false;
            }

            var firstCounter = _queue.Peek().Counter;

            foreach (var item in _queue)
            {
                if (item.Counter != firstCounter)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public void Clear()
    {
        lock (_lock)
        {
            _queue.Clear();
        }
    }
}