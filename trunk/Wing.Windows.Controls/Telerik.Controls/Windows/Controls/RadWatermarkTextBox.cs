namespace Telerik.Windows.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Telerik.Windows;

    [TemplatePart(Name="WatermarkVisualElement", Type=typeof(ContentPresenter)), TemplateVisualState(GroupName="WatermarkStates", Name="WatermarkHidden"), TemplateVisualState(GroupName="WatermarkStates", Name="WatermarkVisible")]
    public class RadWatermarkTextBox : TextBox, IThemable
    {
        public static readonly DependencyProperty CurrentTextProperty = DependencyProperty.Register("CurrentText", typeof(string), typeof(RadWatermarkTextBox), null);
        private static readonly DependencyPropertyKey IsWatermarkVisiblePropertyKey = DependencyPropertyExtensions.RegisterReadOnly("IsWatermarkVisible", typeof(bool), typeof(RadWatermarkTextBox), new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(RadWatermarkTextBox.OnIsWatermarkVisibleChanged)));
        public static readonly DependencyProperty IsWatermarkVisibleProperty = IsWatermarkVisiblePropertyKey.DependencyProperty;
        public static readonly DependencyProperty SelectionOnFocusProperty = DependencyProperty.Register("SelectionOnFocus", typeof(Telerik.Windows.Controls.SelectionOnFocus), typeof(RadWatermarkTextBox), null);
        public static readonly DependencyProperty WatermarkContentProperty = DependencyProperty.Register("WatermarkContent", typeof(object), typeof(RadWatermarkTextBox), null);
        private const string WATERMARKHIDDENSTATE = "WatermarkHidden";
        private const string WATERMARKSTATES = "WatermarkStates";
        public static readonly DependencyProperty WatermarkTemplateProperty = DependencyProperty.Register("WatermarkTemplate", typeof(DataTemplate), typeof(RadWatermarkTextBox), null);
        private const string WATERMARKVISIBLESTATE = "WatermarkVisible";
        private const string WATERMARKVISUALELEMENT = "WatermarkVisualElement";

        public RadWatermarkTextBox()
        {
            base.DefaultStyleKey = typeof(RadWatermarkTextBox);
            base.GotFocus += new RoutedEventHandler(this.OnGotFocus);
            base.LostFocus += new RoutedEventHandler(this.OnLostFocus);
            base.TextChanged += new TextChangedEventHandler(this.OnTextChanged);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.IsWatermarkVisible = string.IsNullOrEmpty(base.Text);
            this.UpdateVisualState(false);
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (this.IsWatermarkVisible)
            {
                this.IsWatermarkVisible = false;
            }
            this.UpdateSelection();
        }

        private static void OnIsWatermarkVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadWatermarkTextBox box = d as RadWatermarkTextBox;
            if (box != null)
            {
                box.UpdateVisualState(true);
            }
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (!this.IsWatermarkVisible && string.IsNullOrEmpty(base.Text))
            {
                this.IsWatermarkVisible = true;
            }
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            this.CurrentText = base.Text;
            this.IsWatermarkVisible = !this.IsFocused && string.IsNullOrEmpty(base.Text);
        }

        void IThemable.ResetTheme()
        {
        }

        private void UpdateSelection()
        {
            switch (this.SelectionOnFocus)
            {
                case Telerik.Windows.Controls.SelectionOnFocus.Unchanged:
                    break;

                case Telerik.Windows.Controls.SelectionOnFocus.SelectAll:
                    base.SelectAll();
                    break;

                case Telerik.Windows.Controls.SelectionOnFocus.CaretToBeginning:
                    base.SelectionStart = 0;
                    return;

                case Telerik.Windows.Controls.SelectionOnFocus.CaretToEnd:
                    base.SelectionStart = base.Text.Length;
                    return;

                default:
                    return;
            }
        }

        private void UpdateVisualState(bool p)
        {
            if (this.IsWatermarkVisible)
            {
                VisualStateManager.GoToState(this, "WatermarkVisible", p);
            }
            else
            {
                VisualStateManager.GoToState(this, "WatermarkHidden", p);
            }
        }

        public string CurrentText
        {
            get
            {
                return (string) base.GetValue(CurrentTextProperty);
            }
            set
            {
                base.SetValue(CurrentTextProperty, value);
            }
        }

        internal bool IsFocused
        {
            get
            {
                return (FocusManager.GetFocusedElement() == this);
            }
        }

        public bool IsWatermarkVisible
        {
            get
            {
                return (bool) base.GetValue(IsWatermarkVisibleProperty);
            }
            private set
            {
                this.SetValue(IsWatermarkVisiblePropertyKey, value);
            }
        }

        public Telerik.Windows.Controls.SelectionOnFocus SelectionOnFocus
        {
            get
            {
                return (Telerik.Windows.Controls.SelectionOnFocus) base.GetValue(SelectionOnFocusProperty);
            }
            set
            {
                base.SetValue(SelectionOnFocusProperty, value);
            }
        }

        public object WatermarkContent
        {
            get
            {
                return base.GetValue(WatermarkContentProperty);
            }
            set
            {
                base.SetValue(WatermarkContentProperty, value);
            }
        }

        public DataTemplate WatermarkTemplate
        {
            get
            {
                return (DataTemplate) base.GetValue(WatermarkTemplateProperty);
            }
            set
            {
                base.SetValue(WatermarkTemplateProperty, value);
            }
        }
    }
}

