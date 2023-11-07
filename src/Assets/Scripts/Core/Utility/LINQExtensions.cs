using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Utility
{
    public static class LinqExtensions
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.OrderBy(item => Guid.NewGuid());
        }
    }
}