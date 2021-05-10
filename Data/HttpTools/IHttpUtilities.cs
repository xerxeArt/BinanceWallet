using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Data.HttpTools
{
    public interface IHttpUtilities
    {
        /// <summary>
        /// Add route to an already built URI
        /// </summary>
        /// <param name="uri">The initial URI</param>
        /// <param name="additionalRoute">The route to add</param>
        /// <returns>The combined URI and new route</returns>
        string AddAdditionalRouteToUri(string uri, string additionalRoute);
        /// <summary>
        /// Adds a querystring to an existing URI
        /// </summary>
        /// <param name="uri">The initial URI</param>
        /// <param name="queryString">The additional querystring</param>
        /// <returns>The combined URI and new querystring</returns>
        string AddQueryStringToUri(string uri, string queryString);
        /// <summary>
        /// Get a querystring-formatted string out of an key-value pairs string
        /// </summary>
        /// <param name="jsonObj">the key-value pairs string</param>
        /// <returns>The key-value pairs formatted as a querystring</returns>
        string GetQueryStringFromKVPObject(string jsonObj);
        string GetQueryStringFromDictionary(Dictionary<string, string> dict);
        /// <summary>
        /// Gets a full HttpRequestMessage with the provided parameters
        /// </summary>
        /// <param name="httpMethod">The HttpMethod</param>
        /// <param name="requestUri">The request URI</param>
        /// <param name="requestBody">The request body</param>
        /// <param name="headers">The request headers</param>
        /// <returns>The constructed HttpRequestMessage</returns>
        HttpRequestMessage GetHttpRequest(HttpMethod httpMethod, Uri requestUri, string requestBody, Dictionary<string, string> headers);
        /// <summary>
        /// Get HttpHeaders as a dictionary from an IHeaderDictionary
        /// </summary>
        /// <param name="headers">The IHeaderDictionary</param>
        /// <returns>The dictionary of key-values containing the HTTP headers</returns>
        Dictionary<string, string> GetHttpHeaders(IHeaderDictionary headers);
        /// <summary>
        /// Merge two dictionaries into a single one
        /// </summary>
        /// <typeparam name="T1">The key type of the dictionaries</typeparam>
        /// <typeparam name="T2">The value type of the dictionaries</typeparam>
        /// <param name="destination">The destination dictionary</param>
        /// <param name="addition">The additional dictionary</param>
        /// <param name="overwriteExisting">If true, then common keys will be overwritten; if false, then common keys will be kept from destination</param>
        /// <returns>A new dictionary containing the merge of the two provided dictionaries</returns>
        Dictionary<T1, T2> MergeDictionaries<T1, T2>(Dictionary<T1, T2> destination, Dictionary<T1, T2> addition, bool overwriteExisting);
    }
}
