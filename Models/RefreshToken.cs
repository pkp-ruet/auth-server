namespace AuthServer.Models
{
    public class RefreshToken
    {
        public string Id { get; set; }
        public string Token { get; set;}
        public string UserId { get; set; }
    }
}
