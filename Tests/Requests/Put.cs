using Generate.Models;
using System.Web;

namespace Tests.Requests
{
    [TestFixture]
    public class Put
    {
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
    }
}
