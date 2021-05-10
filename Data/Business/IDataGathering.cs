using Data.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Business
{
    public interface IDataGathering
    {
        Task<List<Asset>> GetWalletAssets();
        Task<List<Asset>> GetAssetPrices(List<string> assets);
        Task<List<Asset>> GetAssetPricesAsync(List<Asset> assets);
    }
}