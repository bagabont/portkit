using System;
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
    }
}