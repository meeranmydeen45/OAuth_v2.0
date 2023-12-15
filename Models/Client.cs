namespace OauthServer.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string ClientId { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string RedirectUri { get; set; } = string.Empty;
        public string AllowedCorsOrigin { get; set; } = string.Empty;
    }
}
