using System.Collections.Generic;

namespace Wing.Utils
{
    public interface IListPager<TType> : IPager
    {
        IEnumerator<TType> GetPageEnumerator(int pageIndex);
        int PageOfItem(TType item);
        IList<TType> Source { get; }
    }
}
