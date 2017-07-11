using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fort.Serializer
{
    internal class PrimitiveSerializerToken : ISerializerToken
    {
        private readonly Dictionary<Type, Type> _numericTypes = new Dictionary<Type, Type>
        {
            {typeof (byte), typeof (byte)},
            {typeof (sbyte), typeof (sbyte)},
            {typeof (short), typeof (short)},
            {typeof (ushort), typeof (ushort)},
            {typeof (int), typeof (int)},
            {typeof (uint), typeof (uint)},
            {typeof (long), typeof (long)},
            {typeof (ulong), typeof (ulong)},
            {typeof (float), typeof (float)},
            {typeof (double), typeof (double)},
            {typeof (decimal), typeof (decimal)}
        };
        #region Implementation of ISerilazerToken

        public void Serialize(object graph, IGenericSerialierToken serializerToken, SerializationToken resultSerializationToken)
        {
            resultSerializationToken.TokenType = SerializationTokenType.Data;
            resultSerializationToken.Type = graph.GetType();
            resultSerializationToken.Data = graph;
        }

        public void Deserialize(SerializationToken serializationToken, IGenericSerialierToken serializerToken, DeserializeResult deserializeResult)
        {
            deserializeResult.Result = serializationToken.Data;
        }

        public bool IsSupportType(Type type)
        {
            Type underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
                type = underlyingType;
            if (type == typeof(TimeSpan))
            {
                return true;
            }
            if (type == typeof(DateTime))
            {
                return true;
            }
            if (type == typeof(string))
            {
                return true;
            }
            if (_numericTypes.ContainsKey(type))
            {
                return true;
            }
            if (type == typeof(bool))
            {
                return true;
            }
            if (type.IsEnum)
            {
                return true;
            }
            return false;
        }

        #endregion
    }
}
