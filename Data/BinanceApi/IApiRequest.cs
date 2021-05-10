using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Data.BinanceApi
{
    public interface IApiRequest
    {
        Task<string> PerformGetRequest(string additionalUrl, Dictionary<string, string> parameters, bool needUserData);
    }
}