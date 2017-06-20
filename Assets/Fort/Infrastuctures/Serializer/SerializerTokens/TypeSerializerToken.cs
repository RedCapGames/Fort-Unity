using System;

namespace Fort.Serializer
{
    internal class TypeSerializerToken:ISerializerToken
    {
        #region Implementation of ISerializerToken

        public void  Serialize(object graph, IGenericSerialierToken serializerToken, SerializationToken resultSerializationToken)
        {
            resultSerializationToken.Data = ((Type) graph).AssemblyQualifiedName;
            resultSerializationToken.TokenType = SerializationTokenType.Type;
            resultSerializationToken.Type = typeof (Type);
        }

        public void Deserialize(SerializationToken serializationToken, IGenericSerialierToken serializerToken, DeserializeResult deserializeResult)
        {
            Type result = null;
            try
            {
                result = Type.GetType((string) serializationToken.Data);
            }
            catch (Exception)
            {
                // ignored
            }
            if (result == null)
            {
                deserializeResult.Use = false;
            }
            deserializeResult.Result = result;
        }

        public bool IsSupportType(Type type)
        {
            bool result = type == typeof (Type) ||type.FullName == "System.MonoType";
            return result;
        }

        #endregion
    }
}