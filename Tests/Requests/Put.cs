using Generate.Models;
using System.Web;

namespace Tests.Requests
{
    [TestFixture]
    [NonParallelizable]
    public class Put
    {
        [OneTimeSetUp]
        public async Task Setup()
        {
            DR.Networking.RateLimiting.UpdateGlobal(Main.RateLimitDuration);
        }

        [Test]
        public async Task CheckUser()
        {
            User user = Intermediate.Generate.User();
            string createError = await Core.LibraryRequests.CreateUser(user);
            if (!string.IsNullOrWhiteSpace(createError))
            {
                Assert.Fail(createError);
                return;
            }

            string encodedEmail = HttpUtility.UrlEncode(user.Email);
            (User? foundUser, string getError) = await Core.LibraryRequests.GetUserByEmail(encodedEmail);
            if (!string.IsNullOrWhiteSpace(getError))
            {
                Assert.Fail(getError);
                return;
            }

            Assert.That(foundUser.Compare(user), Is.True, "Unable to find the created user");
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            DR.Networking.RateLimiting.UpdateGlobal(null);
        }
    }
}
