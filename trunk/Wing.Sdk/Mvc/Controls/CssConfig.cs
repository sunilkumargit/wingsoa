using System;
using System.Collections.Generic;
using System.Linq;


namespace Wing.Mvc.Controls
{
    //[System.Diagnostics.DebuggerStepThrough]
    internal class CssConfig
    {
        private static Dictionary<Type, Dictionary<Object, String>> _cssEnumNames = new Dictionary<Type, Dictionary<Object, String>>();
        private static Dictionary<Type, Dictionary<String, Object>> _cssEnumValues = new Dictionary<Type, Dictionary<String, Object>>();

        static CssConfig()
        {
            RegisterEnum(typeof(CssProperty));
            RegisterEnum(typeof(CssWhiteSpace));
            RegisterEnum(typeof(CssCursor));
            RegisterEnum(typeof(CssTextDecoration));
            RegisterEnum(typeof(CssDisplay));
            RegisterEnum(typeof(CssClear));
            RegisterEnum(typeof(CssFloat));
            RegisterEnum(typeof(CssFontStyle));
            RegisterEnum(typeof(CssFontWeight));
            RegisterEnum(typeof(CssOverflow));
            RegisterEnum(typeof(CssPosition));
        }

        public static void RegisterEnum(Type enumType)
        {
            Dictionary<Object, String> currNames = null;
            if (!_cssEnumNames.TryGetValue(enumType, out currNames))
            {
                currNames = new Dictionary<Object, String>();
                var currValues = new Dictionary<String, Object>();
                var names = ReflectionHelper.GetEnumItemsName(enumType);
                foreach (var item in names)
                {
                    var parts = SplitParts(item);
                    var itemCss = "";
                    foreach (var part in parts)
                        itemCss = itemCss + (itemCss.Length == 0 ? part : "-" + part);
                    var enumItem = ReflectionHelper.GetEnumItemByName(enumType, item);
                    currNames[enumItem] = itemCss.ToLowerInvariant();
                    currValues[itemCss.ToLowerInvariant()] = enumItem;
                }
                _cssEnumNames[enumType] = currNames;
                _cssEnumValues[enumType] = currValues;
            }
            else
                throw new Exception(String.Format("The enum type {0} is already registered.", enumType.FullName));
        }

        private static String[] SplitParts(String item)
        {
            var result = new List<String>();
            var curr = "";
            foreach (var ch in item)
            {
                if (StringHelper.UppercaseCharsArray.Contains(ch) && curr.Length > 0)
                {
                    result.Add(curr);
                    curr = "" + ch;
                }
                else
                    curr += ch;
            }
            if (curr.Length > 0)
                result.Add(curr);
            return result.ToArray();
        }

        public static TEnumType GetEnumItemFromCssName<TEnumType>(String cssName)
        {
            Dictionary<String, Object> values = null;
            if (_cssEnumValues.TryGetValue(typeof(TEnumType), out values))
                return (TEnumType)values[cssName.AsString().ToLowerInvariant()];
            return ReflectionHelper.GetEnumItemByName<TEnumType>(cssName);
        }

        public static string GetItemNameFromEnum(Type enumType, Object enumValue)
        {
            Dictionary<Object, String> names = null;
            if (_cssEnumNames.TryGetValue(enumType, out names))
                return names[enumValue];
            return enumValue.ToString().ToLower();
        }
    }
}