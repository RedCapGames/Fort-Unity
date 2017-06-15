using System.IO;

namespace Fort.Serializer
{
    public static class Helper
    {
        public static object Clone(object obj)
        {
            Serializer serializer = new Serializer();
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Serialize(stream, obj);
                stream.Position = 0;
                return serializer.Deserialize(stream);
            }
        }
    }
}
