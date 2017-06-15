using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Fort.Info
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
    public class ValueIapPackage : IapPackageInfo<ValueIapData>
    {

    }
    public class ValueIapData
    {
        [JsonConverter(typeof(BalanceJsonConverter))]
        public Balance Values { get; set; }
    }
    public class DiscountIapPackage : IapPackageInfo<DiscountIapData>
    {
    }

    public class DiscountIapData
    {
        [JsonConverter(typeof(DiscountSkuJsonConverter))]
        public IapPackageInfo IapPackageInfo { get; set; }
        public int Discount { get; set; }

    }
    public class PurchasableItemsIapPackageInfo : IapPackageInfo<PurchasableItemsIapData>
    {
        
    }

    public class PurchasableItemsIapData
    {
        [JsonConverter(typeof(PurchaseDatasItemsJsonConverter))]
        public PurchaseData[] PurchaseDatas { get; set; }
    }

    public abstract class PurchaseData
    {
        
    }
    public class PurchaseLevelBaseData: PurchaseData
    {
        public LevelBasePurchasableItemInfo PurchasableItemInfo { get; set; }
        public int Level { get; set; }
    }

    public class PurchaseNoneLevelBaseData : PurchaseData
    {
        public NoneLevelBasePurchasableItemInfo PurchasableItemInfo { get; set; }
    }
    public class SkinnerBoxIapPackageInfo : IapPackageInfo<SkinnerBoxIapData>
    {
        
    }

    public class SkinnerBoxIapData
    {
        public SkinnerBoxIap[] SkinnerBoxes { get; set; }
    }
    public class SkinnerBoxIap
    {
        public int Count { get; set; }
        [JsonConverter(typeof(SkinnerBoxJsonConverter))]
        public SkinnerBoxInfo SkinnerBoxInfo { get; set; }
    }

    public class RemoveAdsIapPackageInfo : IapPackageInfo
    {

    }
    public class MarketInfoesJsonConverter : JsonConverter
    {
        #region Overrides of JsonConverter

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JToken jToken = JToken.FromObject(value == null ? null : ((MarketInfo[])value).Select(info => info.MarketName).ToArray());
            jToken.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken jToken = JToken.ReadFrom(reader);
            if (jToken.Type == JTokenType.Null || jToken.Type == JTokenType.None)
                return new MarketInfo[0];
            return jToken.ToObject<string[]>().Select(s => InfoResolver.FortInfo.MarketInfos.FirstOrDefault(info => info.MarketName ==s)).Where(info => info != null).ToArray();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(MarketInfo[]);
        }

        #endregion
    }
    public class DiscountSkuJsonConverter : JsonConverter
    {
        #region Overrides of JsonConverter

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JToken jToken = JToken.FromObject(value == null ? null : ((IapPackageInfo)value).Sku);
            jToken.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken jToken = JToken.ReadFrom(reader);
            if (jToken.Type == JTokenType.Null || jToken.Type == JTokenType.None)
                return null;
            return jToken.ToObject<string>();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IapPackageInfo).IsAssignableFrom(objectType);
        }

        #endregion
    }
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
    public class SkinnerBoxJsonConverter : JsonConverter
    {
        #region Overrides of JsonConverter

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JToken jToken = JToken.FromObject(value == null ? null : ((SkinnerBoxInfo)value).Id);
            jToken.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken jToken = JToken.ReadFrom(reader);
            if (jToken.Type == JTokenType.Null || jToken.Type == JTokenType.None)
                return null;
            return InfoResolver.FortInfo.SkinnerBox.BoxInfos.FirstOrDefault(info => info.Id == jToken.ToObject<string>());            
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(SkinnerBoxInfo).IsAssignableFrom(objectType);
        }

        #endregion
    }
}
