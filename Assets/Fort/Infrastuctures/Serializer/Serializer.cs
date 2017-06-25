using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace Fort.Serializer
{
    public class Serializer
    {
        #region Fields

        #endregion

        #region  Public Methods

        public void Serialize(System.IO.Stream serializationStream, object graph,params ISerializerToken[] serializerTokens)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            GenericSerializerToken genericSerializer = new GenericSerializerToken(serializerTokens);
            formatter.Serialize(serializationStream, genericSerializer.Serialize(graph));
        }

        public object Deserialize(System.IO.Stream serializationStream, params ISerializerToken[] serializerTokens)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            GenericSerializerToken genericSerializer = new GenericSerializerToken(serializerTokens);
            return
                genericSerializer.Deserialize((SerializationToken) formatter.Deserialize(serializationStream)).Result;
        }

        #endregion

        #region Nested types
        private class RefrenceEqualityComperer : IEqualityComparer<object>
        {
            #region IEqualityComparer<object> Members

            bool IEqualityComparer<object>.Equals(object x, object y)
            {
                return ReferenceEquals(x, y);
            }

            int IEqualityComparer<object>.GetHashCode(object obj)
            {
                return obj.GetHashCode();
            }

            #endregion
        }
        private class ReverceRefrenceEqualityComperer : IEqualityComparer<SerializationToken>
        {
            #region IEqualityComparer<SerializationToken> Members

            bool IEqualityComparer<SerializationToken>.Equals(SerializationToken x, SerializationToken y)
            {
                return ReferenceEquals(x, y);
            }

            int IEqualityComparer<SerializationToken>.GetHashCode(SerializationToken obj)
            {
                return obj.GetHashCode();
            }

            #endregion
        }
        private class GenericSerializerToken : IGenericSerialierToken
        {
            Dictionary<object, SerializationToken> _tokens = new Dictionary<object, SerializationToken>(new RefrenceEqualityComperer());
            Dictionary<SerializationToken, object> _reverseTokens = new Dictionary<SerializationToken, object>(new ReverceRefrenceEqualityComperer());
            List<ISerializerToken> _serializerTokens;

            public GenericSerializerToken(ISerializerToken[] serializerTokens)
            {
                _serializerTokens = new List<ISerializerToken>();
                _serializerTokens.AddRange(serializerTokens);
                _serializerTokens.Add(new TypeSerializerToken());
                _serializerTokens.Add(new PrimitiveSerializerToken());
                _serializerTokens.Add(new DictionarySerializerToken());
                _serializerTokens.Add(new ArraySerializerToken());
                _serializerTokens.Add(new ListSerializerToken());
                _serializerTokens.Add(new ConcreteSerializerToken());
            }
            #region Implementation of ISerializerToken

            public SerializationToken Serialize(object graph)
            {
                return SerializeProperty(graph, null);
            }

            public DeserializeResult Deserialize(SerializationToken serializationToken)
            {
                return DeserializeProperty(serializationToken, null);
            }

            public void RegisterReverseToken(SerializationToken serializationToken, object graph)
            {
                _reverseTokens[serializationToken] = graph;
            }

            public SerializationToken SerializeProperty(object graph, PropertyInfo property)
            {
                if (graph == null)
                    return new SerializationToken { TokenType = SerializationTokenType.Null };
                if (_tokens.ContainsKey(graph))
                    return _tokens[graph];
                SerializationToken result = new SerializationToken();
                _tokens[graph] = result;
                ISerializerToken serializerToken = null;
                if (property != null)
                {
                    ConverterAttribute converterAttribute = property.GetCustomAttribute<ConverterAttribute>();
                    if (converterAttribute != null)
                        serializerToken =
                            (ISerializerToken) Activator.CreateInstance(converterAttribute.SerializerTokenType);
                }
                if(serializerToken == null)
                    serializerToken = Resolve(graph.GetType());
                serializerToken.Serialize(graph, this, result);
                _tokens[graph] = result;
                return result;
            }

            public DeserializeResult DeserializeProperty(SerializationToken serializationToken, PropertyInfo property)
            {
                if (serializationToken.Type == null)
                    return new DeserializeResult { Use = false };
                if(serializationToken.TokenType == SerializationTokenType.Null)
                    return new DeserializeResult();
                if (_reverseTokens.ContainsKey(serializationToken))
                    return new DeserializeResult { Result = _reverseTokens[serializationToken] };
                DeserializeResult result = new DeserializeResult();
                ISerializerToken serializerToken = null;
                if (property != null)
                {
                    ConverterAttribute converterAttribute = property.GetCustomAttribute<ConverterAttribute>();
                    if (converterAttribute != null)
                        serializerToken =
                            (ISerializerToken)Activator.CreateInstance(converterAttribute.SerializerTokenType);
                }
                if (serializerToken == null)
                    serializerToken = Resolve(serializationToken.Type);

                serializerToken.Deserialize(serializationToken, this, result);
                if (result.Use)
                    _reverseTokens[serializationToken] = result.Result;
                return result;
            }

            private ISerializerToken Resolve(Type type)
            {
                ConverterAttribute converterAttribute = type.GetCustomAttribute<ConverterAttribute>();
                if (converterAttribute != null)
                    return (ISerializerToken) Activator.CreateInstance(converterAttribute.SerializerTokenType);
                return _serializerTokens.First(token => token.IsSupportType(type));
            }
            public bool IsSupportType(Type type)
            {
                throw new NotSupportedException();
            }

            #endregion
        }
        #endregion
    }
}