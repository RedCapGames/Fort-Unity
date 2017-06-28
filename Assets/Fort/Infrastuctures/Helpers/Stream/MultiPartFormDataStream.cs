using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Fort.Stream
{
    public class MultiPartFormDataStream:System.IO.Stream
    {
        private readonly System.IO.Stream _baseStream;
        private MemoryStream _memoryStream;

        public MultiPartFormDataStream(string boundary,params MultiPartParameter[] parts)
        {
            _memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(string.Format("--{0}--", boundary)));
            _baseStream = parts.Concat(new System.IO.Stream[] {_memoryStream}).ToStream();
        }
        #region Overrides of Stream

        public override void Flush()
        {
            _baseStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _baseStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override long Length
        {
            get { return _baseStream.Length; }
        }

        public override long Position
        {
            get { return _baseStream.Position; }
            set { _baseStream.Position = value; }
        }

        #endregion

        #region Overrides of Stream

        public override void Close()
        {
            _memoryStream.Close();
            base.Close();
        }

        #endregion
    }
    public abstract class MultiPartParameter:System.IO.Stream
    {
        #region Overrides of Stream

        public override void Flush()
        {
            
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        #endregion
    }
    public class StringMultiPartParameter: MultiPartParameter
    {
        private readonly System.IO.Stream _baseStream;
        public StringMultiPartParameter(string boundary,string parameter,string parameterValue)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(string.Format("--{0}", boundary));
            builder.AppendLine(string.Format("Content-Disposition: form-data; name=\"{0}\"", parameter));
            builder.AppendLine();
            builder.AppendLine(parameterValue);
            _baseStream = new MemoryStream(Encoding.UTF8.GetBytes(builder.ToString()));
        }
        #region Overrides of Stream

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _baseStream.Read(buffer, offset, count);
        }

        public override long Length
        {
            get { return _baseStream.Length; }
        }

        public override long Position
        {
            get { return _baseStream.Position; }
            set { _baseStream.Position = value; }
        }

        #endregion

        #region Overrides of Stream

        public override void Close()
        {
            _baseStream.Close();
            base.Close();
        }

        #endregion
    }
    public class StreamMultiPartParameter: MultiPartParameter
    {
        private readonly System.IO.Stream _baseStream;
        private readonly MemoryStream _memoryStream;

        public StreamMultiPartParameter(string boundary, string parameter,string fileName,System.IO.Stream baseStream)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(string.Format("--{0}", boundary));
            builder.AppendLine(string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"", parameter,fileName));
            builder.AppendLine("Content-Type: application/octet-stream");
            builder.AppendLine();
            _memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(builder.ToString()));
            _baseStream = baseStream.Append(_memoryStream);
            _baseStream = _baseStream.Prepend(new MemoryStream(Encoding.ASCII.GetBytes("\r\n")));
        }
        #region Overrides of Stream

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _baseStream.Read(buffer, offset, count);
        }

        public override long Length
        {
            get { return _baseStream.Length; }
        }

        public override long Position
        {
            get { return _baseStream.Position; }
            set { _baseStream.Position = value; }
        }

        #endregion

        #region Overrides of Stream

        public override void Close()
        {
            _memoryStream.Close();
            _baseStream.Close();
            base.Close();
        }

        #endregion
    }
}
