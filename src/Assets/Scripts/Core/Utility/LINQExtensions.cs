using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Utility
{
    public static class LinqExtensions
    {
        /// <summary>
        /// コレクションをシャッフルします。
        /// </summary>
        /// <param name="source"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.OrderBy(item => Guid.NewGuid());
        }
    }
}