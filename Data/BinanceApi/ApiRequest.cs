using Data.Configuration;
using Data.Crypto;
using Data.HttpTools;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace Data.BinanceApi
{
    public class ApiRequest : IApiRequest
    {
        private readonly ILogger<ApiRequest> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly BinanceConfig _config;
        private readonly IHttpUtilities _httpUtilities;
        private readonly ISignatureGenerator _signatureGenerator;
        public ApiRequest(ILogger<ApiRequest> logger, IHttpClientFactory httpClientFactory, BinanceConfig config, IHttpUtilities httpUtilities, ISignatureGenerator signatureGenerator)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _httpUtilities = httpUtilities ?? throw new ArgumentNullException(nameof(httpUtilities));
            _signatureGenerator = signatureGenerator ?? throw new ArgumentNullException(nameof(signatureGenerator));
        }

        public async Task<string> PerformGetRequest(string additionalUrl, Dictionary<string, string> parameters, bool needUserData)
        {
            HttpResponseMessage response = null;
            var client = GetHttpClient();
            string uriAddress = _httpUtilities.AddAdditionalRouteToUri(client.BaseAddress.AbsoluteUri, additionalUrl);
            uriAddress = _httpUtilities.AddQueryStringToUri(uriAddress, _httpUtilities.GetQueryStringFromDictionary(parameters));

            var request = GenerateGetRequest(uriAddress, needUserData);
            try
            {
                response = await client.SendAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error PerformGetRequest[{uriAddress}]: {ex.Message}");
            }

            if (response == null)
            {
                return String.Empty;
            }
            return Encoding.UTF8.GetString(await response.Content.ReadAsByteArrayAsync());
        }

        private HttpRequestMessage GenerateGetRequest(string url, bool needUserData)
        {
            var headers = new Dictionary<string, string>
            {
                { Constants.BINANCE_HEADER_APIKEY, _config.ApiKey }
            };
            string queryString = null;

            if (needUserData)
            {
                //Add timestamp and signature
                queryString = _httpUtilities.GetQueryStringFromDictionary(new Dictionary<string, string> {
                    { Constants.BINANCE_QUERY_RECVWINDOWS, 60000.ToString() },
                    { Constants.BINANCE_QUERY_TIMESTAMP, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString() }
                 });
                queryString += "&" + _httpUtilities.GetQueryStringFromDictionary(new Dictionary<string, string> {
                    { Constants.BINANCE_QUERY_SIGNATURE, _signatureGenerator.GenerateHMACSHA256(queryString) }
                 });
            }

            url = _httpUtilities.AddQueryStringToUri(url, queryString);

            Uri uri = new Uri(url);
            return _httpUtilities.GetHttpRequest(HttpMethod.Get, uri, null, headers);
        }

        private HttpClient GetHttpClient()
        {
            try
            {
                return _httpClientFactory.CreateClient(Constants.BINANCE_HTTP_CLIENT);
            }
            catch (Exception ex)
            {
                //_logger.LogException(ex, "Error retrieving HttpClient from factory");
                _logger.LogError($"Error retrieving HttpClient from factory: {ex.Message}\r\n{ex.StackTrace}");
                ExceptionDispatchInfo.Capture(ex).Throw();
            }
            return null;
        }
    }
}
