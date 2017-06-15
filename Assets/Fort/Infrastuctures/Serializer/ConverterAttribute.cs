using System;

namespace Fort.Serializer
{
    public class ConverterAttribute : Attribute
    {
        public Type SerializerTokenType { get; private set; }

        public ConverterAttribute(Type serializerTokenType)
        {
            SerializerTokenType = serializerTokenType;
        }
    }
}