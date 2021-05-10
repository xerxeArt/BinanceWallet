using Data.BinanceApi;
using Data.Data;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Business
{
    public class DataGathering : IDataGathering
    {
        private readonly ILogger<DataGathering> _logger;
        private readonly IApiRequest _apiRequests;

        private static object _lock = new object();

        public DataGathering(ILogger<DataGathering> logger, IApiRequest apiRequests)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _apiRequests = apiRequests ?? throw new ArgumentNullException(nameof(apiRequests));
        }

        public async Task<List<Asset>> GetWalletAssets()
        {
            var walletAssets = new List<Asset>();

            var result = await _apiRequests.PerformGetRequest("/api/v3/account", null, true);
            if (string.IsNullOrEmpty(result)) return walletAssets;
            var jsonResult = JToken.Parse(result);

            var nonZeroAssets = jsonResult.SelectTokens("$.balances[?(@.free != '0.00000000')]");
            foreach (var asset in nonZeroAssets)
            {
                var assetName = asset.SelectToken("asset").Value<string>();
                var assetValue = asset.SelectToken("free").Value<decimal>() + asset.SelectToken("locked").Value<decimal>();
                if (assetValue == 0) continue;
                walletAssets.Add(new Asset
                {
                    Name = assetName,
                    WalletHolding = assetValue
                });
            }
            return walletAssets;
        }

        public async Task<List<Asset>> GetAssetPrices(List<string> assets)
        {
            string market = string.Empty;
            var assetPrices = new List<Asset>();
            foreach (var asset in assets)
            {
                try
                {
                    switch (asset)
                    {
                        case "BTC":
                            market = "EUR";
                            break;
                        case "DENT":
                        case "WIN":
                            market = "USDT";
                            break;
                        case "USDT":
                        case "EUR":
                            continue;
                        default:
                            market = "BTC";
                            break;
                    }

                    _logger.LogWarning($"Retrieving {asset}");
                    var result = await DoGetRequest(asset, market);
                    if (string.IsNullOrEmpty(result)) return assetPrices;

                    var jsonResult = JToken.Parse(result);
                    decimal price;
                    decimal.TryParse(jsonResult["price"].Value<string>(), out price);
                    _logger.LogWarning($"Found {asset}={price}");
                    assetPrices.Add(new Asset
                    {
                        Name = asset,
                        Market = market,
                        CurrentValue = price
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error for getting asset {asset} on market {market}: {ex.Message}");
                }
            }

            return assetPrices;
        }

        public async Task<List<Asset>> GetAssetPricesAsync(List<Asset> assets)
        {
            var assetPrices = new List<Asset>();
            var tasks = new List<Task>();
            foreach (var asset in assets)
            {
                try
                {
                    _logger.LogWarning($"Retrieving {asset.Name}_{asset.Market}");
                    var task = Task.Factory.StartNew(async () => { return await DoGetRequest(asset.Name, asset.Market); });
                    var task2 = await task.ContinueWith(async (resultTask) =>
                    {
                        //Something fishy here: if I change the 
                        var result = await resultTask;
                        var resultAsset = await ParseResult(result, assets);
                        if (resultAsset == null) return;
                        lock (_lock)
                        {
                            assetPrices.Add(resultAsset);
                        }
                    });
                    tasks.Add(task2);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error for getting asset {asset.Name} on market {asset.Market}: {ex.Message}");
                }
            }

            var results = Task.WhenAll(tasks);

            try
            {
                await results;
            }
            catch { }

            if (results.Status == TaskStatus.RanToCompletion)
                _logger.LogWarning("All requests successful.");
            else if (results.Status == TaskStatus.Faulted)
                _logger.LogError("One ore more requests failed");

            return assetPrices;
        }

        private async Task<string> DoGetRequest(string asset, string market)
        {
            return await _apiRequests.PerformGetRequest("/api/v3/ticker/price", new Dictionary<string, string> { { "symbol", $"{asset}{market}" } }, false);
        }

        private async Task<Asset> ParseResult(Task<string> result, List<Asset> assets)
        {
            var stringResult = await result;
            if (string.IsNullOrEmpty(stringResult)) return null;

            var jsonResult = JToken.Parse(stringResult);
            decimal price;
            decimal.TryParse(jsonResult["price"].Value<string>(), out price);
            var symbol = jsonResult["symbol"].Value<string>();

            var asset = assets.FirstOrDefault(x => $"{x.Name}{x.Market}" == symbol);
            if (asset == null) return null;


            _logger.LogWarning($"Found {asset.Name}={price}");

            return new Asset
            {
                Name = asset.Name,
                Market = asset.Market,
                CurrentValue = price,
                BuyPriceWeightedAvg = asset.BuyPriceWeightedAvg
            };
        }

        private async Task<Asset> ParseResult(Task<string> result, string asset, string market)
        {
            var stringResult = await result;
            if (string.IsNullOrEmpty(stringResult)) return null;

            var jsonResult = JToken.Parse(stringResult);
            decimal price;
            decimal.TryParse(jsonResult["price"].Value<string>(), out price);
            var symbol = jsonResult["symbol"].Value<string>();
            _logger.LogWarning($"Found {asset}={price}");

            return new Asset
            {
                Name = asset,
                Market = market,
                CurrentValue = price
            };
        }
    }
}
