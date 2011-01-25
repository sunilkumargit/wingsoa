using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace Wing.Mvc.Controls.Base
{
    public abstract class ListControlBase<TConcreteType, TItemType> : ContainerControl<TConcreteType, TItemType>, IListItemContainer
        where TConcreteType : ListControlBase<TConcreteType, TItemType>
        where TItemType : ListItemBase, new()
    {
        public static readonly ControlProperty SelectedValueProperty = ControlProperty.Register("SelectedValue",
            typeof(String),
            typeof(TConcreteType),
            null,
            SelectedValuePropertyChanged);

        public static readonly ControlProperty SelectedItemProperty = ControlProperty.Register("SelectedItem",
            typeof(TItemType),
            typeof(TConcreteType),
            null,
            SelectedItemPropertyChanged);

        public static readonly ControlProperty ItemTemplateProperty = ControlProperty.Register("ItemTemplate",
            typeof(TItemType),
            typeof(TConcreteType));

        public static readonly ControlProperty ItemsSourceProperty = ControlProperty.Register("ItemsSource",
            typeof(Object),
            typeof(TConcreteType));

        public ListControlBase(HtmlTag tag)
            : base(tag)
        {
        }

        public IEnumerable<TItemType> GetSelectedItems()
        {
            return Children.Where(item => item.IsSelected);
        }

        public TItemType SelectedItem
        {
            get { return GetValue<TItemType>(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public TConcreteType SetSelectedItem(TItemType item)
        {
            this.SelectedItem = item;
            return This;
        }

        public TItemType ItemTemplate
        {
            get { return GetValue<TItemType>(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public TConcreteType SetItemTemplate(TItemType item)
        {
            this.ItemTemplate = item;
            return This;
        }

        public TConcreteType SetItemsSource(Object source)
        {
            this.ItemsSource = source;
            return This;
        }

        private static void SelectedItemPropertyChanged(ControlPropertyChangedEventArgs args)
        {
            var target = args.Target as TConcreteType;
            if (target != null)
            {
                var value = args.NewValue as TItemType;
                target.UnselectAll(value);
                if (value != null)
                {
                    if (!target.Children.Contains(value))
                        target.Children.Add(value);
                    value.IsSelected = true;
                }
            }
        }

        public String SelectedValue
        {
            get { return GetValue<String>(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }

        public TConcreteType SetSelectedValue(String value)
        {
            this.SelectedValue = value;
            return This;
        }

        private static void SelectedValuePropertyChanged(ControlPropertyChangedEventArgs args)
        {
            var target = args.Target as TConcreteType;
            if (target != null)
            {
                target.UnselectAll();
                var value = args.NewValue.AsString();
                if (!String.IsNullOrEmpty(value))
                    foreach (var item in target.Children)
                        if (item.Value != null && item.Value.Equals(value))
                            item.IsSelected = true;
            }
        }

        public TItemType NewItem(String text, String value = "", bool selected = false)
        {
            var result = new TItemType()
            {
                Text = text,
                Value = value,
                IsSelected = selected
            };
            Children.Add(result);
            return result;
        }

        public TConcreteType AddItem(String text, String value = "", bool selected = false)
        {
            NewItem(text, value, selected);
            return This;
        }

        public TConcreteType AddItem(String text, bool selected)
        {
            return AddItem(text, text, selected);
        }

        void IListItemContainer.NotifyItemSelectedPropertyChanged(ListItemBase item, bool isSelected)
        {
            if (isSelected)
            {
                UnselectAll(item);
                SetValue(SelectedItemProperty, item);
                SetValue(SelectedValueProperty, item.Value);
            }
        }

        public void UnselectAll(ListItemBase except = null)
        {
            foreach (var child in GetSelectedItems())
                if (child != except)
                    child.IsSelected = false;
        }

        protected override void RenderContent(string innerText, string rawInnerText)
        {
            base.RenderContent("", "");
            if (ItemTemplate != null)
            {
                var dataSource = ItemsSource as IEnumerable;
                if (dataSource != null)
                    RenderItemTemplateItems(dataSource);
                else
                {
                    ItemTemplate.DataContext = ItemsSource;
                    ItemTemplate.Render(CurrentContext);
                }
            }
        }

        public Object ItemsSource
        {
            get { return GetValue<Object>(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        protected abstract void RenderItemTemplateItems(IEnumerable source);
    }
}
