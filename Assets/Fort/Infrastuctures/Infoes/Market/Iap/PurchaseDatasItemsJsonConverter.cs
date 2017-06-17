using System;
using System.Linq;
using Fort.Info.PurchasableItem;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Fort.Info.Market.Iap
{
    public class PurchaseDatasItemsJsonConverter : JsonConverter
    {
        #region Overrides of JsonConverter

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            
            JToken jToken = JToken.FromObject(value == null ? null : ((PurchaseData[])value).Select(data =>
            {
                string result = null;
                if (data != null)
                {
                    PurchaseNoneLevelBaseData purchaseNoneLevelBaseData = data as PurchaseNoneLevelBaseData;
                    if (purchaseNoneLevelBaseData != null)
                    {
                        if (purchaseNoneLevelBaseData.PurchasableItemInfo != null)
                        {
                            result = purchaseNoneLevelBaseData.PurchasableItemInfo.Id;
                        }
                    }
                    PurchaseLevelBaseData purchaseLevelBaseData = data as PurchaseLevelBaseData;
                    if (purchaseLevelBaseData != null)
                    {
                        if (purchaseLevelBaseData.PurchasableItemInfo != null)
                        {
                            PurchasableLevelInfo[] purchasableLevelInfos = purchaseLevelBaseData.PurchasableItemInfo.GetPurchasableLevelInfos();
                            if (purchaseLevelBaseData.Level<purchasableLevelInfos.Length)
                                result = purchasableLevelInfos[purchaseLevelBaseData.Level].Id;
                        }
                    }
                }
                return result;
            }).ToArray());
            jToken.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken jToken = JToken.ReadFrom(reader);
            if (jToken.Type == JTokenType.Null || jToken.Type == JTokenType.None)
                return new PurchaseData[0];
            return jToken.ToObject<string[]>().Select(s =>
            {
                PurchaseData result;
                if (!InfoResolver.FortInfo.Purchase.PurchasableTokens.ContainsKey(s))
                    return null;
                PurchasableToken purchasableToken = InfoResolver.FortInfo.Purchase.PurchasableTokens[s];
                if (purchasableToken.NoneLevelBase)
                {
                    result = new PurchaseNoneLevelBaseData
                    {
                        PurchasableItemInfo = (NoneLevelBasePurchasableItemInfo) purchasableToken.PurchasableItemInfo
                    };
                }
                else
                {
                    result = new PurchaseLevelBaseData
                    {
                        PurchasableItemInfo = (LevelBasePurchasableItemInfo) purchasableToken.PurchasableItemInfo,
                        Level = purchasableToken.Index
                    };

                }
                return result;
            }).Where(info => info != null).ToArray();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(PurchaseData[]);
        }

        #endregion
    }
}