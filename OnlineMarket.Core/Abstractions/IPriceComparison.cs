using OnlineMarket.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
