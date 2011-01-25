using System.Collections;
using System.Collections.Specialized;


namespace Wing.Mvc.Controls
{
    public interface IHtmlControlCollection : IEnumerable, ICollection
    {
        void RenderChildren(RenderContext ctx);
        event ControlCollectionChangedHandler CollectionChanged2;
    }

    public delegate void ControlCollectionChangedHandler(IHtmlControlCollection collection, NotifyCollectionChangedEventArgs args);
}
