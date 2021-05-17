using Data.Configuration;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

namespace BinanceWallet.Middleware
{
    public class UseConfigFromQuerystringMiddleware
    {
        private readonly RequestDelegate _next;
        public UseConfigFromQuerystringMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(HttpContext context, ApplicationConfig applicationConfig)
        {
            var queryStringCollection = HttpUtility.ParseQueryString(context.Request.QueryString.Value);

            foreach (var property in typeof(ApplicationConfig).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var keyInQS = queryStringCollection.AllKeys.FirstOrDefault(x => x == property.Name);
                if (keyInQS != null)
                {
                    property.SetValue(applicationConfig, queryStringCollection[keyInQS]);
                }
            }

            await _next(context);
        }
    }
}
