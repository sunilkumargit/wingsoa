using System;
using System.Collections.Generic;

namespace Wing.Utils
{
    public class ComparerDelegate<T> : Comparer<T>
    {
        private Func<T, T, int> _comparer;

        public ComparerDelegate(Func<T, T, int> comparerDelegate)
        {
            Assert.NullArgument(comparerDelegate, "comparerDelegate");
            _comparer = comparerDelegate;
        }

        public override int Compare(T x, T y)
        {
            return _comparer(x, y);
        }
    }
}
