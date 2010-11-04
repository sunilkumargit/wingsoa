namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Controls;

    public static class TextSearch
    {
        public static readonly DependencyProperty TextPathProperty = DependencyProperty.RegisterAttached("TextPath", typeof(string), typeof(TextSearch), null);
        public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached("Text", typeof(string), typeof(TextSearch), null);

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static TextSearch()
        {
            AutoCompleteTimeout = TimeSpan.FromMilliseconds(800.0);
        }

        internal static string ConvertToPlainText(object o)
        {
            FrameworkElement element = o as FrameworkElement;
            if (element != null)
            {
                string plainText = element.GetPlainText();
                if (plainText != null)
                {
                    return plainText;
                }
            }
            if (o == null)
            {
                return string.Empty;
            }
            return o.ToString();
        }

        public static Func<string, bool> CreateFullMatchFunc(string text, TextSearchMode mode)
        {
            StringComparison stringComparison = mode.IsCaseSensitive() ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            return itemText => itemText.Equals(text, stringComparison);
        }

        public static Func<string, bool> CreatePartialMatchFunc(string text, TextSearchMode mode)
        {
            if (mode.IsContains())
            {
                if (mode.IsCaseSensitive())
                {
                    return itemText => itemText.Contains(text);
                }
                return itemText => (itemText.IndexOf(text, StringComparison.OrdinalIgnoreCase) > -1);
            }
            if (mode.IsStartsWith())
            {
                StringComparison stringComparison = mode.IsCaseSensitive() ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
                return itemText => itemText.StartsWith(text, stringComparison);
            }
            return null;
        }

        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId="4#")]
        internal static int FindMatchingItems(Telerik.Windows.Controls.ItemsControl itemsControl, string text, TextSearchMode mode, int startIndex, out List<int> matchIndexes)
        {
            Func<string, bool> isFullMatch;
            Func<string, bool> isPartialMatch;
            matchIndexes = new List<int>();
            ItemCollection items = itemsControl.Items;
            if (items.Count == 0)
            {
                return -1;
            }
            if (string.IsNullOrEmpty(text))
            {
                isPartialMatch = isFullMatch = itemText => false;
            }
            else
            {
                isFullMatch = CreateFullMatchFunc(text, mode);
                isPartialMatch = CreatePartialMatchFunc(text, mode) ?? isFullMatch;
            }
            Func<object, object> getTextFunc = null;
            int fullMatchIndex = -1;
            for (int i = startIndex; i < items.Count; i++)
            {
                object item = items[i];
                if ((getTextFunc == null) && (item != null))
                {
                    getTextFunc = BindingExpressionHelper.CreateGetValueFunc(item.GetType(), GetPrimaryTextPath(itemsControl));
                }
                Control container = itemsControl.ItemContainerGenerator.ContainerFromIndex(i) as Control;
                if ((container == null) || container.IsEnabled)
                {
                    string primaryText = GetPrimaryText(item, getTextFunc);
                    if ((fullMatchIndex < 0) && isFullMatch(primaryText))
                    {
                        fullMatchIndex = i;
                        matchIndexes.Insert(0, fullMatchIndex);
                    }
                    else if (isPartialMatch(primaryText))
                    {
                        matchIndexes.Add(i);
                    }
                }
            }
            return fullMatchIndex;
        }

        private static CultureInfo GetCulture(DependencyObject element)
        {
            object obj2 = element.GetValue(FrameworkElement.LanguageProperty);
            return null;
        }

        private static string GetPrimaryText(FrameworkElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            string text = GetText(element);
            if (!string.IsNullOrEmpty(text))
            {
                return text;
            }
            return element.GetPlainText();
        }

        internal static string GetPrimaryText(object item, Func<object, object> getTextFunc)
        {
            DependencyObject d = item as DependencyObject;
            if (d != null)
            {
                string str = (string) d.GetValue(TextProperty);
                if (!string.IsNullOrEmpty(str))
                {
                    return str;
                }
            }
            if (getTextFunc != null)
            {
                return ConvertToPlainText(getTextFunc(item));
            }
            return ConvertToPlainText(item);
        }

        internal static string GetPrimaryText(object item, string primaryTextPath)
        {
            if (item == null)
            {
                return null;
            }
            return GetPrimaryText(item, BindingExpressionHelper.CreateGetValueFunc(item.GetType(), primaryTextPath));
        }

        internal static string GetPrimaryTextFromItem(Telerik.Windows.Controls.ItemsControl itemsControl, object item)
        {
            return GetPrimaryText(item, GetPrimaryTextPath(itemsControl));
        }

        internal static string GetPrimaryTextPath(Telerik.Windows.Controls.ItemsControl itemsControl)
        {
            string textPath = GetTextPath(itemsControl);
            if (string.IsNullOrEmpty(textPath))
            {
                textPath = itemsControl.DisplayMemberPath;
            }
            return textPath;
        }

        public static string GetText(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (string) element.GetValue(TextProperty);
        }

        public static string GetTextPath(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (string) element.GetValue(TextPathProperty);
        }

        public static void SetText(DependencyObject element, string value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(TextProperty, value);
        }

        public static void SetTextPath(DependencyObject element, string value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(TextPathProperty, value);
        }

        public static TimeSpan AutoCompleteTimeout{get;set;}
    }
}

