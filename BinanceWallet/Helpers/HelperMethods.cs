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
    }
}
