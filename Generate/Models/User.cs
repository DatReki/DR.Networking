using Bogus;

namespace Generate.Models
{
    public class User
    {
        public User() { }

        public User(Faker faker)
        {
            Personal personal = new(faker);
            Username = faker.Internet.UserName(personal.Firstname, personal.Lastname);
            Password = faker.Internet.Password(Main.RandomNumber(10, 64));
            Email = faker.Internet.Email();
            PersonalInformation = personal;
        }

        public enum GenderType
        {
            Male,
            Female,
            Other,
        }

        public int Id { get; set; } = -1;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Personal PersonalInformation { get; set; } = new Personal();

        public class Personal
        {
            public Personal() { }

            public Personal(Faker faker)
            {
                Firstname = faker.Person.FirstName;
                Lastname = faker.Person.LastName;
                Gender = faker.PickRandom<GenderType>();
                Country = faker.Address.Country();
                CountryCode = faker.Address.CountryCode();
                State = faker.Address.State();
                City = faker.Address.City();
                Street = faker.Address.StreetName();
                BuildingNumber = faker.Address.BuildingNumber();
                ZipCode = faker.Address.ZipCode();
            }

            public string Firstname { get; set; } = string.Empty;
            public string Surname { get; set; } = string.Empty;
            public string Lastname { get; set; } = string.Empty;
            public GenderType Gender { get; set; }
            public string Country { get; set; } = string.Empty;
            public string CountryCode { get; set; } = string.Empty;
            public string State { get; set; } = string.Empty;
            public string City { get; set; } = string.Empty;
            public string Street { get; set; } = string.Empty;
            public string BuildingNumber { get; set; } = string.Empty;
            public string HouseNumberAddition { get; set; } = string.Empty;
            public string ZipCode { get; set; } = string.Empty;
        }
    }
}
