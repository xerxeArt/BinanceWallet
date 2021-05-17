using Microsoft.AspNetCore.Html;
using System;

namespace BinanceWallet.Helpers
{
    public static class HelperMethods
    {
        public static string FormatCurrency(decimal val, int decimals = 8)
        {
            if (val == 0m) return "0";
            if (decimals == 8 && val >= 1) decimals = 2;
            return string.Format(new System.Globalization.NumberFormatInfo() { NumberDecimalDigits = decimals },
                "{0:F}", val);
        }

        public static string FormatPercentage(decimal val, int decimals = 2)
        {
            return string.Format(new System.Globalization.NumberFormatInfo() { NumberDecimalDigits = decimals },
                "{0:F}%", val);
        }

        public static HtmlString GetTradeLink(string asset, string market, string linkText)
        {
            string url = "https://www.binance.com/en/trade/{0}_{1}?layout=pro&type=spot";
            linkText = String.IsNullOrEmpty(linkText) ? $"{asset}_{market}" : linkText;
            var result = new HtmlString($"<a href=\"{String.Format(url, asset, market)}\" target=\"_blank\">{linkText}</a>");
            return result;
        }

        public static HtmlString ShowMarketPrice(string assetName, string market, bool isAssetOnMarket)
        {
            //TODO: implement this to have a link to the binance trading page
            return null;
        }
    }
}
