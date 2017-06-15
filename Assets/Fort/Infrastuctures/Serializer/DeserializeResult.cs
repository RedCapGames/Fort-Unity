namespace Fort.Serializer
{
    public class DeserializeResult
    {
        public DeserializeResult()
        {
            Use = true;
        }
        public object Result { get; set; }
        public bool Use { get; set; }
    }
}