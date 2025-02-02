using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OnlineMarket.Core.Abstractions;
using OnlineMarket.Core.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;

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
                //ParseAliExpress(productName),
                //ParseOzon(productName),
                //ParseWildberries(productName),
                //ParseDNS(productName)
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
            var searchUrl = $"https://api.market.yandex.ru/v2/products.json?text={Uri.EscapeDataString(productName)}&client_id=ecb95142a8f54d7382ad7c48863f193c&client_secret=2fb29b8c62ea4396b07f68a5f7a9b719";

            try
            {
                var client = new HttpClient();
                var response = await client.GetStringAsync(searchUrl);
                var products = JsonConvert.DeserializeObject<dynamic>(response);

                // Примерный парсинг ответа, в зависимости от формата
                var firstProduct = products?.items[0];

                if (firstProduct != null)
                {
                    decimal price = firstProduct.price.value;
                    string productLink = firstProduct.link;

                    return new CompetitorPrice
                    {
                        PlatformName = "Яндекс.Маркет",
                        Price = price,
                        Url = productLink,
                        LastUpdated = DateTime.UtcNow
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении данных с Яндекс.Маркет: {ex.Message}");
            }

            return null;
        }

        public class ProductList
        {
            public List<ProductItem> Items { get; set; }  // Список товаров

            public ProductList()
            {
                Items = new List<ProductItem>();
            }
        }

        public class ProductItem
        {
            public string Title { get; set; }  // Название товара
            public decimal Price { get; set; }  // Цена товара
            public string Url { get; set; }  // Ссылка на товар
            public string ImageUrl { get; set; }  // URL изображения товара
            public string SellerName { get; set; }  // Название продавца (для AliExpress)
            public string Brand { get; set; }  // Бренд товара (для Ozon, Wildberries)
        }

        private async Task<CompetitorPrice> ParseAliExpress(string productName)
        {
            string searchUrl = $"https://www.aliexpress.com/wholesale?SearchText={Uri.EscapeDataString(productName)}";

            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(searchUrl);

            var productNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'list-item')]");
            if (productNode == null) return null;

            var priceNode = productNode.SelectSingleNode(".//span[contains(@class, 'price')]");
            var linkNode = productNode.SelectSingleNode(".//a[contains(@class, 'title')]");

            if (priceNode == null || linkNode == null) return null;

            return new CompetitorPrice
            {
                PlatformName = "AliExpress",
                Price = decimal.Parse(priceNode.InnerText.Replace("$", "").Replace(",", ".")),
                Url = "https://" + linkNode.GetAttributeValue("href", "").TrimStart('/'),
                LastUpdated = DateTime.UtcNow
            };
        }

        private async Task<CompetitorPrice> ParseOzon(string productName)
        {
            string searchUrl = $"https://www.ozon.ru/search/?text={Uri.EscapeDataString(productName)}";

            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(searchUrl);

            var productNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'tile')]");

            if (productNode == null) return null;

            var priceNode = productNode.SelectSingleNode(".//span[contains(@class, 'price')]");
            var linkNode = productNode.SelectSingleNode(".//a[contains(@class, 'tile-hover-target')]");

            if (priceNode == null || linkNode == null) return null;

            return new CompetitorPrice
            {
                PlatformName = "Ozon",
                Price = decimal.Parse(priceNode.InnerText.Replace(" ", "").Replace("₽", "")),
                Url = "https://www.ozon.ru" + linkNode.GetAttributeValue("href", ""),
                LastUpdated = DateTime.UtcNow
            };
        }

        private async Task<CompetitorPrice> ParseWildberries(string productName)
        {
            string searchUrl = $"https://www.wildberries.ru/catalog/0/search.aspx?search={Uri.EscapeDataString(productName)}";

            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(searchUrl);

            var productNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'product-card')]");

            if (productNode == null) return null;

            var priceNode = productNode.SelectSingleNode(".//ins[contains(@class, 'price')]");
            var linkNode = productNode.SelectSingleNode(".//a[contains(@class, 'card__link')]");

            if (priceNode == null || linkNode == null) return null;

            return new CompetitorPrice
            {
                PlatformName = "Wildberries",
                Price = decimal.Parse(priceNode.InnerText.Replace(" ", "").Replace("₽", "")),
                Url = "https://www.wildberries.ru" + linkNode.GetAttributeValue("href", ""),
                LastUpdated = DateTime.UtcNow
            };
        }
        private async Task<CompetitorPrice> ParseDNS(string productName)
        {
            string searchUrl = $"https://www.dns-shop.ru/search/?q={Uri.EscapeDataString(productName)}";

            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(searchUrl);

            var productNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'catalog-product')]");

            if (productNode == null) return null;

            var priceNode = productNode.SelectSingleNode(".//div[contains(@class, 'product-buy__price')]");
            var linkNode = productNode.SelectSingleNode(".//a[contains(@class, 'catalog-product__name')]");

            if (priceNode == null || linkNode == null) return null;

            return new CompetitorPrice
            {
                PlatformName = "DNS",
                Price = decimal.Parse(priceNode.InnerText.Replace(" ", "").Replace("₽", "")),
                Url = "https://www.dns-shop.ru" + linkNode.GetAttributeValue("href", ""),
                LastUpdated = DateTime.UtcNow
            };
        }

    }
}
