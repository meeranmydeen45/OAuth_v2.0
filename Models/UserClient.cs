namespace OauthServer.Models
{
    public class UserClient
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int ClientId { get; set; }
    }
}
