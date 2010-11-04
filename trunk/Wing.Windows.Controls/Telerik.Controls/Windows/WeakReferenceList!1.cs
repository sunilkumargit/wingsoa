namespace Telerik.Windows
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    internal class WeakReferenceList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
    {
        private List<WeakReference> items;

        public WeakReferenceList()
        {
            this.items = new List<WeakReference>();
        }

        public void Add(T item)
        {
            this.Insert(this.items.Count, item);
        }

        public void Clear()
        {
            this.items.Clear();
        }

        public bool Contains(T item)
        {
            return (this.IndexOf(item) > -1);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int i = 0; i < this.items.Count; i++)
            {
                array[arrayIndex + i] = (T)this.items[i].Target;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new WeakReferenceEnumerator(this.items.GetEnumerator());
        }

        public int IndexOf(T item)
        {
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < this.items.Count; i++)
            {
                if (comparer.Equals((T)this.items[i].Target, item))
                {
                    return i;
                }
            }
            return -1;
        }

        public void Insert(int index, T item)
        {
            this.items.Insert(index, new WeakReference(item));
        }

        public bool Remove(T item)
        {
            int i = this.IndexOf(item);
            if (i < 0)
            {
                return false;
            }
            this.RemoveAt(i);
            return true;
        }

        public void RemoveAt(int index)
        {
            this.items.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public int Count
        {
            get
            {
                return this.items.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public T this[int index]
        {
            get
            {
                if (this.items[index].IsAlive)
                {
                    return (T)this.items[index].Target;
                }
                return default(T);
            }
            set
            {
                if (this.items.Count <= index)
                {
                    throw new ArgumentOutOfRangeException("index", "Index out of range!");
                }
                this.items[index] = new WeakReference(value);
            }
        }

        public class WeakReferenceEnumerator : IEnumerator<T>, IDisposable, IEnumerator
        {
            internal WeakReferenceEnumerator(IEnumerator<WeakReference> baseEnumerator)
            {
                this.BaseEnumerator = baseEnumerator;
            }

            public void Dispose()
            {
                this.BaseEnumerator.Dispose();
            }

            public bool MoveNext()
            {
                return this.BaseEnumerator.MoveNext();
            }

            public void Reset()
            {
                this.BaseEnumerator.Reset();
            }

            private IEnumerator<WeakReference> BaseEnumerator { get; set; }

            public T Current
            {
                get
                {
                    return (T)this.BaseEnumerator.Current.Target;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return this.Current;
                }
            }
        }
    }
}

