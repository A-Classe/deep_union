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

        private abstract class ListDummy<T>
        {
            internal T[] _items = default!;
            internal int _size;
            internal int _version;
#if NETFRAMEWORK
        internal object _syncRoot = default!;
#endif

            private ListDummy() { }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<T> AsSpan<T>(this List<T> list)
        {
            return Unsafe.As<_<T>>(list).Items.AsSpan(0, list.Count);
        }

        public static void AddRange<T>(this List<T> list, ReadOnlySpan<T> span) => list.InsertRange(list.Count, span);

        public static void InsertRange<T>(this List<T> list, int index, ReadOnlySpan<T> span)
        {
            if (list is null)
            {
                throw new ArgumentNullException();
            }

            static void EnsureCapacity(List<T> original, ListDummy<T> dummy, int min)
            {
                if (dummy._items.Length < min)
                {
                    const int defaultCapacity = 4;
                    int newCapacity = dummy._items.Length == 0 ? defaultCapacity : dummy._items.Length * 2;

                    // Allow the list to grow to maximum possible capacity (~2G elements) before encountering overflow.
                    // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
                    const int maxArrayLength = 0X7FEFFFFF; // This size is compatible with Array.MaxArrayLength. (This is internal)
                    if ((uint)newCapacity > maxArrayLength) newCapacity = maxArrayLength;
                    if (newCapacity < min) newCapacity = min;
                    original.Capacity = newCapacity;
                }
            }

            var dummyList = Unsafe.As<ListDummy<T>>(list);
            if ((uint)index > (uint)dummyList._size)
            {
                throw new ArgumentOutOfRangeException();
            }

            int count = span.Length;
            if (count > 0)
            {
                EnsureCapacity(list, dummyList, dummyList._size + count);
                if (index < dummyList._size)
                {
                    Array.Copy(dummyList._items, index, dummyList._items, index + count, dummyList._size - index);
                }

                span.CopyTo(dummyList._items.AsSpan(index));
                dummyList._size += count;
                dummyList._version++;
            }
        }
    }
}