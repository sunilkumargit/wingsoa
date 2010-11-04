namespace Telerik.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Input;
    using Telerik.Windows;
    using Telerik.Windows.Controls.Design;

    [TypeConverter(typeof(KeyGestureConverter))]
    public class KeyGesture : InputGesture
    {
        private static TypeConverter keyGestureConverter = new KeyGestureConverter();
        private const string MultipleGestureDelimiter = ";";

        public KeyGesture(System.Windows.Input.Key key) : this(key, ModifierKeys.None)
        {
        }

        public KeyGesture(System.Windows.Input.Key key, ModifierKeys modifiers) : this(key, modifiers, string.Empty, true)
        {
        }

        internal KeyGesture(System.Windows.Input.Key key, ModifierKeys modifiers, bool validateGesture) : this(key, modifiers, string.Empty, validateGesture)
        {
        }

        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId="string")]
        public KeyGesture(System.Windows.Input.Key key, ModifierKeys modifiers, string displayString) : this(key, modifiers, displayString, true)
        {
        }

        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        private KeyGesture(System.Windows.Input.Key key, ModifierKeys modifiers, string displayString, bool validateGesture)
        {
            displayString.TestNotNull("displayString");
            if (validateGesture && !IsValid(key, modifiers))
            {
                throw new NotSupportedException("KeyGesture_Invalid");
            }
            this.Modifiers = modifiers;
            this.Key = key;
            this.DisplayString = displayString;
        }

        internal static void AddGesturesFromResourceStrings(string keyGestures, string displayStrings, InputGestureCollection gestures)
        {
            while (!string.IsNullOrEmpty(keyGestures))
            {
                string str;
                string str2;
                int index = keyGestures.IndexOf(";", StringComparison.Ordinal);
                if (index >= 0)
                {
                    str2 = keyGestures.Substring(0, index);
                    keyGestures = keyGestures.Substring(index + 1);
                }
                else
                {
                    str2 = keyGestures;
                    keyGestures = string.Empty;
                }
                index = displayStrings.IndexOf(";", StringComparison.Ordinal);
                if (index >= 0)
                {
                    str = displayStrings.Substring(0, index);
                    displayStrings = displayStrings.Substring(index + 1);
                }
                else
                {
                    str = displayStrings;
                    displayStrings = string.Empty;
                }
                KeyGesture inputGesture = CreateFromResourceStrings(str2, str);
                if (inputGesture != null)
                {
                    gestures.Add(inputGesture);
                }
            }
        }

        internal static KeyGesture CreateFromResourceStrings(string keyGestureToken, string keyDisplayString)
        {
            if (!string.IsNullOrEmpty(keyDisplayString))
            {
                keyGestureToken = keyGestureToken + ',' + keyDisplayString;
            }
            return (keyGestureConverter.ConvertFromString(keyGestureToken) as KeyGesture);
        }

        public string GetDisplayStringForCulture(CultureInfo culture)
        {
            if (!string.IsNullOrEmpty(this.DisplayString))
            {
                return this.DisplayString;
            }
            return (string) keyGestureConverter.ConvertTo(null, culture, this, typeof(string));
        }

        internal static bool IsDefinedKey(System.Windows.Input.Key key)
        {
            return ((key >= System.Windows.Input.Key.None) && (key <= System.Windows.Input.Key.Divide));
        }

        internal static bool IsValid(System.Windows.Input.Key key, ModifierKeys modifiers)
        {
            if (((key < System.Windows.Input.Key.F1) || (key > System.Windows.Input.Key.F12)) && ((key < System.Windows.Input.Key.NumPad0) || (key > System.Windows.Input.Key.Divide)))
            {
                if ((modifiers & (ModifierKeys.Windows | ModifierKeys.Control | ModifierKeys.Alt)) != ModifierKeys.None)
                {
                    switch (key)
                    {
                        case System.Windows.Input.Key.Ctrl:
                        case System.Windows.Input.Key.Alt:
                            return false;
                    }
                    return true;
                }
                if (((key >= System.Windows.Input.Key.D0) && (key <= System.Windows.Input.Key.D9)) || ((key >= System.Windows.Input.Key.A) && (key <= System.Windows.Input.Key.Z)))
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Matches(object targetElement, RoutedEventArgs inputEventArgs)
        {
            KeyEventArgs args = inputEventArgs as KeyEventArgs;
            if (args != null)
            {
                return this.Matches(args.Key, System.Windows.Input.Keyboard.Modifiers);
            }
            TestKeyEventArgs testArgs = inputEventArgs as TestKeyEventArgs;
            return ((testArgs != null) && this.Matches(testArgs.Key, testArgs.Modifiers));
        }

        private bool Matches(System.Windows.Input.Key key, ModifierKeys modifiers)
        {
            if (!IsDefinedKey(key))
            {
                return false;
            }
            return ((this.Key == key) && (this.Modifiers == modifiers));
        }

        public string DisplayString { get; private set; }

        public System.Windows.Input.Key Key { get; private set; }

        public ModifierKeys Modifiers { get; private set; }
    }
}

