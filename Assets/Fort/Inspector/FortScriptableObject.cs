using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fort;
using Fort.Info;
using Fort.Serializer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Object = UnityEngine.Object;

namespace Fort.Inspector
{
    [IgnorePresentation]
    [Serializable]
    public class FortScriptableObject : ScriptableObject
    {
        [JsonIgnore]
        public object Data { get; set; }
        [JsonIgnore]
        public string SerializedData;
        [JsonIgnore]
        public string SerializedPresentation;

        public UnityObjectToken[] UnityObjectTokens;
        public Dictionary<string, Object> Tokens { get; private set; }

        /*    [JsonIgnore]
            public FortScriptableObject ScriptableObject { get; set; }*/

        public void Save(IInfo fortObject)
        {
            Tokens = new Dictionary<string, Object>();
            
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    Serializer.Serializer serializer = new Serializer.Serializer();
                    serializer.Serialize(stream, fortObject,new UnityObjectSerializerToken(this));
                    SerializedData = Convert.ToBase64String(stream.ToArray());
                }

            }
            catch (Exception e)
            {
                Debug.LogException(e);
                SerializedData = null;
            }

/*            SerializedData = JsonConvert.SerializeObject(fortObject,
                new JsonSerializerSettings { Converters = { new UnityObjectConverter(this) }, /*Converters = { new ArrayReferencePreservngConverter() },#1# TypeNameHandling = TypeNameHandling.All/*,PreserveReferencesHandling = PreserveReferencesHandling.All#1#});*/

            UnityObjectTokens = Tokens.Select(pair => new UnityObjectToken
            {
                Item = pair.Value,
                Token = pair.Key
            }).ToArray();
        }

        public IInfo Load(Type objectType)
        {
            if(UnityObjectTokens == null)
                UnityObjectTokens = new UnityObjectToken[0];
            Tokens = UnityObjectTokens.ToDictionary(token => token.Token, token => token.Item);
            if (string.IsNullOrEmpty(SerializedData) || SerializedData == "null")
                return (IInfo) Activator.CreateInstance(objectType);
            try
            {
                
                using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(SerializedData)))
                {
                    Serializer.Serializer serializer = new Serializer.Serializer();
                    object deserialize = serializer.Deserialize(stream, new UnityObjectSerializerToken(this));
                    if (deserialize == null)
                        return null;
                    if(!objectType.IsInstanceOfType(deserialize))
                        throw new Exception("Invalid Item on loading in FortScriptableObject");
                    return (IInfo) deserialize;
                }
                              
/*                return
                        JsonConvert.DeserializeObject(SerializedData,
                            new JsonSerializerSettings { Converters = { new UnityObjectConverter(this) }, /*Converters = { new ArrayReferencePreservngConverter() },#1# TypeNameHandling = TypeNameHandling.All/*, PreserveReferencesHandling = PreserveReferencesHandling.All#1# });*/

            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return (IInfo) Activator.CreateInstance(objectType);
            }
        }

        public void SavePresentationData(object presentationData)
        {
            SerializedPresentation = JsonConvert.SerializeObject(presentationData,
                new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
        }

        public object LoadPresentationData()
        {
            try
            {
                return
                        JsonConvert.DeserializeObject(SerializedPresentation,
                            new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
    [IgnorePresentation]
    public class FortScriptableObject<T> : FortScriptableObject where T:IInfo
    {
        
    }

    public class ArrayReferencePreservngConverter : JsonConverter
    {
        const string refProperty = "$ref";
        const string idProperty = "$id";
        const string valuesProperty = "$values";

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsArray;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;
            else if (reader.TokenType == JsonToken.StartArray)
            {
                // No $ref.  Deserialize as a List<T> to avoid infinite recursion and return as an array.
                var elementType = objectType.GetElementType();
                var listType = typeof(List<>).MakeGenericType(elementType);
                var list = (IList)serializer.Deserialize(reader, listType);
                if (list == null)
                    return null;
                var array = Array.CreateInstance(elementType, list.Count);
                list.CopyTo(array, 0);
                return array;
            }
            else
            {
                var obj = JObject.Load(reader);
                var refId = (string)obj[refProperty];
                if (refId != null)
                {
                    var reference = serializer.ReferenceResolver.ResolveReference(serializer, refId);
                    if (reference != null)
                        return reference;
                }
                var values = obj[valuesProperty];
                if (values == null || values.Type == JTokenType.Null)
                    return null;
                if (!(values is JArray))
                {
                    throw new JsonSerializationException(string.Format("{0} was not an array", values));
                }
                var count = ((JArray)values).Count;

                var elementType = objectType.GetElementType();
                var array = Array.CreateInstance(elementType, count);

                var objId = (string)obj[idProperty];
                if (objId != null)
                {
                    // Add the empty array into the reference table BEFORE poppulating it,
                    // to handle recursive references.
                    serializer.ReferenceResolver.AddReference(serializer, objId, array);
                }

                var listType = typeof(List<>).MakeGenericType(elementType);
                using (var subReader = values.CreateReader())
                {
                    var list = (IList)serializer.Deserialize(subReader, listType);
                    list.CopyTo(array, 0);
                }

                return array;
            }
        }

        public override bool CanWrite { get { return false; } }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public class UnityObjectConverter: JsonConverter
    {
        private readonly FortScriptableObject _fortScriptableObject;

        public UnityObjectConverter(FortScriptableObject fortScriptableObject)
        {
            _fortScriptableObject = fortScriptableObject;
        }

        #region Overrides of JsonConverter

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string token = Guid.NewGuid().ToString();
            _fortScriptableObject.Tokens[token] = (Object) value;
            JToken jToken = JToken.FromObject(token);
            jToken.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken jToken = JToken.ReadFrom(reader);
            if (jToken.Type == JTokenType.String)
            {
                string token = jToken.ToObject<string>();
                return _fortScriptableObject.Tokens[token];
            }
            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Object).IsAssignableFrom(objectType);
        }

        #endregion
        
    }
    class UnityObjectSerializerToken : ISerializerToken
    {
        private readonly FortScriptableObject _fortScriptableObject;

        public UnityObjectSerializerToken(FortScriptableObject fortScriptableObject)
        {
            _fortScriptableObject = fortScriptableObject;
        }

        #region Implementation of ISerializerToken

        public void Serialize(object graph, IGenericSerialierToken serializerToken, SerializationToken resultSerializationToken)
        {
            string token = Guid.NewGuid().ToString();
            _fortScriptableObject.Tokens[token] = (Object)graph;
            resultSerializationToken.Type = graph.GetType();
            resultSerializationToken.Data = serializerToken.Serialize(token);
        }

        public void Deserialize(SerializationToken serializationToken, IGenericSerialierToken serializerToken,
            DeserializeResult deserializeResult)
        {
            string token = (string)serializerToken.Deserialize((SerializationToken)serializationToken.Data).Result;
            deserializeResult.Result = _fortScriptableObject.Tokens[token];
        }

        public bool IsSupportType(Type type)
        {
            return typeof(Object).IsAssignableFrom(type);
        }

        #endregion
    }
    [Serializable]
    public class UnityObjectToken
    {
        public string Token;
        public UnityEngine.Object Item;
    }
}
