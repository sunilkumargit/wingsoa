using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing
{
    public class WeakReference<T> where T:class
    {
        private WeakReference _weakRef;

        public WeakReference(T target)
        {
            _weakRef = new WeakReference(target);
        }

        public bool IsAlive { get { return _weakRef.IsAlive; } }
        public T Target { get { return _weakRef.Target as T; } }
    }
}
