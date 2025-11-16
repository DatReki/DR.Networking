namespace Api.Models
{
    public class ApiUser
    {
        public Guid ClientId { get; set; } = new Guid();
        public string ClientSecret { get; set; } = string.Empty;
    }
}
