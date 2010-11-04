namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;

    public static class ThemeManager
    {
        internal static readonly string DefaultThemeName = "Office_Blue";
        private static readonly List<string> standardThemeNames = new List<string>();
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly ReadOnlyCollection<string> StandardThemeNames;
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly Dictionary<string, Theme> StandardThemes = new Dictionary<string, Theme>();

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static ThemeManager()
        {
            RegisterTheme("Office_Blue", new Office_BlueTheme(), true);
            StandardThemeNames = new ReadOnlyCollection<string>(standardThemeNames);
        }

        public static Theme FromName(string themeName)
        {
            if (themeName == null)
            {
                return null;
            }
            if (StandardThemes.ContainsKey(themeName))
            {
                return StandardThemes[themeName];
            }
            return StandardThemes[DefaultThemeName];
        }

        private static void RegisterTheme(string name, Theme theme, bool isCommon)
        {
            StandardThemes.Add(name, theme);
            if (isCommon)
            {
                standardThemeNames.Add(name);
            }
        }
    }
}

