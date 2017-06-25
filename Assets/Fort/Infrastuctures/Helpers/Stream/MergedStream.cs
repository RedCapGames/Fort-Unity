using System;
using System.IO;
using System.Linq;

namespace Fort.Stream
{    
    internal class MergedStream:System.IO.Stream
    {
        #region Fields

        private long _position;
        private readonly long _length;
        readonly System.IO.Stream[] _streams;
        readonly bool _isSeakable;
        private int _streamIndex;
        #endregion

        #region Constructors

        public MergedStream(params System.IO.Stream[] streams)
        {
            if(streams.Length<2)
                throw new Exception("streams length Must be at least 2");
            _streamIndex = 0;
            _streams = streams;
            
            _isSeakable = streams.All(o => o.CanSeek);
            if (_isSeakable)
                _length = streams.Sum(stream => stream.Length);
            _position = 0;
        } 
        #endregion

        #region Implementation of Stream
        public override void Flush()
        {
            lock (this)
            {
                foreach (var stream in _streams)
                {
                    stream.Flush();
                } 
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            lock (this)
            {
                long desirePosition;
                if (!_isSeakable)
                    throw new StreamIsNotSeekableException();
                switch (origin)
                {
                    case SeekOrigin.Begin:
                        desirePosition = offset;
                        break;
                    case SeekOrigin.Current:
                        desirePosition = _position + offset;
                        break;
                    case SeekOrigin.End:
                        desirePosition = Length - offset;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("origin");
                }
                if (desirePosition > Length || desirePosition < 0)
                    throw new InvalidOffsetForSeekingException();

                long pos = 0;
                for (int i = 0; i < _streams.Length; i++)
                {
                    if (desirePosition < pos + _streams[i].Length)
                    {

                        _streams[i].Seek(desirePosition - pos, SeekOrigin.Begin);
                        _streamIndex = i;
                        _position = desirePosition;
                        return desirePosition;
                    }
                }
                return -1;
            }
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            lock (this)
            {
                if (offset < 0)
                    throw new Exception("Negetive Offset!");
                if (count < 0)
                    throw new Exception("Negetive Count!");
                int result = 0;
                while (count > 0)
                {
                    int readSize = _streams[_streamIndex].SafeRead(buffer, offset, count);
                    //End of Last Stream
                    if (readSize == 0 && _streamIndex == _streams.Length - 1)
                        return result;
                    result += readSize;
                    count -= readSize;
                    offset += readSize;
                    if (readSize == 0)
                        _streamIndex++;
                }
                return result;
            }
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
            get { return _isSeakable; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override long Length
        {
            get
            {
                lock (this)
                {
                    return _length; 
                }
            }
        }

        public override long Position
        {
            get
            {
                lock (this)
                {
                    return _position; 
                }
            }
            set { Seek(value,SeekOrigin.Begin); }
        }

        #endregion
    }
}
