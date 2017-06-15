using System;
using System.Reflection;

namespace Fort.Serializer
{
    public interface ISerializerToken
    {
        void Serialize(object graph, IGenericSerialierToken serializerToken, SerializationToken resultSerializationToken);
        void Deserialize(SerializationToken serializationToken, IGenericSerialierToken serializerToken, DeserializeResult deserializeResult);
        bool IsSupportType(Type type);

    }

    public interface IGenericSerialierToken
    {
        SerializationToken Serialize(object graph);
        DeserializeResult Deserialize(SerializationToken serializationToken);        
        void RegisterReverseToken(SerializationToken serializationToken, object graph);
        SerializationToken SerializeProperty(object graph, PropertyInfo property);
        DeserializeResult DeserializeProperty(SerializationToken serializationToken,PropertyInfo property);

    }
}