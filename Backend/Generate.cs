using Api.Models;
using Bogus;
using System.Security.Cryptography;

namespace Backend
{
    public class Generate
    {
        /// <summary>
        /// A list of users that have been generated.
        /// </summary>
        internal static List<User> Users { get; private set; } = [];

        /// <summary>
        /// Generate a new <see cref="Api.Models.User"/>.
        /// </summary>
        /// <returns></returns>
        public static User User()
        {
            bool unique = false;
            Faker faker = new();
            User user = new();

            while (!unique)
            {
                User.Personal personal = new()
                {
                    Firstname = faker.Person.FirstName,
                    Lastname = faker.Person.LastName,
                    Gender = faker.PickRandom<User.GenderType>(),
                    Country = faker.Address.Country(),
                    CountryCode = faker.Address.CountryCode(),
                    State = faker.Address.State(),
                    City = faker.Address.City(),
                    Street = faker.Address.StreetName(),
                    BuildingNumber = faker.Address.BuildingNumber(),
                    ZipCode = faker.Address.ZipCode(),
                };

                user = new()
                {
                    Username = faker.Internet.UserName(personal.Firstname, personal.Lastname),
                    Password = faker.Internet.Password(GetPasswordLength()),
                    Email = faker.Internet.Email(),
                    PersonalInformation = personal,
                };

                if (Users.Any(x => x.Username == user.Username && x.Email == user.Email))
                    continue;

                unique = true;
                Users.Add(user);
            }

            return user;
        }

        /// <summary>
        /// Get random password length.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private static int GetPasswordLength(int min = 15, int max = 125)
            => RandomNumberGenerator.GetInt32(min, max);
    }
}
