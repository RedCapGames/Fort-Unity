using System;
using System.Linq;
using Fort.Info.SkinnerBox;
using Fort.Inspector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Fort.Info.Market.Iap
{
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
            return InfoResolver.Resolve<FortInfo>().SkinnerBox.BoxInfos.FirstOrDefault(info => info.Id == jToken.ToObject<string>());            
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(SkinnerBoxInfo).IsAssignableFrom(objectType);
        }

        #endregion
    }
}