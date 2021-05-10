using Data.Data;
using System.Collections.Generic;

namespace Data.Business
{
    public interface IDataTransformation
    {
        List<Asset> GetAssets(List<Asset> currentValues, List<Asset> walletValues);
        List<Asset> PopulateInitialOfflineData(List<Asset> assets);
    }
}