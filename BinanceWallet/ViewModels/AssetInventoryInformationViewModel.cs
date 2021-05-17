using Data.Data;
using System;
using System.Collections.Generic;

namespace BinanceWallet.ViewModels
{
    public class AssetInventoryInformationViewModel
    {
        public string AssetName { get; set; }
        public string Market { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal CurrentPriceConverted { get; set; }
        public decimal HoldingCount { get; set; }
        public decimal BuyPriceWeightedAvg { get; set; }
        public decimal HoldingAmount { get; set; }
        public decimal HoldingAmountConverted { get; set; }
        public decimal ProfitPercent { get; set; }
        public decimal ProfitValue { get; set; }
        public decimal ProfitValueConverted { get; set; }
        public string RgbColor { get; set; }
    }
}
