using System;
using System.IO;

namespace Fort.Stream
{
    public class ActionStream : System.IO.Stream
    {
        private readonly StreamActions _streamActions;
        private long _position;
        public ActionStream(StreamActions streamActions)
        {
            _position = 0;
            _streamActions = streamActions;
        }

        #region Overrides of Stream

        public override void Flush()
        {
            if (_streamActions.Flush == null)
                throw new NotSupportedException();
            _streamActions.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (_streamActions.Seek == null)
                throw new NotSupportedException();
            return _position = _streamActions.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            if (_streamActions.SetLength == null)
                throw new NotSupportedException();
            _streamActions.SetLength(value);
            if (value < _position)
                _position = value;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_streamActions.Read == null)
                throw new NotSupportedException();
            int read = _streamActions.Read(buffer, offset, count);
            _position += read;
            return read;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (_streamActions.Write == null)
                throw new NotSupportedException();
            _streamActions.Write(buffer, offset, count);
            _position += count;
        }

        public override bool CanRead
        {
            get { return _streamActions.Read != null; }
        }

        public override bool CanSeek
        {
            get { return _streamActions.Seek != null; }
        }

        public override bool CanWrite
        {
            get { return _streamActions.Write != null; }
        }

        public override long Length
        {
            get
            {
                if (_streamActions.GetLength == null)
                    throw new NotSupportedException();
                return _streamActions.GetLength();
            }
        }

        public override long Position
        {
            get { return _position; }
            set { Seek(value, SeekOrigin.Begin); }
        }

        #endregion
    }

    public class StreamActions
    {
        public StreamActions()
        {
            Flush = () => { };
        }

        public Func<byte[], int, int, int> Read { get; set; }
        public Action<byte[], int, int> Write { get; set; }
        public Func<long> GetLength { get; set; }
        public Action<long> SetLength { get; set; }
        public Func<long, SeekOrigin, long> Seek { get; set; }
        public Action Flush { get; set; }
    }
}
