using Bogus;

namespace Generate.Models
{
    public class ApiUser
    {
        public ApiUser() { }

        public ApiUser(Faker faker)
        {
            ClientId = Main.RandomGuid(faker);
            ClientSecret = faker.Internet.Password(Main.RandomNumber(10, 64));
        }

        public Guid ClientId { get; set; } = new Guid();
        public string ClientSecret { get; set; } = string.Empty;
    }
}
