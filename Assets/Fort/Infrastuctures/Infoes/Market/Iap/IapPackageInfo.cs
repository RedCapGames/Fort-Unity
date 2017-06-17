using Newtonsoft.Json;

namespace Fort.Info.Market.Iap
{
    public abstract class IapPackageInfo
    {
        public string Sku { get; set; }
        public string DisplayName { get; set; }
        public int Price { get; set; }
        [JsonConverter(typeof(MarketInfoesJsonConverter))]
        public MarketInfo[] Markets { get; set; }
    }
    public abstract class IapPackageInfo<T> : IapPackageInfo
    {
        public T PackageData { get; set; }
    }
}
