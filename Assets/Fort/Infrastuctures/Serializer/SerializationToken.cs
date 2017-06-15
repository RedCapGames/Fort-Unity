using System;
using System.Runtime.Serialization;

namespace Fort.Serializer
{
    [Serializable]
    public class SerializationToken
    {
        public SerializationToken()
        {
            TokenType = SerializationTokenType.Custom;
        }
        #region Fields

        [OptionalField]
        private object _data;


        #endregion

        #region Properties

        public SerializationTokenType TokenType { get; set; }

        private string _type;

        public Type Type
        {
            get
            {
                try
                {
                    return Type.GetType(_type);
                }
                catch (Exception)
                {
                    return null;
                }
            }
            set { _type = value == null ? null : value.AssemblyQualifiedName; }
        }

        public SerializationToken[] SerializationTokens { get; set; }

        public object Data
        {
            get { return _data; }
            set { _data = value; }
        }

        #endregion
    }
}