using PackingLineMonitor.Application.Dtos;
using PackingLineMonitor.Application.Queries;

namespace PackingLineMonitor.Application.Abstractions;

public interface IGetLineStatsHandler
{
    /// <summary>
    ///     Получить агрегированные данные за период.
    /// </summary>
    /// <param name="query">Тело запроса.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Модель отображающую кол-во событий сгруппированных по статусам, время начала и окончания поиска.</returns>
    Task<LineStatsDto> Handle(GetLineStatsQuery query, CancellationToken cancellationToken);
}