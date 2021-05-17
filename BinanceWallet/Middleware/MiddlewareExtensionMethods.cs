using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BinanceWallet.Middleware
{
    public static class MiddlewareExtensionMethods
    {
        public static IApplicationBuilder UseConfigFromQuerystring(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UseConfigFromQuerystringMiddleware>();
        }
    }
}
