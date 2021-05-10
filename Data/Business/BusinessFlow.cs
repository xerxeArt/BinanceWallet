using Data.Data;
using Data.EF.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Business
{
    public class BusinessFlow : IBusinessFlow
    {
        private readonly IDataGathering _dataGathering;
        private readonly IDataTransformation _dataTransformation;
        private readonly IBinanceRepository _binanceRepository;

        public BusinessFlow(IDataGathering dataGathering, IDataTransformation dataTransformation, IBinanceRepository binanceRepository)
        {
            _dataGathering = dataGathering ?? throw new ArgumentNullException(nameof(dataGathering));
            _dataTransformation = dataTransformation ?? throw new ArgumentNullException(nameof(dataTransformation));
            this._binanceRepository = binanceRepository ?? throw new ArgumentNullException(nameof(binanceRepository));
        }

        public async Task InitializeData()
        {
            var assets = await _dataGathering.GetWalletAssets();
            assets = _dataTransformation.PopulateInitialOfflineData(assets);
            assets = await _dataGathering.GetAssetPricesAsync(assets);
            assets = _dataTransformation.GetAssets(assets, assets);

            _binanceRepository.ExecuteSqlScript("TRUNCATE TABLE public.\"Assets\"");

            //Write to DB
            await _binanceRepository.InsertAssets(assets);
        }

        public List<Asset> GetStoredPrices()
        {
            var assets = _binanceRepository.GetAssets();
            return assets;
        }


        public async Task UpdateCurrentPrices()
        {
            var assets = _binanceRepository.GetAssets();
            assets = _dataTransformation.PopulateInitialOfflineData(assets);
            assets = await _dataGathering.GetAssetPricesAsync(assets);

            await _binanceRepository.PopulateCurrentPrices(assets);
        }

        public async Task UpdateWalletHoldings()
        {
            var assets = await _dataGathering.GetWalletAssets();

            List<Asset> assetsList = new List<Asset>();
            foreach (var assetPrice in assets)
            {
                assetsList.Add(new Asset { Name = assetPrice.Name, WalletHolding = assetPrice.WalletHolding });
            }
            await _binanceRepository.PopulateWalletHoldings(assetsList);
        }

        public (decimal btcEur, decimal btcUsdt) GetBtcFiatValues()
        {
            var assets = _binanceRepository.GetAssets();
            var eur = assets.FirstOrDefault(x => x.Name == "BTC" && x.Market == "EUR")?.CurrentValue ?? 0m;
            var usdt = assets.FirstOrDefault(x => x.Name == "BTC" && x.Market == "USDT")?.CurrentValue ?? 0m;
            return (eur, usdt);
        }

    }
}
