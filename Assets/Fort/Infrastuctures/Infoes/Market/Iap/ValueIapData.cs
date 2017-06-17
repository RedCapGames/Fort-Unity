using Newtonsoft.Json;

namespace Fort.Info.Market.Iap
{
    public class ValueIapData
    {
        [JsonConverter(typeof(BalanceJsonConverter))]
        public Balance Values { get; set; }
    }
}