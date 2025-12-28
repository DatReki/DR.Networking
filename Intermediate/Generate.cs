using Api;
using Bogus;
using Generate.Models;
using Generation = Generate.Main;

namespace Intermediate
{
    public class Generate
    {
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
                user = Generation.RandomUser(faker);
                if (Data.Users.Any(x => x.Username == user.Username && x.Email == user.Email))
                    continue;

                unique = true;
                Data.Users.Add(user);
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
                user = Generation.RandomApiUser(faker);
                if (Data.ApiUsers.Any(x => x.ClientId == user.ClientId && x.ClientSecret == user.ClientSecret))
                    continue;

                unique = true;
                Data.ApiUsers.Add(user);
            }

            return user;
        }
    }
}
