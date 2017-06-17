using Newtonsoft.Json;

namespace Fort.Info.Market.Iap
{
    public class PurchasableItemsIapData
    {
        [JsonConverter(typeof(PurchaseDatasItemsJsonConverter))]
        public PurchaseData[] PurchaseDatas { get; set; }
    }
}