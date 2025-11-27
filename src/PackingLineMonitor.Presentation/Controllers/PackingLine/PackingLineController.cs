using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PackingLineMonitor.Application.Abstractions;
using PackingLineMonitor.Application.Queries;

namespace PackingLineMonitor.Presentation.Controllers.PackingLine;

[ApiController]
[Route("api/line")]
public class PackingLineController : ControllerBase
{
    /// <summary>
    ///     Получить текущий статус линии.
    /// </summary>
    [HttpGet("status")]
    public IActionResult GetLineStatus(
        [FromServices] INormalizationService service)
    {
        var result = service.GetLineStatus();

        return Ok(result);
    }

    /// <summary>
    ///     Получить аггрегированную статистику линии.
    /// </summary>
    [HttpGet("stats")]
    public async Task<IActionResult> GetLineStats(
        [FromQuery] int period,
        [FromServices] IGetLineStatsHandler handler,
        [FromServices] IValidator<GetLineStatsQuery> validator,
        CancellationToken cancellationToken)
    {
        GetLineStatsQuery query = new(period);
        var validationResult = await validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var result = await handler.Handle(query, cancellationToken);

        return Ok(result);
    }
}