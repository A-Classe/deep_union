using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Core.Utility
{
    public static class SpanUtility
    {
        // ReSharper disable once ClassNeverInstantiated.Local
        private class _<T>
        {
            internal T[] Items;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<T> AsSpan<T>(this List<T> list)
        {
            return Unsafe.As<_<T>>(list).Items.AsSpan(0, list.Count);
        }
    }
}