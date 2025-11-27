using PackingLineMonitor.Application.Dtos;

namespace PackingLineMonitor.Application.Abstractions;

public interface INormalizationService
{
    /// <summary>
    ///     Получить статус фасовочной линии.
    /// </summary>
    LineStatusDto GetLineStatus();

    /// <summary>
    ///     Запустить процесс обработки измерений.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task ProcessMeasurements(CancellationToken cancellationToken);
}