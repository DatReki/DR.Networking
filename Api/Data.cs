using Generate.Models;
using System.Collections.ObjectModel;

namespace Api
{
    public class Data
    {
        /// <summary>
        /// A list of users who can access specific endpoints
        /// </summary>
        public static List<ApiUser> ApiUsers { get; set; } = [];

        /// <summary>
        /// A list of <see cref="User"/>s that have been added through the API
        /// </summary>
        public static ObservableCollection<User> Users { get; private set; } = [];

        /// <summary>
        /// Configure everything required to use the <see cref="Data"/> class. Is only required to be called once on startup.
        /// </summary>
        internal static void Setup()
        {
            Users.CollectionChanged += UsersChanged;
        }

        /// <summary>
        /// When a new <see cref="User"/> get's added to the list give them an id.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void UsersChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems == null)
                return;

            foreach (User user in e.NewItems)
            {
                if (user.Id != -1)
                    continue;

                int id = 1;
                if (Users.Any())
                {
                    int max = Users.Max(x => x.Id);
                    if (max > id)
                        id = max++;
                }

                user.Id = id;
            }
        }
    }
}
