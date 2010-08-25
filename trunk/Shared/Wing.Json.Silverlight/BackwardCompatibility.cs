using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Wing.Json
{
    public class ArrayList : List<object>
    {
        public ArrayList()
        {
        }

        public ArrayList(System.Collections.IEnumerable items)
            : base(items.Cast<object>())
        {
        }

        public Array ToArray(Type type)
        {
            Array destinationArray = Array.CreateInstance(type, this.Count);
            Array.Copy(this.ToArray(), 0, destinationArray, 0, this.Count);
            return destinationArray;
        }

        public static System.Collections.IList ReadOnly(ArrayList list)
        {
            return new System.Collections.ObjectModel.ReadOnlyCollection<object>(list);
        }
    }

    public class Hashtable : Dictionary<object, object>
    {
        protected internal Dictionary<object, object> Dictionary
        {
            get
            {
                return this;
            }
        }

        new public object this[object key]
        {
            get
            {
                if (base.ContainsKey(key))
                {
                    return base[key];
                }

                return null;
            }
            set
            {
                if (base.ContainsKey(key))
                {
                    base[key] = value;
                }

                base.Add(key, value);
            }
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection)this).CopyTo(array, index);
        }

        public bool Contains(object key)
        {
            return base.ContainsKey(key);
        }

        public bool IsSynchronized
        {
            get
            {
                return ((ICollection)this).IsSynchronized;
            }
        }

        public object SyncRoot
        {
            get
            {
                return ((ICollection)this).SyncRoot;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }
    }

    public class Stack : Stack<object>
    {
        public Stack()
        {
        }

        public Stack(int capacity)
            : base(capacity)
        {
        }
    }

    //public abstract class DictionaryBase : System.Collections.ObjectModel.KeyedCollection<object, object>
    //{
    //    protected internal System.Collections.ObjectModel.KeyedCollection<object, object> InnerHashtable
    //    {
    //        get
    //        {
    //            return this;
    //        }
    //    }
    //}

    //public static class DictionaryExtensions
    //{
    //    public static bool Contains(this DictionaryBase dict, object key)
    //    {
    //        return dict.ContainsKey(key);
    //    }
    //}

    public sealed class NameValueCollection : System.Collections.Generic.Dictionary<string, string>
    {
        public string GetKey(int i)
        {
            return this.ElementAt<KeyValuePair<string, string>>(i).Key;
        }

        public string GetValue(int i)
        {
            return this.ElementAt<KeyValuePair<string, string>>(i).Value;
        }

        public string[] GetValues(int i)
        {
            return new[] { this.GetValue(i) };
        }
    }


    public abstract class DictionaryBase : IDictionary, ICollection, IEnumerable
    {
        // Fields
        private Hashtable hashtable;

        // Methods
        protected DictionaryBase()
        {
        }

        public void Clear()
        {
            this.OnClear();
            this.InnerHashtable.Clear();
            this.OnClearComplete();
        }

        public void CopyTo(Array array, int index)
        {
            this.InnerHashtable.CopyTo(array, index);
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            return this.InnerHashtable.GetEnumerator();
        }

        protected virtual void OnClear()
        {
        }

        protected virtual void OnClearComplete()
        {
        }

        protected virtual object OnGet(object key, object currentValue)
        {
            return currentValue;
        }

        protected virtual void OnInsert(object key, object value)
        {
        }

        protected virtual void OnInsertComplete(object key, object value)
        {
        }

        protected virtual void OnRemove(object key, object value)
        {
        }

        protected virtual void OnRemoveComplete(object key, object value)
        {
        }

        protected virtual void OnSet(object key, object oldValue, object newValue)
        {
        }

        protected virtual void OnSetComplete(object key, object oldValue, object newValue)
        {
        }

        protected virtual void OnValidate(object key, object value)
        {
        }

        void IDictionary.Add(object key, object value)
        {
            this.OnValidate(key, value);
            this.OnInsert(key, value);
            this.InnerHashtable.Add(key, value);
            try
            {
                this.OnInsertComplete(key, value);
            }
            catch
            {
                this.InnerHashtable.Remove(key);
                throw;
            }
        }

        bool IDictionary.Contains(object key)
        {
            return this.InnerHashtable.Contains(key);
        }

        void IDictionary.Remove(object key)
        {
            if (this.InnerHashtable.Contains(key))
            {
                object obj2 = this.InnerHashtable[key];
                this.OnValidate(key, obj2);
                this.OnRemove(key, obj2);
                this.InnerHashtable.Remove(key);
                try
                {
                    this.OnRemoveComplete(key, obj2);
                }
                catch
                {
                    this.InnerHashtable.Add(key, obj2);
                    throw;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.InnerHashtable.GetEnumerator();
        }

        // Properties
        public int Count
        {
            get
            {
                if (this.hashtable != null)
                {
                    return this.hashtable.Count;
                }
                return 0;
            }
        }

        protected IDictionary Dictionary
        {
            get
            {
                return this;
            }
        }

        protected Hashtable InnerHashtable
        {
            get
            {
                if (this.hashtable == null)
                {
                    this.hashtable = new Hashtable();
                }
                return this.hashtable;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return this.InnerHashtable.IsSynchronized;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return this.InnerHashtable.SyncRoot;
            }
        }

        bool IDictionary.IsFixedSize
        {
            get
            {
                return this.InnerHashtable.IsFixedSize;
            }
        }

        bool IDictionary.IsReadOnly
        {
            get
            {
                return this.InnerHashtable.IsReadOnly;
            }
        }

        object IDictionary.this[object key]
        {
            get
            {
                object currentValue = this.InnerHashtable[key];
                this.OnGet(key, currentValue);
                return currentValue;
            }
            set
            {
                this.OnValidate(key, value);
                bool flag = true;
                object oldValue = this.InnerHashtable[key];
                if (oldValue == null)
                {
                    flag = this.InnerHashtable.Contains(key);
                }
                this.OnSet(key, oldValue, value);
                this.InnerHashtable[key] = value;
                try
                {
                    this.OnSetComplete(key, oldValue, value);
                }
                catch
                {
                    if (flag)
                    {
                        this.InnerHashtable[key] = oldValue;
                    }
                    else
                    {
                        this.InnerHashtable.Remove(key);
                    }
                    throw;
                }
            }
        }

        ICollection IDictionary.Keys
        {
            get
            {
                return this.InnerHashtable.Keys;
            }
        }

        ICollection IDictionary.Values
        {
            get
            {
                return this.InnerHashtable.Values;
            }
        }
    }
}
