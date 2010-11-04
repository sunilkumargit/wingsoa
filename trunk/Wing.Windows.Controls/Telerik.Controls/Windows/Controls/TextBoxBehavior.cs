namespace Telerik.Windows.Controls
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;

    public static class TextBoxBehavior
    {
        public static readonly DependencyProperty SelectAllOnGotFocusProperty = DependencyProperty.RegisterAttached("SelectAllOnGotFocus", typeof(bool), typeof(TextBoxBehavior), new PropertyMetadata(false, new PropertyChangedCallback(TextBoxBehavior.OnSelectAllOnGotFocusChanged)));
        public static readonly DependencyProperty SelectAllOnTripleClickProperty = DependencyProperty.RegisterAttached("SelectAllOnTripleClick", typeof(bool), typeof(TextBoxBehavior), new PropertyMetadata(false, new PropertyChangedCallback(TextBoxBehavior.OnSelectAllOnTripleClickChanged)));
        internal static readonly DependencyProperty TripleClickInstanceProperty = DependencyProperty.RegisterAttached("TripleClickInstance", typeof(TripleClickBehavior), typeof(TextBoxBehavior), new PropertyMetadata(null));
        public static readonly DependencyProperty UpdateTextOnEnterProperty = DependencyProperty.RegisterAttached("UpdateTextOnEnter", typeof(bool), typeof(TextBoxBehavior), new PropertyMetadata(false, new PropertyChangedCallback(TextBoxBehavior.OnUpdateTextOnEnterChanged)));

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static bool GetSelectAllOnGotFocus(TextBox textBox)
        {
            return (bool) textBox.GetValue(SelectAllOnGotFocusProperty);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static bool GetSelectAllOnTripleClick(TextBox textBox)
        {
            return (bool) textBox.GetValue(SelectAllOnTripleClickProperty);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static bool GetUpdateTextOnEnter(TextBox textBox)
        {
            return (bool) textBox.GetValue(UpdateTextOnEnterProperty);
        }

        private static void OnSelectAllOnGotFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBox textBox = d as TextBox;
            if ((textBox != null) && (e.NewValue is bool))
            {
                if ((bool) e.NewValue)
                {
                    textBox.GotFocus += new RoutedEventHandler(TextBoxBehavior.OnTextBoxGotFocus);
                }
                else
                {
                    textBox.GotFocus -= new RoutedEventHandler(TextBoxBehavior.OnTextBoxGotFocus);
                }
            }
        }

        private static void OnSelectAllOnTripleClickChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBox textBox = d as TextBox;
            if ((textBox != null) && (e.NewValue is bool))
            {
                if ((bool) e.NewValue)
                {
                    TripleClickBehavior tripleClickBehavior = new TripleClickBehavior(textBox);
                    textBox.SetValue(TripleClickInstanceProperty, tripleClickBehavior);
                    tripleClickBehavior.TripleClick += new EventHandler(TextBoxBehavior.tripleClickBehavior_TripleClick);
                }
                else
                {
                    TripleClickBehavior tripleClickBehavior = textBox.GetValue(TripleClickInstanceProperty) as TripleClickBehavior;
                    if (tripleClickBehavior != null)
                    {
                        tripleClickBehavior.TripleClick -= new EventHandler(TextBoxBehavior.tripleClickBehavior_TripleClick);
                        tripleClickBehavior.Dispose();
                    }
                    textBox.SetValue(TripleClickInstanceProperty, null);
                }
            }
        }

        private static void OnTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            ((TextBox) sender).SelectAll();
        }

        private static void OnTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BindingExpression expression = ((TextBox) sender).GetBindingExpression(TextBox.TextProperty);
                if (expression != null)
                {
                    expression.UpdateSource();
                }
            }
        }

        private static void OnUpdateTextOnEnterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBox textBox = d as TextBox;
            if ((textBox != null) && (e.NewValue is bool))
            {
                if ((bool) e.NewValue)
                {
                    textBox.KeyDown += new KeyEventHandler(TextBoxBehavior.OnTextBoxKeyDown);
                }
                else
                {
                    textBox.KeyDown -= new KeyEventHandler(TextBoxBehavior.OnTextBoxKeyDown);
                }
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static void SetSelectAllOnGotFocus(TextBox textBox, bool value)
        {
            textBox.SetValue(SelectAllOnGotFocusProperty, value);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static void SetSelectAllOnTripleClick(TextBox textBox, bool value)
        {
            textBox.SetValue(SelectAllOnTripleClickProperty, value);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static void SetUpdateTextOnEnter(TextBox textBox, bool value)
        {
            textBox.SetValue(UpdateTextOnEnterProperty, value);
        }

        private static void tripleClickBehavior_TripleClick(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                textBox.SelectAll();
            }
        }
    }
}

