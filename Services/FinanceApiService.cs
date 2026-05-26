using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace PortfolioTracker.Services
{
    public class FinanceApiService
    {
        public async Task<(decimal price, string name)> GetAssetDataAsync(string symbol)
        {
            decimal price = 0;
            string name = symbol;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
                string url = string.Format(Constants.YahooFinanceApiUrl, symbol);
                string response = await client.GetStringAsync(url);

                using (JsonDocument doc = JsonDocument.Parse(response))
                {
                    var root = doc.RootElement;
                    var result = root.GetProperty("chart").GetProperty("result")[0];
                    var meta = result.GetProperty("meta");

                    if (meta.TryGetProperty("regularMarketPrice", out JsonElement priceElement))
                        price = priceElement.GetDecimal();
                    else
                        throw new Exception("Nu s-a putut găsi prețul curent pe piață.");

                    if (meta.TryGetProperty("shortName", out JsonElement nameElement))
                        name = nameElement.GetString() ?? symbol;
                }
            }

            return (price, name);
        }
        
        public async Task<decimal> GetAssetPriceAsync(string symbol)
        {
            var data = await GetAssetDataAsync(symbol);
            return data.price;
        }
    }
}
