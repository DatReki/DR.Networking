using System.Collections.ObjectModel;
using System.Reflection;

namespace Api.Core.Extensions
{
    public static class Compare
    {
        /// <summary>
        /// Get the index of an item within a <see cref="ObservableCollection{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static int DeepIndexOf<T>(this ObservableCollection<T> collection, Func<T, bool> predicate)
        {
            int index = -1;
            T? found = collection.FirstOrDefault(predicate);

            if (found == null)
                return index;

            index = collection.DeepIndexOf(found);
            return index;
        }

        /// <summary>
        /// Get the index of an item within a <see cref="ObservableCollection{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public static int DeepIndexOf<T>(this ObservableCollection<T> collection, T search)
        {
            int index = -1;

            for (int i = 0; i < collection.Count; i++)
            {
                T? item = collection[i];
                if (item.DeepEquals(search))
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        /// <summary>
        /// Preform a deep comparison between two objects (also check all of the properties within the two objects).
        /// </summary>
        public static bool DeepEquals<T>(this T expected, T actual)
            => expected.DeepCompare(actual);

        /// <summary>
        /// Preform a deep comparison between two objects (also check all of the properties within the two objects).
        /// </summary>
        public static bool DeepCompare<T>(this T expected, T actual)
        {
            if (expected == null && actual == null)
                return true;

            if (expected == null || actual == null)
                return false;

            if (!expected.GetType().Equals(actual.GetType()))
                return false;

            Type type = expected.GetType();
            if (type.IsPrimitive || typeof(string).Equals(type) || typeof(DateTime).Equals(type))
            {
                return expected.Equals(actual);
            }
            else if (typeof(IEnumerable<T>).IsAssignableFrom(type))
            {
                return ((IEnumerable<T>)expected).DeepCompare((IEnumerable<T>)actual);
            }
            else
            {
                PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                foreach (PropertyInfo prop in props)
                {
                    object? expectedVal = prop.GetValue(expected, null);
                    object? actualVal = prop.GetValue(actual, null);
                    if (!expectedVal.DeepCompare(actualVal))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Compare the data from two different IEnumerables.
        /// </summary>
        public static bool DeepCompare<T>(this IEnumerable<T> expected, IEnumerable<T> actual)
        {
            IEnumerator<T> actualVal = actual.GetEnumerator();
            IEnumerator<T>? expectedVal = expected.GetEnumerator();

            if (expectedVal == null && actualVal == null)
                return true;
            else if (expectedVal == null || actualVal == null)
                return false;

            int actualCount = 0;
            int expectedCount = 0;
            Dictionary<object, int> map = [];

            if (expectedVal != null)
            {
                while (expectedVal.MoveNext())
                {
                    if (expectedVal.Current != null)
                    {
                        if (!map.ContainsKey(expectedVal.Current))
                            map[expectedVal.Current] = 1;
                        else
                            map[expectedVal.Current] = ++map[expectedVal.Current];
                    }

                    expectedCount++;
                }
            }

            if (actualVal != null)
            {
                while (actualVal.MoveNext())
                {
                    if (actualVal.Current != null)
                    {
                        if (!map.ContainsKey(actualVal.Current))
                            return false;

                        if (map[actualVal.Current] == 0)
                            return false;

                        map[actualVal.Current] = --map[actualVal.Current];
                    }

                    actualCount++;
                }
            }

            if (expectedCount != actualCount)
                return false;

            return true;
        }
    }
}
