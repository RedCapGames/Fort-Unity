using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Fort.Info.Market.Iap
{
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
}