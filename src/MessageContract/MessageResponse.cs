namespace MessageContract
{
    public class MessageResponse
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public string Hash { get; set; }

        public MessageResponse(int status, string message, string hash)
        {
            this.Status = status;
            this.Message = message;
            this.Hash = hash;
        }
    }
}
