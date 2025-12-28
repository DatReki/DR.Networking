using Api.Models;
using Generate.Models;

namespace Tests
{
    internal static class Extensions
    {
        internal static bool Compare(this User? found, User original)
        {
            if (found == null || found == default)
                return false;

            bool username = found.Username == original.Username;
            bool password = found.Password == original.Password;
            bool email = found.Email == original.Email;

            if (username && password && email)
                return true;

            return false;
        }

        internal static bool Compare(this AuthorizedResponse<User>? auth, User original)
        {
            if (auth == null || auth == default || !auth.Authorized)
                return false;

            if (auth.Content == null)
                return false;

            return auth.Content.Compare(original);
        }
    }
}
