using Fort.Info.PurchasableItem;
using Newtonsoft.Json;

namespace Fort.Info.Market.Iap
{
    public class PurchasableItemsIapData
    {
        public PurchasableItemsIapData()
        {
            PurchaseDatas = new PurchaseData[0];
        }
        [JsonConverter(typeof(PurchaseDatasItemsJsonConverter))]
        public PurchaseData[] PurchaseDatas { get; set; }
    }
}