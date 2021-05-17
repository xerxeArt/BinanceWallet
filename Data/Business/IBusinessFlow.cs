using Data.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Business
{
    public interface IBusinessFlow
    {
        Task UpdateCurrentPrices();
        List<Asset> GetStoredPrices();
        Task InitializeData();
        Task UpdateWalletHoldings();
        (decimal btcEur, decimal btcBusd) GetBtcFiatValues();
    }
}