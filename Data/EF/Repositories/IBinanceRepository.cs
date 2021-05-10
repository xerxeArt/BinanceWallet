using Data.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.EF.Repositories
{
    public interface IBinanceRepository : IRepository
    {
        List<Tuple<string, string>> GetAssetNames();
        List<Asset> GetAssets();
        Task InsertAssets(List<Asset> assets);
        Task PopulateCurrentPrices(List<Asset> assetCurrentValues);
        Task PopulateWalletHoldings(List<Asset> assetCurrentValues);
    }
}