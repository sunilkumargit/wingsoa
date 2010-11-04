namespace Telerik.Windows.Controls.TreeView
{
    using System;

    internal class WeakReferenceKey<T>
    {
        private int hashCode;
        private WeakReference itemReference;

        public WeakReferenceKey(T item)
        {
            this.itemReference = new WeakReference(item);
            this.hashCode = item.GetHashCode();
        }

        public override bool Equals(object other)
        {
            if (other == this)
            {
                return true;
            }
            WeakReferenceKey<T> key = other as WeakReferenceKey<T>;
            if (key == null)
            {
                return false;
            }
            T item = this.Item;
            if (item == null)
            {
                return false;
            }
            return ((this.hashCode == key.hashCode) && object.Equals(item, key.Item));
        }

        public override int GetHashCode()
        {
            return this.hashCode;
        }

        public override string ToString()
        {
            if (!this.itemReference.IsAlive)
            {
                return "Dead WeakReferenceKey";
            }
            return this.itemReference.Target.ToString();
        }

        public T Item
        {
            get
            {
                return (T) this.itemReference.Target;
            }
        }
    }
}

