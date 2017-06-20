using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Fort.Serializer
{
    internal class ListSerializerToken : ISerializerToken
    {
/*        private static object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }*/
        private static IEnumerable<Type> GetParentTypes(Type type)
        {
            // is there any base type?
            if ((type == null) || (type.BaseType == null))
            {
                yield break;
            }

            // return all implemented or inherited interfaces
            foreach (Type i in type.GetInterfaces())
            {
                yield return i;
            }

            // return all inherited types
            Type currentBaseType = type.BaseType;
            while (currentBaseType != null)
            {
                yield return currentBaseType;
                currentBaseType = currentBaseType.BaseType;
            }
        }
        #region Implementation of ISerializerToken

        public void Serialize(object graph, IGenericSerialierToken serializerToken, SerializationToken resultSerializationToken)
        {
            IEnumerable enumerable = (IEnumerable)graph;
            List<SerializationToken> serializationTokens = new List<SerializationToken>();
            foreach (object item in enumerable)
            {
                serializationTokens.Add(serializerToken.Serialize(item));
            }
            resultSerializationToken.Type = graph.GetType();
            resultSerializationToken.TokenType = SerializationTokenType.List;
            resultSerializationToken.SerializationTokens = serializationTokens.ToArray();
        }

        public void Deserialize(SerializationToken serializationToken, IGenericSerialierToken serializerToken, DeserializeResult deserializeResult)
        {
            Type parentIListType =
                GetParentTypes(serializationToken.Type)
                    .First(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof (IList<>));
            MethodInfo insertMethod = parentIListType.GetMethod("Insert");
            object list = Activator.CreateInstance(serializationToken.Type);
            serializerToken.RegisterReverseToken(serializationToken,list);
            for (int i = 0; i < serializationToken.SerializationTokens.Length; i++)
            {
                DeserializeResult dResult = serializerToken.Deserialize(serializationToken.SerializationTokens[i]);
                if (dResult.Use)
                    insertMethod.Invoke(list, new[] {i, dResult.Result});
                else
                {
                    Type elementType = parentIListType.GetGenericArguments().First();
                    insertMethod.Invoke(list, new[] { i, elementType.GetDefault() });
                }
            }
            deserializeResult.Result = list;            
        }

        public bool IsSupportType(Type type)
        {
            Type parentIListType =
                GetParentTypes(type).ToArray()
                    .FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof (IList<>));
            return parentIListType != null;
        }

        #endregion
    }
}