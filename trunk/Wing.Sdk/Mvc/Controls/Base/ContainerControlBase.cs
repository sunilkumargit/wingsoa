using System;
using System.Collections.Generic;
using System.Linq;


namespace Wing.Mvc.Controls.Base
{
    /// <summary>
    /// Classe base para controles que podem conter outros controles, ditos Containeres.
    /// </summary>
    public abstract class ContainerControlBase : HtmlControl, IChildrenCollectionProvider
    {
        /// <summary>
        /// Alinhamento vertical do conteúdo.
        /// </summary>
        public static readonly ControlProperty ContentVerticalAlignProperty = ControlProperty.Register("ContentVerticalAlign",
            typeof(VerticalAlignment),
            typeof(ContainerControlBase),
            new CssEnumPropertyApplier(CssProperty.VerticalAlign, VerticalAlignment.Top),
            VerticalAlignment.Top);

        private HtmlControlCollection<HtmlObject> _beforeChildrenGuestControls;
        private HtmlControlCollection<HtmlObject> _afterChildrenGuestControls;
        private List<IHtmlControlCollection> _childrenCollections;

        [System.Diagnostics.DebuggerStepThrough]
        public ContainerControlBase(HtmlTag tag)
            : base(tag)
        {
        }

        [System.Diagnostics.DebuggerStepThrough]
        protected void RegisterChildrenCollection(params IHtmlControlCollection[] childrenCollection)
        {
            if (_childrenCollections == null)
                _childrenCollections = new List<IHtmlControlCollection>();
            _childrenCollections.AddRange(childrenCollection);
        }

        public VerticalAlignment ContentVerticalAlign
        {
            get { return GetValue<VerticalAlignment>(ContentVerticalAlignProperty); }
            set { SetValue(ContentVerticalAlignProperty, value); }
        }

        protected override void RenderContent(String innerText, String rawInnerText)
        {
            base.RenderContent(innerText, rawInnerText);

            if (_beforeChildrenGuestControls != null)
                _beforeChildrenGuestControls.RenderChildren(CurrentContext);

            var _children = GetChildren();
            if (_children != null && _children.Count > 0)
                RenderChildren(_children);

            if (this._afterChildrenGuestControls != null)
                _afterChildrenGuestControls.RenderChildren(CurrentContext);
        }

        public override void ApplyTemplate(HtmlObject source)
        {
            base.ApplyTemplate(source);
            if (_childrenCollections != null)
                foreach (var c1 in _childrenCollections)
                    foreach (var c2 in c1)
                        ((HtmlObject)c2).ApplyTemplate(source);
        }

        internal void _AddGuestControl(HtmlObject control, bool beforeChildren)
        {
            Assert.NullArgument(control, "control");
            if (beforeChildren)
            {
                _beforeChildrenGuestControls = _beforeChildrenGuestControls ?? new HtmlControlCollection<HtmlObject>();
                _beforeChildrenGuestControls.Add(control);
            }
            else
            {
                _afterChildrenGuestControls = _afterChildrenGuestControls ?? new HtmlControlCollection<HtmlObject>();
                _afterChildrenGuestControls.Add(control);
            }
        }

        internal void _RemoveGuestControl(HtmlObject control)
        {
            if (_beforeChildrenGuestControls != null)
                _beforeChildrenGuestControls.Remove(control);
            if (_afterChildrenGuestControls != null)
                _afterChildrenGuestControls.Remove(control);
        }

        protected virtual void RenderChildren(IHtmlControlCollection children)
        {
            children.RenderChildren(this.CurrentContext);
        }

        public int Count { get { return GetChildren() == null ? 0 : GetChildren().Count; } }

        protected abstract IHtmlControlCollection GetChildren(bool createIfNotExists = false);

        [System.Diagnostics.DebuggerStepThrough]
        IEnumerable<IHtmlControlCollection> IChildrenCollectionProvider.GetChildrenCollections()
        {
            return _childrenCollections;
        }

        bool IChildrenCollectionProvider.HasChildren
        {
            get
            {
                return _childrenCollections != null
                    && _childrenCollections.Any(c => c.Count > 0);
            }
        }
    }
}
