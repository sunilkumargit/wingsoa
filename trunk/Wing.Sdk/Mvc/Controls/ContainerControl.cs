using System;
using System.Linq;

using Wing.Mvc.Controls.Base;
using System.Collections.Specialized;

namespace Wing.Mvc.Controls
{
    public abstract class ContainerControl<TConcreteType, TChildrenType> : ContainerControlBase
        where TConcreteType : ContainerControl<TConcreteType, TChildrenType>
        where TChildrenType : HtmlObject
    {
        /// <summary>
        /// Controle wrapper onde os filhos deste container serão renderizados.
        /// </summary>
        [TemplatePart]
        public static readonly ControlProperty ChildrenContainerTemplateProperty = ControlProperty.Register("ChildrenContainerTemplate",
            typeof(HtmlObject),
            typeof(TConcreteType),
            null);

        public static readonly ControlProperty ChildrenProperty = ControlProperty.Register("Children",
            typeof(HtmlControlCollection<TChildrenType>),
            typeof(TConcreteType),
            new ControlPropertyMetadata(null, null, null, true));

        private HtmlControlCollection<TChildrenType> _children;

        public ContainerControl(HtmlTag tag) : base(tag) { }

        protected override IHtmlControlCollection GetChildren(bool createIfNotExists = false)
        {
            if (_children == null && createIfNotExists)
            {
                _children = new HtmlControlCollection<TChildrenType>();
                _children.CollectionChanged2 += _children_ListChanged;
                SetReadOnlyValue(ChildrenProperty, _children);
                RegisterChildrenCollection(_children);
            }
            return _children;
        }

        void _children_ListChanged(IHtmlControlCollection collection, NotifyCollectionChangedEventArgs args)
        {
            if (args.NewItems.Count > 0)
            {
                foreach (HtmlControl item in args.NewItems)
                {
                    if (this.IsInitialized && !item.IsInitialized)
                        item.InitInternal(CurrentContext);
                }
            }
        }

        public HtmlObject ChildrenContainerTemplate
        {
            get { return GetValue<HtmlObject>(ChildrenContainerTemplateProperty); }
            set { SetValue(ChildrenContainerTemplateProperty, value); }
        }

        protected TConcreteType This { get { return (TConcreteType)this; } }

        public HtmlControlCollection<TChildrenType> Children
        {
            get { return (HtmlControlCollection<TChildrenType>)GetChildren(true); }
        }

        public TConcreteType AddNew<TControl>(TypedControlDelegate<TControl> controlSetHandler = null) where TControl : TChildrenType, new()
        {
            var control = new TControl();
            if (controlSetHandler != null)
                controlSetHandler.Invoke(control);
            Children.Add(control);
            return This;
        }

        public TConcreteType Add(params TChildrenType[] controls)
        {
            if (controls != null)
            {
                foreach (var control in controls)
                    Children.Add(control);
            }
            return This;
        }

        public TControl New<TControl>() where TControl : TChildrenType, new()
        {
            var control = new TControl();
            Children.Add(control);
            return control;
        }

        public TChildrenType this[String id]
        {
            get { return this.Children.FirstOrDefault(c => c.Id.HasValue() && c.Id.EqualsIgnoreCase(id)); }
        }

        public TChildrenType this[int index]
        {
            get { return this.Children[index]; }
        }

        protected override void RenderChildren(IHtmlControlCollection children)
        {
            if (ChildrenContainerTemplate != null)
                ChildrenContainerTemplate.Render(CurrentContext);
            else
                base.RenderChildren(children);
        }
    }

    public abstract class ContainerControl<TConcreteType> : ContainerControl<TConcreteType, HtmlObject>
        where TConcreteType : ContainerControl<TConcreteType>
    {
        public ContainerControl(HtmlTag tag) : base(tag) { }

        public TConcreteType AddFloatClearFix()
        {
            this.Add(new Html.Span()
                .Css(CssProperty.Display, "block")
                .SetFloatClear(CssClear.Both));
            return This;
        }

        public TConcreteType AddLine(int count = 1)
        {
            for (var i = 0; i < count; i++)
                Add(new BrTag());
            return This;
        }
    }
}
