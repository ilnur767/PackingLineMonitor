using PackingLineMonitor.Domain;

namespace PackingLineMonitor.Application.Abstractions;

public interface ILineEventRepository
{
    /// <summary>
    ///     Создать событие изменения статуса линии.
    /// </summary>
    /// <param name="lineEvent">Тело события</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task CreateEvent(LineEvent lineEvent, CancellationToken cancellationToken);

    /// <summary>
    ///     Получить события на заданный инетервал времени.
    /// </summary>
    /// <param name="from">Время начала.</param>
    /// <param name="to">Время окончания.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Коллекцию событий.</returns>
    Task<IList<LineEvent>> GetEvents(DateTime from, DateTime to, CancellationToken cancellationToken);
}