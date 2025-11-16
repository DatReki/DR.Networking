using Api.Models;
using Bogus;
using System.Security.Cryptography;
using System.Text;

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
                    Password = faker.Internet.Password(GetRandomNumber()),
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
        /// Generate a new <see cref="Api.Models.ApiUser"/>.
        /// </summary>
        /// <returns></returns>
        public static ApiUser ApiUser()
        {
            bool unique = false;
            Faker faker = new();
            ApiUser user = new();

            while (!unique)
            {
                user = new ApiUser()
                {
                    ClientId = GenerateGuid(faker),
                    ClientSecret = faker.Internet.Password(GetRandomNumber()),
                };

                if (Api.Data.ApiUsers.Any(x => x.ClientId == user.ClientId && x.ClientSecret == user.ClientSecret))
                    continue;

                unique = true;
                Api.Data.ApiUsers.Add(user);
            }

            return user;
        }

        /// <summary>
        /// Get random password length.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private static int GetRandomNumber(int min = 15, int max = 125)
            => RandomNumberGenerator.GetInt32(min, max);

        /// <summary>
        /// Generate a random <see cref="Guid"/>.
        /// </summary>
        /// <param name="faker"></param>
        /// <returns></returns>
        private static Guid GenerateGuid(Faker? faker)
        {
            string input;
            if (faker != null)
                input = faker.Internet.Password(GetRandomNumber());
            else
                input = GetRandomString();

            byte[] hash = MD5.HashData(Encoding.UTF8.GetBytes(input));
            return new Guid(hash);
        }

        /// <summary>
        /// Get a completely random string based on a min & max length
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private static string GetRandomString(int min = 15, int max = 125)
        {
            StringBuilder builder = new();
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            int length = GetRandomNumber(min, max);

            while (builder.Length < length)
                builder.Append(chars[GetRandomNumber(0, chars.Length)]);

            return builder.ToString();
        }
    }
}
