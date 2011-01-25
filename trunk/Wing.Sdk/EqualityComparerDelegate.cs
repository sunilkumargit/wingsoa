using System;
using System.Collections.Generic;

namespace Wing
{
    public delegate int HashCodeDelegate<T>(T o);

    public class EqualityComparerDelegate<T> : IEqualityComparer<T>
    {
        private Func<T, T, bool> _del;
        private HashCodeDelegate<T> _hashDel;

        public EqualityComparerDelegate(Func<T, T, bool> del, HashCodeDelegate<T> hashDel)
        {
            _del = del;
            _hashDel = hashDel;
        }

        public bool Equals(T x, T y)
        {
            if (_del != null)
                return _del.Invoke(x, y);
            return x.GetHashCode() == y.GetHashCode();
        }

        public int GetHashCode(T obj)
        {
            if (_hashDel != null)
                return _hashDel.Invoke(obj);
            return obj.GetHashCode();
        }
    }
}
