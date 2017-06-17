using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Fort.Info.Market.Iap;
using Debug = UnityEngine.Debug;

namespace Fort.Serializer
{
    public class ConcreteSerializerToken:ISerializerToken
    {
        #region Implementation of ISerilazerToken

        public void Serialize(object graph, IGenericSerialierToken serializerToken, SerializationToken resultSerializationToken)
        {
            if (graph is ValueIapPackage)
            {
                Debug.Log("Test");
            }
            PropertyInfo[] info = graph.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            List<SerializationToken> serializationTokens = new List<SerializationToken>();
            
            foreach (
                PropertyInfo source in
                    info.Where(
                        propertyInfo =>
                            propertyInfo.CanRead && propertyInfo.CanWrite &&
                            propertyInfo.GetIndexParameters().Length == 0))
            {
                serializationTokens.Add(serializerToken.Serialize(source.Name));
                serializationTokens.Add(serializerToken.SerializeProperty(source.GetValue(graph, new object[0]), source));
            }
            resultSerializationToken.Type = graph.GetType();
            resultSerializationToken.TokenType = SerializationTokenType.Concrete;
            resultSerializationToken.SerializationTokens = serializationTokens.ToArray();
        }

        public void Deserialize(SerializationToken serializationToken, IGenericSerialierToken serializerToken, DeserializeResult deserializeResult)
        {
            object graph;

            try
            {
                graph = Activator.CreateInstance(serializationToken.Type);
            }
            catch (Exception)
            {
                deserializeResult.Result = null;
                deserializeResult.Use = false;
                return;
            }
            if (graph is ValueIapPackage)
            {
                Debug.Log("Test");
            }
            serializerToken.RegisterReverseToken(serializationToken,graph);
            for (int i = 0; i < serializationToken.SerializationTokens.Length / 2; i++)
            {
                string propertyName = (string) serializerToken.Deserialize(serializationToken.SerializationTokens[2 * i]).Result;
                
                PropertyInfo propertyInfo = serializationToken.Type.GetProperty(propertyName,
                    BindingFlags.Public | BindingFlags.Instance);
                if ( propertyInfo != null && propertyInfo.CanRead && propertyInfo.CanWrite &&
                    propertyInfo.GetIndexParameters().Length == 0)
                {
                    DeserializeResult dResult = serializerToken.DeserializeProperty(serializationToken.SerializationTokens[2 * i + 1],propertyInfo);
                    if (dResult.Use)
                        try
                        {
                            propertyInfo.SetValue(graph, dResult.Result, new object[0]);
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                }
            }
            deserializeResult.Result = graph;
        }

        public bool IsSupportType(Type type)
        {
            return true;
        }

        #endregion
    }
}