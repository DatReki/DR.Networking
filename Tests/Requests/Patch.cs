using Generate.Models;
using System.Web;

namespace Tests.Requests
{
    [TestFixture]
    public class Patch
    {
        [OneTimeSetUp]
        public async Task Setup()
        {
            DR.Networking.RateLimiting.UpdateGlobal(Main.RateLimitDuration);
        }

        [Test]
        public async Task EditUser()
        {
            (List<User>? users, string? error) = await Core.LibraryRequests.GetUsers();
            if (!string.IsNullOrWhiteSpace(error))
            {
                Assert.Fail(error);
                return;
            }
            else if (users == null || users.Count == 0) // If this method is tested separately than no users will have been added yet.
            {
                string createError = await Core.LibraryRequests.CreateUser(Intermediate.Generate.User());
                if (!string.IsNullOrWhiteSpace(createError))
                {
                    Assert.Fail(createError);
                    return;
                }

                (users, error) = await Core.LibraryRequests.GetUsers();
                if (!string.IsNullOrWhiteSpace(error))
                {
                    Assert.Fail(error);
                    return;
                }
            }

            if (users == null)
            {
                Assert.Fail("No users have been added yet");
                return;
            }

            int index = Generate.Main.RandomNumber(0, users.Count);
            User user = users[index];
            User changedUser = Intermediate.Generate.User();

            string encodedEmail = HttpUtility.UrlEncode(user.Email);
            (string changedResponse, string changedError) = await Core.LibraryRequests.EditUser(encodedEmail, changedUser);
            if (!string.IsNullOrWhiteSpace(changedError))
            {
                Assert.Fail(changedError);
                return;
            }

            Assert.That($"User updated '{changedUser.Username}'", Is.EqualTo(changedResponse));
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            DR.Networking.RateLimiting.UpdateGlobal(null);
        }
    }
}
