namespace MessageContract
{
    public class MessageRequest
    {
        public long Id { get; set; }
        public string Value { get; set; }
        public string Hash { get; set; }

        public MessageRequest(long id, string value, string hash)
        {
            this.Id = id;
            this.Value = value;
            this.Hash = hash;
        }
    }
}
