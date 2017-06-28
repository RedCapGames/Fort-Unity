using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Fort.Stream
{
    public static class StreamExtentions
    {
        public static int Read(this System.IO.Stream stream, byte[] buffer, int count)
        {
            return stream.Read(buffer, 0, count);
        }
        public static int SafeRead(this System.IO.Stream stream, byte[] buffer, int offset, int count)
        {
            int result = 0;
            while (result < count)
            {
                int readSize = stream.Read(buffer, offset + result, count - result);
                if (readSize == 0)
                    return result;
                result += readSize;

            }
            return result;
        }
        public static int SafeRead(this System.IO.Stream stream, byte[] buffer, int count)
        {
            return stream.SafeRead(buffer, 0, count);
        }
        public static System.IO.Stream Append(this System.IO.Stream stream, byte[] buffer, int offset, int count)
        {
            return new MergedStream(new MemoryStream(buffer, offset, count), stream);
        }
        public static System.IO.Stream Append(this System.IO.Stream stream, System.IO.Stream header)
        {
            return new MergedStream(header, stream);
        }
        public static System.IO.Stream Prepend(this System.IO.Stream stream, byte[] buffer, int offset, int count)
        {
            return new MergedStream(stream, new MemoryStream(buffer, offset, count));
        }
        public static System.IO.Stream Prepend(this System.IO.Stream stream, System.IO.Stream tail)
        {
            return new MergedStream(stream, tail);
        }
        public static System.IO.Stream SubStream(this System.IO.Stream stream, long offset, long count, bool seekToOffset = true)
        {
            return new SubStream(stream, offset, count, seekToOffset);
        }
        public static System.IO.Stream SubStream(this System.IO.Stream stream, long offset, bool seekToOffset = true)
        {
            return new SubStream(stream, offset, seekToOffset);
        }
        public static System.IO.Stream ToStream(this IEnumerable<System.IO.Stream> streams)
        {
            System.IO.Stream[] strms = streams.ToArray();
            if (strms.Length == 1)
                return strms[0];
            return new MergedStream(strms);
        }
        public static MemoryStream ToMemoryStream(this byte[] buffer)
        {
            return new MemoryStream(buffer);
        }
        public static byte[] ToArray(this System.IO.Stream stream)
        {
            byte[] result = new byte[stream.Length];
            stream.SafeRead(result, 0, result.Length);
            return result;
        }
        public static string ReadString(this System.IO.Stream stream)
        {
            return stream.ReadString(Encoding.ASCII);
        }
        public static string ReadString(this System.IO.Stream stream, Encoding encoding)
        {
            return encoding.GetString(stream.ToArray());
        }
        public static void WriteString(this System.IO.Stream stream, string value)
        {
            WriteString(stream, value, Encoding.ASCII);
        }
        public static void WriteString(this System.IO.Stream stream, string value, Encoding encoding)
        {
            byte[] bytes = encoding.GetBytes(value);
            stream.Write(bytes, 0, bytes.Length);
        }
        public static void BinarySerialize(this System.IO.Stream stream, object obj)
        {
            BinaryFormatter formater = new BinaryFormatter();
            formater.Serialize(stream, obj);
        }
        public static object BinaryDeserialize(this System.IO.Stream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(stream);
        }
        public static T BinaryDeserialize<T>(this System.IO.Stream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            return (T)formatter.Deserialize(stream);
        }
        public static System.IO.Stream Inspect(this System.IO.Stream stream, StreamInspectionActions inspectionActions)
        {
            return new StreamInspecter(stream,inspectionActions);
        }
        public static void CopyTo(this System.IO.Stream stream, System.IO.Stream destination)
        {
            int bytesRead;
            byte[] buffer = new byte[2048];
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                destination.Write(buffer, 0, bytesRead);
            }
        }

    }
}
