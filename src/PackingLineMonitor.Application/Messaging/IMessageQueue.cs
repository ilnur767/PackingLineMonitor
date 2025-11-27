namespace PackingLineMonitor.Application.Messaging;

/// <summary>
///     Очередь сообщений.
/// </summary>
public interface IMessageQueue<TMessage>
{
    /// <summary>
    ///     Записать сообщение в очередь.
    /// </summary>
    /// <param name="message">Сообщение для записи в очередь.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task WriteAsync(TMessage message, CancellationToken cancellationToken);

    /// <summary>
    ///     Получить сообщение из очереди.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Элемент очереди.</returns>
    Task<TMessage> ReadAsync(CancellationToken cancellationToken);
}