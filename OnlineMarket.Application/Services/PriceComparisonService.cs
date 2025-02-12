using OnlineMarket.Core.Abstractions;
using OnlineMarket.Core.Models;

namespace OnlineMarket.Application.Services
{
    public class PriceComparisonService : IPriceComparison
    {
        public async Task<List<CompetitorPrice>> GetCompetitorPricesAsync(string productName)
        {
            var competitorPrices = new List<CompetitorPrice>();

            var tasks = new List<Task<CompetitorPrice>>
            {
                ParseYandexMarket(productName),
                ParseAliExpress(productName),
                ParseOzon(productName),
                ParseWildberries(productName),
                ParseDNS(productName)
            };

            var results = await Task.WhenAll(tasks);

            foreach (var result in results)
            {
                if (result != null)
                    competitorPrices.Add(result);
            }

            return competitorPrices;
        }

        private async Task<CompetitorPrice> ParseYandexMarket(string productName)
        {
            return new CompetitorPrice
            {
                PlatformName = "Яндекс.Маркет",
                Url = "https://market.yandex.ru/",
                LastUpdated = DateTime.UtcNow
            };
            //var searchUrl = $"https://yandex.ru/products/api/ext/partner/feeds-info";
            //var newCompetitorPrice = new CompetitorPrice();
            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        client.DefaultRequestHeaders.Add("Authorization", $"OAuth y0__xCXgdihARjl8DQgtrSHlxLntd4vRwkGt-F8jK5eES78IjCvEg");
            //        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            //        var response = await client.GetAsync(searchUrl);

            //        if (!response.IsSuccessStatusCode)
            //        {
            //            newCompetitorPrice.PlatformName = $"Ошибка: {response}";
            //            return newCompetitorPrice;
            //        }

            //        string responseInfo = await response.Content.ReadAsStringAsync();

            //        var products = JsonConvert.DeserializeObject<dynamic>(responseInfo);

            //        if (products != null)
            //        {
            //            newCompetitorPrice.PlatformName = $"Product: {products}";
            //            return newCompetitorPrice;
            //        }

            //        var firstProduct = products?.products[0];

            //        if (firstProduct != null)
            //        {
            //            decimal price = firstProduct.price.value;
            //            string productLink = firstProduct.url;

            //            newCompetitorPrice.PlatformName = "Яндекс.Маркет";
            //            newCompetitorPrice.Price = price;
            //            newCompetitorPrice.Url = productLink;
            //            newCompetitorPrice.LastUpdated = DateTime.Now;
            //            return newCompetitorPrice;
            //        }
            //        else
            //        {
            //            newCompetitorPrice.PlatformName = "Яндекс.Маркет не нашел такой продукт";
            //            return newCompetitorPrice;
            //        }
            //    }
            //}
            //catch (HttpRequestException ex)
            //{
            //    newCompetitorPrice.PlatformName = $"Ошибка при отправке запроса: {ex.Message}";
            //    return newCompetitorPrice;
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"Ошибка при получении данных с Яндекс.Маркет: {ex.Message}");
            //}

            //return null;
        }

        private async Task<CompetitorPrice> ParseAliExpress(string productName)
        {
            //string searchUrl = $"https://www.aliexpress.com/wholesale?SearchText={Uri.EscapeDataString(productName)}";

            //var web = new HtmlWeb();
            //var doc = await web.LoadFromWebAsync(searchUrl);

            //var productNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'list-item')]");
            //if (productNode == null) return null;

            //var priceNode = productNode.SelectSingleNode(".//span[contains(@class, 'price')]");
            //var linkNode = productNode.SelectSingleNode(".//a[contains(@class, 'title')]");

            //if (priceNode == null || linkNode == null) return null;

            return new CompetitorPrice
            {
                PlatformName = "AliExpress",
                Url = "https://aliexpress.ru/?spm=a2g2w.productlist.logo.1.715aa6a2a23OFZ",
                LastUpdated = DateTime.UtcNow
            };
        }

        private async Task<CompetitorPrice> ParseOzon(string productName)
        {
            //    string searchUrl = $"https://www.ozon.ru/search/?text={Uri.EscapeDataString(productName)}";

            //    var web = new HtmlWeb();
            //    var doc = await web.LoadFromWebAsync(searchUrl);

            //    var productNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'tile')]");

            //    if (productNode == null) return null;

            //    var priceNode = productNode.SelectSingleNode(".//span[contains(@class, 'price')]");
            //    var linkNode = productNode.SelectSingleNode(".//a[contains(@class, 'tile-hover-target')]");

            //    if (priceNode == null || linkNode == null) return null;

            return new CompetitorPrice
            {
                PlatformName = "Ozon",
                Url = "https://www.ozon.ru",
                LastUpdated = DateTime.UtcNow
            };
        }

        private async Task<CompetitorPrice> ParseWildberries(string productName)
        {
            //string searchUrl = $"https://www.wildberries.ru/catalog/0/search.aspx?search={Uri.EscapeDataString(productName)}";

            //var web = new HtmlWeb();
            //var doc = await web.LoadFromWebAsync(searchUrl);

            //var productNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'product-card')]");

            //if (productNode == null) return null;

            //var priceNode = productNode.SelectSingleNode(".//ins[contains(@class, 'price')]");
            //var linkNode = productNode.SelectSingleNode(".//a[contains(@class, 'card__link')]");

            //if (priceNode == null || linkNode == null) return null;

            return new CompetitorPrice
            {
                PlatformName = "Wildberries",
                Url = "https://www.wildberries.ru",
                LastUpdated = DateTime.UtcNow
            };
        }
        private async Task<CompetitorPrice> ParseDNS(string productName)
        {
            //string searchUrl = $"https://www.dns-shop.ru/search/?q={Uri.EscapeDataString(productName)}";

            //var web = new HtmlWeb();
            //var doc = await web.LoadFromWebAsync(searchUrl);

            //var productNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'catalog-product')]");

            //if (productNode == null) return null;

            //var priceNode = productNode.SelectSingleNode(".//div[contains(@class, 'product-buy__price')]");
            //var linkNode = productNode.SelectSingleNode(".//a[contains(@class, 'catalog-product__name')]");

            //if (priceNode == null || linkNode == null) return null;

            return new CompetitorPrice
            {
                PlatformName = "DNS",
                Url = "https://www.dns-shop.ru",
                LastUpdated = DateTime.UtcNow
            };
        }

    }
}
