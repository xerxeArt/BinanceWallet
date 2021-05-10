using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Data.HttpTools
{
    public class HttpUtilities : IHttpUtilities
    {
        private readonly char[] TrimChars = { '?', '&', '#', '/', '\\', ' ', '\t', '\r', '\n' };

        /// <summary>
        /// Whether or not the current call is mocked
        /// </summary>
        public bool MockedCall { get; set; } = false;

        #region Uri Utilities
        /// <summary>
        /// Add route to an already built URI
        /// </summary>
        /// <param name="uri">The initial URI</param>
        /// <param name="additionalRoute">The route to add</param>
        /// <returns>The combined URI and new route</returns>
        public string AddAdditionalRouteToUri(string uri, string additionalRoute)
        {
            string cleanUri = uri?.Trim(TrimChars);
            string cleanAdditionalRoute = additionalRoute?.Trim(TrimChars);
            if (String.IsNullOrEmpty(cleanUri)) return String.Empty;
            if (String.IsNullOrEmpty(cleanAdditionalRoute)) return uri;
            if (cleanUri.Contains('?'))
            {
                string[] splitUri = cleanUri.Split(new char[] { '?' });
                splitUri[0] = $"{splitUri[0].Trim(TrimChars)}/{cleanAdditionalRoute}";
                cleanUri = string.Join('?', splitUri);
            }
            else if (cleanUri.Contains('#'))
            {
                string[] splitUri = cleanUri.Split(new char[] { '#' });
                cleanUri = $"{splitUri[0].Trim(TrimChars)}/{cleanAdditionalRoute}#{splitUri[1].Trim(TrimChars)}";
            }
            else
            {
                cleanUri = $"{cleanUri}/{cleanAdditionalRoute}";
            }
            return cleanUri;
        }

        /// <summary>
        /// Adds a querystring to an existing URI
        /// </summary>
        /// <param name="uri">The initial URI</param>
        /// <param name="queryString">The additional querystring</param>
        /// <returns>The combined URI and new querystring</returns>
        public string AddQueryStringToUri(string uri, string queryString)
        {
            string cleanUri = uri?.Trim(TrimChars);
            string cleanQueryString = queryString?.Trim(TrimChars);
            if (String.IsNullOrEmpty(cleanUri)) return String.Empty;
            if (String.IsNullOrEmpty(cleanQueryString)) return uri;

            char joinChar = cleanUri.Contains('?') ? '&' : '?';

            string result;
            if (cleanUri.Contains('#'))
            {
                int hashPosition = cleanUri.IndexOf('#');
                result = $"{cleanUri.Substring(0, hashPosition).Trim(TrimChars)}{joinChar}{cleanQueryString}#{cleanUri.Substring(hashPosition + 1).Trim(TrimChars)}";
            }
            else result = $"{cleanUri}{joinChar}{cleanQueryString}";
            return result;
        }

        /// <summary>
        /// Get a querystring-formatted string out of an key-value pairs string
        /// </summary>
        /// <param name="jsonObj">the key-value pairs string</param>
        /// <returns>The key-value pairs formatted as a querystring</returns>
        public string GetQueryStringFromKVPObject(string jsonObj)
        {
            if (String.IsNullOrWhiteSpace(jsonObj)) return String.Empty;
            var kvPairs = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonObj, new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });

            return GetQueryStringFromDictionary(kvPairs);
        }

        public string GetQueryStringFromDictionary(Dictionary<string, string> dict)
        {
            if (dict == null || dict.Count == 0) return String.Empty;

            List<string> qParams = new List<string>();

            foreach (var kvPair in dict)
            {
                if (kvPair.Value == null) continue;
                qParams.Add($"{kvPair.Key}={System.Web.HttpUtility.UrlEncode(kvPair.Value.ToString())}");
            }

            string queryString = String.Join("&", qParams);

            return queryString;
        }
        #endregion

        /// <summary>
        /// Gets a full HttpRequestMessage with the provided parameters
        /// </summary>
        /// <param name="httpMethod">The HttpMethod</param>
        /// <param name="requestUri">The request URI</param>
        /// <param name="requestBody">The request body</param>
        /// <param name="headers">The request headers</param>
        /// <returns>The constructed HttpRequestMessage</returns>
        public HttpRequestMessage GetHttpRequest(HttpMethod httpMethod, Uri requestUri, string requestBody, Dictionary<string, string> headers)
        {
            var request = new HttpRequestMessage()
            {
                Method = httpMethod,
                RequestUri = requestUri
            };

            request.Content = new StringContent(requestBody ?? String.Empty);
            request.Headers.Clear();
            request.Content.Headers.Clear();

            foreach (var header in headers)
            {
                if (header.Key.Equals("content-length", StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }
                else if (header.Key.Equals("content-type", StringComparison.InvariantCultureIgnoreCase))
                {
                    request.Content.Headers.Add(header.Key, header.Value);
                    continue;
                }
                request.Headers.Add(header.Key, header.Value);
            }

            return request;
        }

        /// <summary>
        /// Get HttpHeaders as a dictionary from an IHeaderDictionary
        /// </summary>
        /// <param name="headers">The IHeaderDictionary</param>
        /// <returns>The dictionary of key-values containing the HTTP headers</returns>
        public Dictionary<string, string> GetHttpHeaders(IHeaderDictionary headers)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            string[] skipHeaders = { "host", "content-length", "accept", "user-agent" };
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    if (skipHeaders.Contains(header.Key.ToLower())) continue;
                    keyValuePairs.Add(header.Key, header.Value);
                }
            }
            return keyValuePairs;
        }

        /// <summary>
        /// Merge two dictionaries into a single one
        /// </summary>
        /// <typeparam name="T1">The key type of the dictionaries</typeparam>
        /// <typeparam name="T2">The value type of the dictionaries</typeparam>
        /// <param name="destination">The destination dictionary</param>
        /// <param name="addition">The additional dictionary</param>
        /// <param name="overwriteExisting">If true, then common keys will be overwritten; if false, then common keys will be kept from destination</param>
        /// <returns>A new dictionary containing the merge of the two provided dictionaries</returns>
        public Dictionary<T1, T2> MergeDictionaries<T1, T2>(Dictionary<T1, T2> destination, Dictionary<T1, T2> addition, bool overwriteExisting)
        {
            Dictionary<T1, T2> result = CloneDictionary(destination);
            if (addition == null) return result;
            foreach (var item in addition)
            {
                if (overwriteExisting || !result.ContainsKey(item.Key))
                {
                    result[item.Key] = item.Value;
                }
            }

            return result;
        }

        private Dictionary<T1, T2> CloneDictionary<T1, T2>(Dictionary<T1, T2> original)
        {
            if (original == null) return new Dictionary<T1, T2>();
            Dictionary<T1, T2> ret = new Dictionary<T1, T2>(original.Count);
            foreach (KeyValuePair<T1, T2> entry in original)
            {
                ret.Add(entry.Key, entry.Value);
            }
            return ret;
        }
    }
}
