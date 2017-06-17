using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Fort.Info.Market.Iap
{
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
}