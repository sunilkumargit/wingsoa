namespace Telerik.Windows.Controls.TreeView
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    internal class IndexTree : ICollection, IEnumerable<double>, IEnumerable
    {
        private int count;
        private double itemDefaultValue;
        private int size;
        private object syncRoot;
        private double[] tree;

        internal IndexTree(int capacity) : this(capacity, 0.0)
        {
        }

        internal IndexTree(int capacity, double defaultValue)
        {
            this.count = capacity;
            this.size = 8;
            while (this.size < capacity)
            {
                this.size = this.size << 1;
            }
            this.tree = new double[this.size << 1];
            this.itemDefaultValue = defaultValue;
            this.Initialize(defaultValue);
        }

        public void Add(double value)
        {
            if (this.Count == this.Capacity)
            {
                this.DoubleCapacity();
            }
            this.count++;
            this[this.Count - 1] = value;
        }

        [Conditional("DEBUG")]
        private void CheckIndex(int index)
        {
        }

        [Conditional("DEBUG")]
        private static void CheckValue(double value)
        {
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        internal int CumulativeIndex(double value)
        {
            if (value > this.tree[1])
            {
                return (this.count - 1);
            }
            int index = 1;
            while (index < this.size)
            {
                if (this.tree[2 * index] < value)
                {
                    value -= this.tree[2 * index];
                    index = (2 * index) + 1;
                }
                else
                {
                    index = 2 * index;
                }
            }
            return Math.Min((int) (index - this.size), (int) (this.count - 1));
        }

        internal double CumulativeValue(int endIndex)
        {
            int index = endIndex + this.size;
            double result = this.tree[index];
            while (index != 1)
            {
                if ((index % 2) == 1)
                {
                    result += this.tree[index - 1];
                }
                index = index >> 1;
            }
            return result;
        }

        private void DoubleCapacity()
        {
            int newSize = this.size * 2;
            double[] newTree = new double[newSize * 2];
            newTree[1] = this.tree[1];
            int startOldIndex = 1;
            int endOldIndex = 2;
            int index = 1;
            while (index < newSize)
            {
                for (index = startOldIndex; index < endOldIndex; index++)
                {
                    newTree[startOldIndex + index] = this.tree[index];
                }
                startOldIndex = startOldIndex << 1;
                endOldIndex = endOldIndex << 1;
            }
            this.tree = newTree;
            this.size = newSize;
        }

        private void Initialize(double defaultValue)
        {
            if (defaultValue != 0.0)
            {
                int index = (this.size * 2) - 1;
                while (index != 1)
                {
                    int half = index >> 1;
                    while (index > half)
                    {
                        this.tree[index] = defaultValue;
                        index--;
                    }
                    defaultValue *= 2.0;
                }
                this.tree[1] = this.tree[2] + this.tree[3];
            }
        }

        public void Insert(int index, double value)
        {
            if (index == this.Count)
            {
                this.Add(value);
            }
            else
            {
                if (this.Count == (this.Capacity - 1))
                {
                    this.DoubleCapacity();
                }
                this.Add(this.itemDefaultValue);
                for (int i = this.count - 1; i > index; i--)
                {
                    this.SwapItemsAtIndexes(i - 1, i);
                }
                if (this.Count == (this.Capacity - 1))
                {
                    this.DoubleCapacity();
                }
                this[index] = value;
            }
        }

        public void RemoveAt(int index)
        {
            this[index] = this.itemDefaultValue;
            for (int i = index; i < (this.count - 1); i++)
            {
                this.SwapItemsAtIndexes(i, i + 1);
            }
            this[this.count - 1] = 0.0;
            this.count--;
        }

        private void Set(int index, double value)
        {
            index += this.size;
            double oldValue = this.tree[index];
            double dif = value - oldValue;
            if (dif != 0.0)
            {
                while (index > 0)
                {
                    this.tree[index] += dif;
                    index = index >> 1;
                }
            }
        }

        private void SwapItemsAtIndexes(int lowerIndex, int higherIndex)
        {
            double difference = this[lowerIndex] - this[higherIndex];
            if (difference != 0.0)
            {
                lowerIndex += this.size;
                higherIndex += this.size;
                while (lowerIndex != higherIndex)
                {
                    this.tree[lowerIndex] -= difference;
                    this.tree[higherIndex] += difference;
                    lowerIndex = lowerIndex >> 1;
                    higherIndex = higherIndex >> 1;
                }
            }
        }

        IEnumerator<double> IEnumerable<double>.GetEnumerator()
        {
            //.TODO.
            return new List<double>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<double>) this).GetEnumerator();
        }

        public int Capacity
        {
            get
            {
                return this.size;
            }
        }

        public int Count
        {
            get
            {
                return this.count;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        internal double this[int index]
        {
            get
            {
                return this.tree[index + this.size];
            }
            set
            {
                this.Set(index, value);
            }
        }

        public object SyncRoot
        {
            get
            {
                return (this.syncRoot ?? (this.syncRoot = new object()));
            }
        }
   }
}

