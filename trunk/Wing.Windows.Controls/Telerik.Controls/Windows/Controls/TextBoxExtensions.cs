namespace Telerik.Windows.Controls
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows.Controls;

    internal static class TextBoxExtensions
    {
        public static void SelectAll(this TextBox textBox)
        {
            textBox.Select(0, textBox.Text.Length);
        }
    }
}

