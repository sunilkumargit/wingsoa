namespace Telerik.Windows.Controls
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Data;

    public class HierarchicalDataTemplate : DataTemplate
    {
        private Style itemContainerStyle;
        private StyleSelector itemContainerStyleSelector;
        private DataTemplate itemTemplate;
        private DataTemplateSelector itemTemplateSelector;

        internal bool IsItemContainerStyleSelectorSet { get; private set; }

        internal bool IsItemContainerStyleSet { get; private set; }

        internal bool IsItemTemplateSelectorSet { get; private set; }

        internal bool IsItemTemplateSet { get; private set; }

        public Style ItemContainerStyle
        {
            get
            {
                return this.itemContainerStyle;
            }
            set
            {
                this.itemContainerStyle = value;
                this.IsItemContainerStyleSet = true;
            }
        }

        public StyleSelector ItemContainerStyleSelector
        {
            get
            {
                return this.itemContainerStyleSelector;
            }
            set
            {
                this.itemContainerStyleSelector = value;
                this.IsItemContainerStyleSelectorSet = true;
            }
        }

        public Binding ItemsSource { get; set; }

        public DataTemplate ItemTemplate
        {
            get
            {
                return this.itemTemplate;
            }
            set
            {
                this.itemTemplate = value;
                this.IsItemTemplateSet = true;
            }
        }

        public DataTemplateSelector ItemTemplateSelector
        {
            get
            {
                return this.itemTemplateSelector;
            }
            set
            {
                this.itemTemplateSelector = value;
                this.IsItemTemplateSelectorSet = true;
            }
        }
    }
}

