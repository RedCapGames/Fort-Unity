using System;
using System.IO;

namespace Fort.Stream
{
    internal class StreamInspecter:System.IO.Stream
    {
        private readonly System.IO.Stream _baseStream;
        private readonly StreamInspectionActions _streamInspectionActions;

        public StreamInspecter(System.IO.Stream baseStream, StreamInspectionActions streamInspectionActions)
        {
            _baseStream = baseStream;
            _streamInspectionActions = streamInspectionActions;
        }

        #region Overrides of Stream

        public override void Flush()
        {
            _baseStream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _baseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _baseStream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _baseStream.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _baseStream.Write(buffer,offset,count);
        }

        public override bool CanRead { get { return _baseStream.CanRead; } }
        public override bool CanSeek { get { return _baseStream.CanSeek; } }
        public override bool CanWrite { get { return _baseStream.CanWrite; } }
        public override long Length { get { return _baseStream.Length; } }
        public override long Position { get { return _baseStream.Position; } set { _baseStream.Position = value; } }

        #endregion

        #region Overrides of Stream

        public override void Close()
        {
            System.IO.Stream baseStream = _baseStream;
            StreamInspectionActions streamInspectionActions = _streamInspectionActions;
            base.Close();
            baseStream.Close();
            if(streamInspectionActions != null && streamInspectionActions.Closed != null)
                streamInspectionActions.Closed.Invoke();
        }

        #endregion
    }

    public class StreamInspectionActions
    {
        public Action Closed { get; set; }
    }
}
