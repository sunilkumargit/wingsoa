namespace Telerik.Windows.Controls.TreeView
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows.Automation;

    public class CheckedItemsCollection : ICollection<object>, ICollection, IEnumerable<object>, IEnumerable, INotifyPropertyChanged, INotifyCollectionChanged
    {
        private static PropertyChangedEventArgs countChangedEventArgs = new PropertyChangedEventArgs("Count");
        private ICollection<WeakReferenceKey<object>> innerCollection;
        private IDictionary<WeakReferenceKey<object>, ToggleState> innerDictionary;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        internal CheckedItemsCollection(ICollection<WeakReferenceKey<object>> innerCollection, IDictionary<WeakReferenceKey<object>, ToggleState> innerDictionary)
        {
            this.innerCollection = innerCollection;
            this.innerDictionary = innerDictionary;
        }

        public void Add(object item)
        {
            throw new NotSupportedException("Adding items to the CheckedItems collection is not supported.");
        }

        public void Clear()
        {
            throw new NotSupportedException("The CheckedItems is ReadOnly and it cannot be modified.");
        }

        public bool Contains(object item)
        {
            WeakReferenceKey<object> key = new WeakReferenceKey<object>(item);
            return this.innerCollection.Contains(key);
        }

        public void CopyTo(object[] array, int arrayIndex)
        {
            if (this.innerCollection.Any<WeakReferenceKey<object>>())
            {
                foreach (WeakReferenceKey<object> item in from key in this.innerCollection
                    where key.Item != null
                    select key)
                {
                    array[arrayIndex++] = item.Item;
                }
            }
        }

        public void CopyTo(Array array, int index)
        {
            (this.innerCollection as ICollection).CopyTo(array, index);
        }

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification="The method implies that the return value involves computation.")]
        public IEnumerator<object> GetCheckedItemsOnly()
        {
            //.TODO.
            return new List<Object>().GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<object>) this).GetEnumerator();
        }

        internal void NotifyCountChanged()
        {
            this.OnPropertyChanged(countChangedEventArgs);
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        internal void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, args);
            }
        }

        public bool Remove(object item)
        {
            WeakReferenceKey<object> key = new WeakReferenceKey<object>(item);
            bool result = this.innerDictionary.Remove(key);
            this.NotifyCountChanged();
            return result;
        }

        IEnumerator<object> IEnumerable<object>.GetEnumerator()
        {
            //.TODO.
            return this.innerDictionary.Values.Cast<Object>().GetEnumerator();
        }

        public int Count
        {
            get
            {
                return this.innerCollection.Count<WeakReferenceKey<object>>(item => (item.Item != null));
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return (this.innerCollection as ICollection).IsSynchronized;
            }
        }

        public object SyncRoot
        {
            get
            {
                return (this.innerCollection as ICollection).SyncRoot;
            }
        }
    }
}

