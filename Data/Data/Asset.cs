namespace Data.Data
{
    public class Asset
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Market { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal CurrentFiatValue { get; set; }
        public decimal WalletHolding { get; set; }
        public decimal BuyPriceWeightedAvg { get; set; }
    }
}
