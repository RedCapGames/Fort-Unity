using System;
using System.Collections;
using System.Collections.Generic;

namespace Fort.Serializer
{
    internal class DictionarySerializerToken:ISerializerToken
    {
        #region Implementation of ISerializerToken

        public void Serialize(object graph, IGenericSerialierToken serializerToken, SerializationToken resultSerializationToken)
        {
            IDictionary dictionary = (IDictionary)graph;
            List<SerializationToken> serializationTokens = new List<SerializationToken>();
            foreach (object key in dictionary.Keys)
            {
                serializationTokens.Add(serializerToken.Serialize(key));
                serializationTokens.Add(serializerToken.Serialize(dictionary[key]));
            }
            resultSerializationToken.Type = graph.GetType();
            resultSerializationToken.TokenType = SerializationTokenType.Dictionary;
            resultSerializationToken.SerializationTokens = serializationTokens.ToArray();
        }

        public void Deserialize(SerializationToken serializationToken, IGenericSerialierToken serializerToken, DeserializeResult deserializeResult)
        {
            IDictionary dictionary = (IDictionary)Activator.CreateInstance(serializationToken.Type);
            serializerToken.RegisterReverseToken(serializationToken,dictionary);
            for (int i = 0; i < serializationToken.SerializationTokens.Length / 2; i++)
            {
                DeserializeResult key = serializerToken.Deserialize(serializationToken.SerializationTokens[2 * i]);
                DeserializeResult value = serializerToken.Deserialize(serializationToken.SerializationTokens[2 * i + 1]);
                if (key.Use && key.Result != null && !dictionary.Contains(key.Result) && value.Use)
                    try
                    {
                        dictionary.Add(key.Result, value.Result);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
            }
            deserializeResult.Result = dictionary;
        }

        public bool IsSupportType(Type type)
        {
            return typeof (IDictionary).IsAssignableFrom(type);
        }

        #endregion
    }
}