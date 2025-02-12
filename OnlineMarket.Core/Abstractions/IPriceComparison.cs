using OnlineMarket.Core.Models;

namespace OnlineMarket.Core.Abstractions
{
    public interface IPriceComparison
    {
        /// <summary>
        /// Получает список цен из разных магазинов.
        /// </summary>
        Task<List<CompetitorPrice>> GetCompetitorPricesAsync(string productName);
    }
}
