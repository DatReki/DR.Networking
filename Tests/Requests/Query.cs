using Generate.Models;
using System.Web;

namespace Tests.Requests
{
    [TestFixture]
    [NonParallelizable]
    public class Query
    {
        [OneTimeSetUp]
        public async Task Setup()
        {
            DR.Networking.RateLimiting.UpdateGlobal(Main.RateLimitDuration);
        }

        [Test]
        public async Task GetUsersByEmail()
        {
            (List<User>? users, string? error) = await Core.LibraryRequests.GetUsers();
            if (!string.IsNullOrWhiteSpace(error))
            {
                Assert.Fail(error);
                return;
            }
            else if (users == null || users.Count == 0) // If this method is tested separately than no users will have been added yet.
            {
                for (int i = 0; i < Generate.Main.RandomNumber(5, 20); i++)
                {
                    string createError = await Core.LibraryRequests.CreateUser(Intermediate.Generate.User());
                    if (!string.IsNullOrWhiteSpace(createError))
                    {
                        Assert.Fail(createError);
                        return;
                    }
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

            List<string> emails = [];
            List<int> checkedIndexes = [];
            for (int i = 0; i < Generate.Main.RandomNumber(1, users.Count); i++)
            {
                int index = Generate.Main.RandomNumber(0, users.Count);
                while (checkedIndexes.Contains(index))
                    index = Generate.Main.RandomNumber(0, users.Count);

                emails.Add(users[index].Email);
            }

            List<string> encodedEmails = [.. emails.Select(x => HttpUtility.UrlEncode(x))];
            (string rawResponse, string changedError) = await Core.LibraryRequests.GetUsersByEmail(encodedEmails);
            if (!string.IsNullOrWhiteSpace(changedError))
            {
                Assert.Fail(changedError);
                return;
            }
            else if (string.IsNullOrWhiteSpace(rawResponse))
            {
                Assert.Fail($"'Query/GetUsersByEmail' returned an empty response");
                return;
            }
            else if (!Backend.Tools.TryParseJson(rawResponse, out List<User>? responseUsers) || responseUsers == null)
            {
                Assert.Fail($"Unable to parse response");
                return;
            }
            else if (responseUsers.Count == 0)
            {
                Assert.Fail($"'Query/GetUsersByEmail' returned no users");
                return;
            }
            else if (responseUsers.Any(x => !emails.Contains(x.Email)))
            {
                Assert.Fail($"'Query/GetUsersByEmail' returned one or more users with an unexpected email");
                return;
            }

            Assert.Pass();
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            DR.Networking.RateLimiting.UpdateGlobal(null);
        }
    }
}
