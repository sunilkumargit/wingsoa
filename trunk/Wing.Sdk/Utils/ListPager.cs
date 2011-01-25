using System;
using System.Collections.Generic;

namespace Wing.Utils
{
    public class ListPager<TType> : IListPager<TType>
    {
        public IList<TType> Source { get; private set; }
        public IPager Pager { get; private set; }

        public ListPager(IList<TType> source, int itemsPerPage)
        {
            Assert.NullArgument(source, "source");
            Assert.EmptyNumber(itemsPerPage, "itemsPerPage");
            Source = source;
            Pager = new Pager(source.Count, itemsPerPage);
        }
        public int PageOfItem(TType item)
        {
            var idx = Source.IndexOf(item);
            if (idx == -1)
                return -1;
            return Convert.ToInt32((double)idx / ItemsPerPage);
        }

        public IEnumerator<TType> GetPageEnumerator(int pageIndex)
        {
            return new PageEnumerator(this, pageIndex);
        }

        private class PageEnumerator : IEnumerator<TType>
        {
            int start;
            int stop;
            int current;
            ListPager<TType> pager;

            public PageEnumerator(ListPager<TType> pager, int pageIndex)
            {
                pageIndex = Math.Min(pageIndex, pager.PageCount);
                this.pager = pager;
                this.start = pager.PageStartAt(pageIndex);
                this.stop = pager.PageEndAt(pageIndex);
                this.current = this.start - 1;
            }

            #region IEnumerator<TType> Members

            public TType Current
            {
                get { return (current < start || current > stop) ? default(TType) : pager.Source[current]; }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                    pager = null;
            }

            #endregion

            #region IEnumerator Members

            object System.Collections.IEnumerator.Current
            {
                get { return this.Current; }
            }

            public bool MoveNext()
            {
                if (current < stop)
                {
                    ++current;
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                current = this.start - 1;
            }

            #endregion
        }

        #region IPager Members

        public int ItemsCount
        {
            get { return Pager.ItemsCount; }
        }

        public int ItemsPerPage
        {
            get { return Pager.ItemsPerPage; }
        }

        public int PageCount
        {
            get { return Pager.PageCount; }
        }

        public int PageEndAt(int pageIndex)
        {
            return Pager.PageEndAt(pageIndex);
        }

        public int PageStartAt(int pageIndex)
        {
            return Pager.PageStartAt(pageIndex);
        }

        #endregion
    }

    public static class PagerListExtensions
    {
        public static ListPager<TType> GetPager<TType>(this IList<TType> list, int itemsPerPage)
        {
            return new ListPager<TType>(list, itemsPerPage);
        }
    }
}
