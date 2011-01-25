using System;
using System.Collections;
using System.Collections.ObjectModel;


namespace Wing.Mvc.Controls
{
    [System.Diagnostics.DebuggerStepThrough]
    public class HtmlControlCollection<TControlType> : ObservableCollection<TControlType>,
        ICollection,
        IHtmlControlCollection where TControlType : HtmlObject
    {
        public HtmlControlCollection()
        {
            this.CollectionChanged += (sender, args) =>
                {
                    if (CollectionChanged2 != null)
                        CollectionChanged2.Invoke(this, args);
                };
        }

        public void RenderChildren(RenderContext ctx)
        {
            foreach (TControlType ctrl in this)
                if (ctrl != null)
                    ctrl.Render(ctx);
        }

        private Object _syncRoot = new Object();
        public Object SyncRoot { get { return _syncRoot; } }

        void ICollection.CopyTo(Array array, int index)
        {
            var top = index + Count;
            for (var i = index; i < top; i++)
                array.SetValue(this[i - index], i);
        }

        int ICollection.Count
        {
            get { return this.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { return this.SyncRoot; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public event ControlCollectionChangedHandler CollectionChanged2;
    }
}
