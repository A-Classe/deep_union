using System;
using System.Collections;
using System.Collections.Generic;

// ReSharper disable RedundantExtendsListEntry

// 参考
// https://github.com/dotnet/dotNext/blob/5c27d9ec80efe3d253bed4a15b70791336e543b8/src/DotNext/Collections/Generic/ListSegment.cs

namespace Core.Utility
{
    /// <summary>
    ///     ArraySegmentのList版
    ///     リストの性質上、親リストが変更されるとセグメントの内容も変わるので注意
    ///     また、バージョン変更を監視しないため、イテレート時にコレクションを変更しないでください
    /// </summary>
    /// <typeparam name="T">要素の型</typeparam>
    public readonly struct ListSegment<T> : IList<T>, IReadOnlyList<T>
    {
        public int Count { get; }

        private readonly int startIndex;
        private readonly IList<T> list;

        public ListSegment(IList<T> list, int startIndex, int offset)
        {
            this.list = list;
            this.startIndex = startIndex;
            Count = offset;
        }

        bool ICollection<T>.IsReadOnly => true;

        public T this[int index]
        {
            get => list[startIndex + index];
            set => list[startIndex + index] = value;
        }

        public int IndexOf(T item)
        {
            var index = list.IndexOf(item);

            if (startIndex <= index && index <= startIndex + Count)
            {
                return index - startIndex;
            }

            return -1;
        }

        void IList<T>.Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            switch (list)
            {
                case List<T> typedList:
                    typedList.CopyTo(startIndex, array, arrayIndex, Count);
                    break;
                case T[] source:
                    Array.Copy(source, startIndex, array, arrayIndex, Count);
                    break;
                default:
                    for (var i = 0; i < Count; i++)
                    {
                        array[i] = list[i + startIndex];
                    }

                    break;
            }
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(list);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [Serializable]
        public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
        {
            private T current;
            private int index;
            private IList<T> list;

            internal Enumerator(IList<T> ls)
            {
                list = ls;
                index = 0;

                current = default;
            }

            public void Dispose() { }

            public bool MoveNext()
            {
                if ((uint)index >= (uint)list.Count)
                {
                    return MoveNextRare();
                }

                current = list[index];

                ++index;
                return true;
            }

            public T Current => current;

            object IEnumerator.Current => Current;

            void IEnumerator.Reset()
            {
                index = 0;
                current = default;
            }

            private bool MoveNextRare()
            {
                index = list.Count + 1;
                current = default;
                return false;
            }
        }
    }
}