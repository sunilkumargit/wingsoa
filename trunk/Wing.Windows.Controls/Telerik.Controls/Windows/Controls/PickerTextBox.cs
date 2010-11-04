namespace Telerik.Windows.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    public class PickerTextBox : TextBox, IThemable
    {
        private static readonly DependencyProperty HorizontalContentAlignmentHelperProperty = DependencyProperty.Register("HorizontalContentAlignmentHelper", typeof(HorizontalAlignment), typeof(PickerTextBox), new PropertyMetadata(HorizontalAlignment.Center, new PropertyChangedCallback(PickerTextBox.OnHorizontalContentAlignmentHelperChanged)));

        public PickerTextBox()
        {
            base.DefaultStyleKey = typeof(PickerTextBox);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            base.SetBinding(HorizontalContentAlignmentHelperProperty, new Binding("HorizontalContentAlignment") { Source = this });
        }

        private static void OnHorizontalContentAlignmentHelperChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PickerTextBox pickerTextBox = d as PickerTextBox;
            switch (((HorizontalAlignment) e.NewValue))
            {
                case HorizontalAlignment.Left:
                case HorizontalAlignment.Stretch:
                    pickerTextBox.TextAlignment = TextAlignment.Left;
                    return;

                case HorizontalAlignment.Center:
                    pickerTextBox.TextAlignment = TextAlignment.Center;
                    return;

                case HorizontalAlignment.Right:
                    pickerTextBox.TextAlignment = TextAlignment.Right;
                    return;
            }
        }

        public void ResetTheme()
        {
        }
    }
}

