using Api.Models;
using Bogus;

namespace Backend
{
    public class Generate
    {
        public static User User()
        {
            Faker<User> user = new Faker<User>()
                .RuleFor(x => x.Username, (y, z) => y.Internet.UserName(z.PersonalInformation.Firstname, z.PersonalInformation.Lastname))
                .RuleFor(x => x.PersonalInformation.Gender, y => y.PickRandom<User.GenderType>());

            return user.Generate();
        }
    }
}
