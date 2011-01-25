using System.Collections.Generic;

namespace Wing.Mvc.Controls
{
    public interface IChildrenCollectionProvider
    {
        bool HasChildren { get; }
        IEnumerable<IHtmlControlCollection> GetChildrenCollections();
    }
}
