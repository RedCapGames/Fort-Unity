using System;
using System.Collections.Generic;
using System.Linq;
using Fort.Inspector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Fort.Info
{
    [Serializable]
    [Inspector(Presentation = "Fort.CustomEditor.BalancePresentation")]
    [JsonConverter(typeof(BalanceJsonConverter))]
    public class Balance
    {
        public Balance()
        {
            Values = InfoResolver.FortInfo.ValueDefenitions.ToDictionary(s => s, s => 0);
        }
        public void SyncValues()
        {
            if(Values == null)
                Values = new Dictionary<string, int>();
            string[] valueDefenitions = InfoResolver.FortInfo.ValueDefenitions??new string[0];
            foreach (string valueDefenition in valueDefenitions)
            {
                if (!Values.ContainsKey(valueDefenition))
                    Values[valueDefenition] = 0;
            }
            foreach (KeyValuePair<string, int> pair in Values.ToArray())
            {
                if (!valueDefenitions.Contains(pair.Key))
                    Values.Remove(pair.Key);
            }
        }

        public Dictionary<string, int> Values { get; set; }

        public int this[string name]
        {
            get
            {
                if (!Values.ContainsKey(name))
                    return 0;
                return Values[name];
            }
            set { Values[name] = value; }
        }

        public void SyncValueKey(string oldKey, string newKey)
        {
            if (Values.ContainsKey(oldKey) && oldKey != newKey)
            {
                Values[newKey] = Values[oldKey];
                Values.Remove(oldKey);
            }
            else if (!Values.ContainsKey(oldKey))
            {
                Values[newKey] = 0;
            }
            {
                
            }
        }

        public static bool operator <(Balance c1, Balance c2)
        {
            return c1.Values.All(pair => c2.Values.ContainsKey(pair.Key) && pair.Value<c2.Values[pair.Key]);
        }
        public static bool operator >(Balance c1, Balance c2)
        {
            return c1.Values.All(pair => c2.Values.ContainsKey(pair.Key) && pair.Value > c2.Values[pair.Key]);
        }
        public static bool operator <=(Balance c1, Balance c2)
        {
            return c1.Values.All(pair => c2.Values.ContainsKey(pair.Key) && pair.Value <= c2.Values[pair.Key]);
        }
        public static bool operator >=(Balance c1, Balance c2)
        {
            return c1.Values.All(pair => c2.Values.ContainsKey(pair.Key) && pair.Value >= c2.Values[pair.Key]);
        }
        public static Balance operator -(Balance value)
        {
            Balance result = new Balance();
            result.Values = new Dictionary<string, int>();
            foreach (KeyValuePair<string, int> pair in value.Values)
            {
                result.Values.Add(pair.Key, -pair.Value);
            }
            return result;
        }
        public static Balance operator *(Balance value,float ratio)
        {
            Balance result = new Balance();
            result.Values = new Dictionary<string, int>();
            foreach (KeyValuePair<string, int> pair in value.Values)
            {
                result.Values.Add(pair.Key, (int) (pair.Value*ratio));
            }
            return result;
        }
        public static Balance operator /(Balance value, float ratio)
        {
            Balance result = new Balance();
            result.Values = new Dictionary<string, int>();
            foreach (KeyValuePair<string, int> pair in value.Values)
            {
                result.Values.Add(pair.Key, (int)(pair.Value / ratio));
            }
            return result;
        }
        public static Balance operator *(Balance value, int ratio)
        {
            Balance result = new Balance();
            result.Values = new Dictionary<string, int>();
            foreach (KeyValuePair<string, int> pair in value.Values)
            {
                result.Values.Add(pair.Key, pair.Value * ratio);
            }
            return result;
        }
        public static Balance operator /(Balance value, int ratio)
        {
            Balance result = new Balance();
            result.Values = new Dictionary<string, int>();
            foreach (KeyValuePair<string, int> pair in value.Values)
            {
                result.Values.Add(pair.Key, pair.Value / ratio);
            }
            return result;
        }
        public static Balance operator +(Balance value, Balance secondory)
        {
            Balance result = new Balance();
            result.Values = new Dictionary<string, int>();
            foreach (KeyValuePair<string, int> pair in value.Values)
            {
                result.Values.Add(pair.Key, pair.Value + secondory.Values[pair.Key]);
            }
            return result;
        }
        public static Balance operator -(Balance value, Balance secondory)
        {
            Balance result = new Balance();
            result.Values = new Dictionary<string, int>();
            foreach (KeyValuePair<string, int> pair in value.Values)
            {
                result.Values.Add(pair.Key, pair.Value - secondory.Values[pair.Key]);
            }
            return result;
        }
    }

    internal class BalanceJsonConverter:JsonConverter
    {
        #region Overrides of JsonConverter

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JToken jToken = JToken.FromObject(value==null?null:((Balance)value).Values);
            jToken.WriteTo(writer);            
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken jToken = JToken.ReadFrom(reader);
            if(jToken.Type == JTokenType.Null || jToken.Type== JTokenType.None)
                return new Balance();
            Balance result = new Balance();

            Dictionary<string, int> balanceValues = jToken.ToObject<Dictionary<string, int>>();
            if (balanceValues != null)
            {
                foreach (KeyValuePair<string, int> pair in result.Values.ToArray())
                {
                    if (balanceValues.ContainsKey(pair.Key))
                        result[pair.Key] = balanceValues[pair.Key];
                }
            }
            result.Values = jToken.ToObject<Dictionary<string, int>>();
            return result;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof (Balance);
        }

        #endregion
    }
}