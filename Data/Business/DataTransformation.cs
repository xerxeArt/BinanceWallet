using Data.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Data.Business
{
    public class DataTransformation : IDataTransformation
    {
        public List<Asset> GetAssets(List<Asset> currentValues, List<Asset> walletValues)
        {
            var assets = new List<Asset>();
            foreach (var currentValue in currentValues)
            {
                var walletValue = walletValues.FirstOrDefault(x => x.Name == currentValue.Name && x.Market == currentValue.Market);
                assets.Add(new Asset
                {
                    Name = currentValue.Name,
                    Market = currentValue.Market,
                    CurrentValue = currentValue.CurrentValue,
                    WalletHolding = walletValue?.WalletHolding ?? 0,
                    BuyPriceWeightedAvg = walletValue?.BuyPriceWeightedAvg ?? 0
                });
            }

            return assets;
        }

        public List<Asset> PopulateInitialOfflineData(List<Asset> assets)
        {
            var myAssets = new List<Asset>();

            string fileContents;
            using (var stream = new FileStream(@"D:\SharedFolder\Coins.csv", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var reader = new StreamReader(stream))
                {
                    fileContents = reader.ReadToEnd();
                }
            }
            var coins = fileContents.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
            int row = 0;
            foreach (var coin in coins)
            {
                //skip the first row
                if (row++ == 0) continue;
                var splitCoin = coin.Split(",");
                decimal price;
                decimal.TryParse(splitCoin[2], out price);

                var asset = assets.FirstOrDefault(x => x.Name == splitCoin[0]);
                var myAsset = new Asset
                {
                    Name = splitCoin[0],
                    Market = splitCoin[1],
                    BuyPriceWeightedAvg = price,
                    WalletHolding = asset?.WalletHolding ?? 0
                };
                myAssets.Add(myAsset);
            }

            return myAssets;
        }
    }
}
