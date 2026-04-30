using FinancialDataTracker.Entities.Concrete.DTOs;

namespace FinancialDataTracker.Business.Abstract;

public interface IAnalyticsService
{
    Task<IReadOnlyList<TopMoverDto>> GetTopMoversAsync(string direction, int limit = 5, CancellationToken cancellationToken=default);
}
