﻿using Data.Data;
using Data.EF.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.EF.Repositories
{
    public class BinanceRepository : BaseRepository<BinanceContext>, IBinanceRepository
    {
        public BinanceRepository(BinanceContext context) : base(context) { }

        public async Task InsertAssets(List<Asset> assets)
        {
            CalculateCurrentFiatPrices(assets);
            _context.Assets.AddRange(assets);
            await _context.SaveChangesAsync();
        }

        public List<Asset> GetAssets()
        {
            return _context.Assets.ToList();
        }

        public List<Tuple<string, string>> GetAssetNames()
        {
            var list = _context.Assets.Select(x => new Tuple<string, string>(x.Name, x.Market)).ToList();

            return list;
        }

        public async Task PopulateCurrentPrices(List<Asset> assetCurrentValues)
        {
            CalculateCurrentFiatPrices(assetCurrentValues);
            var assets = _context.Assets.ToList();

            foreach (var currVal in assetCurrentValues)
            {
                var asset = assets.FirstOrDefault(x => x.Name == currVal.Name && x.Market == currVal.Market);
                if (asset != null)
                {
                    asset.CurrentValue = currVal.CurrentValue;
                    asset.CurrentFiatValue = currVal.CurrentFiatValue;
                    if (currVal.BuyPriceWeightedAvg != 0m) asset.BuyPriceWeightedAvg = currVal.BuyPriceWeightedAvg;
                }
                else
                {
                    _context.Assets.Add(currVal);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task PopulateWalletHoldings(List<Asset> assetWalletValues)
        {
            var assets = _context.Assets.ToList();

            foreach (var currVal in assetWalletValues)
            {
                var asset = assets.FirstOrDefault(x => x.Name == currVal.Name);
                if (asset != null)
                {
                    asset.WalletHolding = currVal.WalletHolding;
                }
            }

            //Clear 0 assets
            foreach(var currAsset in assets)
            {
                var asset = assetWalletValues.FirstOrDefault(x => x.Name == currAsset.Name);
                if (asset == null)
                {
                    currAsset.WalletHolding = 0;
                }
            }

            await _context.SaveChangesAsync();
        }

        private void CalculateCurrentFiatPrices(List<Asset> assets)
        {
            string market, assetName;
            var marketValueBtcEur = assets.FirstOrDefault(x => x.Name == "BTC" && x.Market == "EUR")?.CurrentValue;
            var marketValueBtcUsdt = assets.FirstOrDefault(x => x.Name == "BTC" && x.Market == "USDT")?.CurrentValue;
            decimal? marketValue;

            foreach (var asset in assets)
            {
                market = asset.Market;
                assetName = asset.Name;
                if (asset.Name == "BTC" && asset.Market == "EUR") marketValue = 1;
                else if (asset.Name == "BTC" && asset.Market == "USDT") marketValue = marketValueBtcEur / marketValueBtcUsdt;
                else if (asset.Market == "USDT") marketValue = marketValueBtcUsdt;
                else if (asset.Market == "BTC") marketValue = marketValueBtcEur;
                else marketValue = assets.FirstOrDefault(x => x.Name == assetName && x.Market == market)?.CurrentValue;

                if (marketValue == null) continue;

                if (asset.Name != "BTC" && asset.Market == "USDT") asset.CurrentFiatValue = marketValueBtcEur / marketValueBtcUsdt ?? 0;
                else if (asset.Market == "USDT")
                {
                    asset.CurrentFiatValue = asset.WalletHolding * marketValueBtcUsdt ?? 0;
                }
                else
                {
                    asset.CurrentFiatValue = asset.CurrentValue * marketValue ?? 0;
                }
            }
        }
    }
}
