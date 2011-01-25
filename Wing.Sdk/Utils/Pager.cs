using System;

namespace Wing.Utils
{
    public class Pager : IPager
    {
        public int PageCount { get; private set; }
        public int ItemsPerPage { get; private set; }
        public int ItemsCount { get; private set; }

        public Pager(int itemCount, int itemsPerPage)
        {
            ItemsCount = itemCount;
            ItemsPerPage = itemsPerPage;

            //pagecount
            var c = itemCount;

            //no items
            if (c == 0)
            {
                PageCount = 0;
                return;
            }

            //source have items
            PageCount = Convert.ToInt32((double)c / ItemsPerPage);
        }

        public int PageStartAt(int pageIndex)
        {
            if (pageIndex < 1)
                return 0;
            return Math.Min((pageIndex - 1) * ItemsPerPage, ItemsCount);
        }

        public int PageEndAt(int pageIndex)
        {
            return Math.Min(PageStartAt(pageIndex) + ItemsPerPage - 1, ItemsCount - 1);
        }
    }
}
