using Bogus;
using Generate.Models;
using System.Security.Cryptography;

namespace Generate
{
    public class Main
    {
        public static int RandomNumber(int? min = null, int? max = null)
        {
            min ??= int.MinValue;
            max ??= int.MaxValue;

            return RandomNumberGenerator.GetInt32((int)min, (int)max);
        }

        public static string RandomString(int? min = null, int? max = null, Faker? faker = null)
        {
            faker ??= new Faker();
            return faker.Random.String(RandomNumber(min, max));
        }

        public static string RandomText(int? min = null, int? max = null, Faker? faker = null)
        {
            faker ??= new Faker();
            return faker.Lorem.Paragraphs(RandomNumber(min, max));
        }

        public static Guid RandomGuid(Faker? faker = null)
        {
            faker ??= new Faker();
            return faker.Random.Guid();
        }

        public static User RandomUser(Faker? faker = null)
        {
            faker ??= new Faker();
            User user = new(faker);
            return user;
        }

        public static ApiUser RandomApiUser(Faker? faker = null)
        {
            faker ??= new Faker();
            ApiUser user = new(faker);

            return user;
        }

        public static Vehicle RandomVehicle(Faker? faker = null)
        {
            faker ??= new Faker();
            return new Vehicle(faker);
        }

        public static List<Vehicle> RandomVehicles(int? min = null, int? max = null, Faker ? faker = null)
        {
            faker ??= new Faker();

            List<Vehicle> vehicles = [];
            for (int i = 0; i < RandomNumber(min, max); i++)
                vehicles.Add(new Vehicle(faker));

            return vehicles;
        }
    }
}
