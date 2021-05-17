using Data.Data;
using System;
using System.Collections.Generic;

namespace BinanceWallet.ViewModels
{
    public class AssetInventoryViewModel
    {
        public decimal BtcEurValue { get; }
        public decimal BtcBusdValue { get; }

        protected List<AssetInventoryInformationViewModel> Assets { get; set; }
        public AssetInventoryViewModel(decimal btcEurValue, decimal btcBusdValue)
        {
            Assets = new List<AssetInventoryInformationViewModel>();
            BtcEurValue = btcEurValue;
            BtcBusdValue = btcBusdValue;
        }

        public List<AssetInventoryInformationViewModel> GetAssets()
        {
            return Assets;
        }

        public void AddAsset(Asset asset)
        {
            decimal profitRatio = asset.BuyPriceWeightedAvg == 0 ? 0 : (asset.CurrentValue - asset.BuyPriceWeightedAvg) / asset.BuyPriceWeightedAvg;

            var newAsset = new AssetInventoryInformationViewModel
            {
                AssetName = asset.Name,
                Market = asset.Market,
                CurrentPrice = asset.CurrentValue,
                CurrentPriceConverted = asset.CurrentFiatValue,
                HoldingCount = asset.WalletHolding,
                HoldingAmount = asset.CurrentValue * asset.WalletHolding,
                HoldingAmountConverted = asset.CurrentFiatValue * asset.WalletHolding,
                BuyPriceWeightedAvg = asset.BuyPriceWeightedAvg,
                ProfitPercent = profitRatio * 100,
            };

            if (asset.Market == "BUSD" && asset.Name != "BTC")
            {
                newAsset.HoldingAmount = asset.WalletHolding * asset.CurrentValue / BtcBusdValue;
                newAsset.HoldingAmountConverted = newAsset.HoldingAmount * BtcEurValue;
            }

            if (asset.Name == "BTC")
            {
                newAsset.HoldingAmount = asset.WalletHolding;
                newAsset.HoldingAmountConverted = asset.CurrentFiatValue * asset.WalletHolding;
                newAsset.ProfitValue = newAsset.HoldingCount / (1 + profitRatio) * profitRatio;
            }
            else
            {
                newAsset.ProfitValue = newAsset.HoldingAmount / (1 + profitRatio) * profitRatio;
            }
            newAsset.ProfitValueConverted = newAsset.HoldingAmountConverted / (1 + profitRatio) * profitRatio;
            newAsset.RgbColor = GetRgbPercentage(newAsset.ProfitPercent);

            Assets.Add(newAsset);
        }

        private string GetRgbPercentage(decimal percent)
        {
            int r, g, b = 0;
            if (percent <= 0) r = 255;
            else r = (int)Math.Floor(255 * (100 - 2.5m * percent) / 100);

            if (percent >= 0) g = 255;
            else g = (int)Math.Floor(255 * (100 + 2.5m * percent) / 100);

            if (r > 255) r = 255; else if (r < 0) r = 0;
            if (g > 255) g = 255; else if (g < 0) g = 0;
            if (b > 255) b = 255; else if (b < 0) b = 0;

            return $"{r:X2}{g:X2}{b:X2}";
        }
    }
}
