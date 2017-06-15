using System;
using System.Collections;
using System.Collections.Generic;

namespace Fort.Serializer
{
    public class ArraySerializerToken:ISerializerToken
    {
        #region Implementation of ISerializerToken

        public void Serialize(object graph, IGenericSerialierToken serializerToken, SerializationToken resultSerializationToken)
        {
            IEnumerable enumerable = (IEnumerable) graph;
            List<SerializationToken> serializationTokens = new List<SerializationToken>();
            foreach (object item in enumerable)
            {
                serializationTokens.Add(serializerToken.Serialize(item));
            }
            resultSerializationToken.Type = graph.GetType();
            resultSerializationToken.TokenType = SerializationTokenType.Array;
            resultSerializationToken.SerializationTokens = serializationTokens.ToArray();
        }

        public void Deserialize(SerializationToken serializationToken, IGenericSerialierToken serializerToken, DeserializeResult deserializeResult)
        {
            Array array = Array.CreateInstance(serializationToken.Type.GetElementType(),
                serializationToken.SerializationTokens.Length);
            serializerToken.RegisterReverseToken(serializationToken,array);
            for (int i = 0; i < serializationToken.SerializationTokens.Length; i++)
            {
                DeserializeResult dResult = serializerToken.Deserialize(serializationToken.SerializationTokens[i]);
                if(dResult.Use)
                    try
                    {
                        array.SetValue(dResult.Result, i);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
            }
            deserializeResult.Result = array;
        }

        public bool IsSupportType(Type type)
        {
            return type.IsArray;
        }

        #endregion
    }
}