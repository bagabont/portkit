using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PortKit.Extensions
{
    public static class EnumerableExtensions
    {
        private static readonly Random Random = new Random();

        public static T RandomElement<T>(this IEnumerable<T> list)
        {
            var array = list.ToArray();

            var index = Random.Next(0, array.Length);

            return array[index];
        }

        public static int Count(this IEnumerable enumerable)
        {
            if (enumerable == null)
            {
                return 0;
            }

            if (enumerable is ICollection itemsList)
            {
                return itemsList.Count;
            }

            var enumerator = enumerable.GetEnumerator();

            var count = 0;
            while (enumerator.MoveNext())
            {
                count++;
            }

            return count;
        }

        public static object ElementAt(this IEnumerable items, int position)
        {
            if (items == null)
            {
                return null;
            }

            if (items is IList itemsList)
            {
                return itemsList[position];
            }

            var enumerator = items.GetEnumerator();
            for (var i = 0; i <= position; i++)
            {
                enumerator.MoveNext();
            }

            return enumerator.Current;
        }
    }
}