namespace UCAE_KeyStore
{
    public class CommandModel
    {
        public int CommandId { get; set; }
        public byte[] CommandValue { get; set; }
        public string SessionKey { get; set; }
    }
}
