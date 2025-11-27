using FluentValidation;

namespace PackingLineMonitor.Application.Queries;

public class GetLineStatsQueryValidator : AbstractValidator<GetLineStatsQuery>
{
    private const int MAX_PERIOD = 24;

    public GetLineStatsQueryValidator() => RuleFor(x => x.Period).Must(p => p <= MAX_PERIOD);
}