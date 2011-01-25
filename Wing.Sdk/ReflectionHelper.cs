using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Wing
{
    [System.Diagnostics.DebuggerStepThrough]
    public static class ReflectionHelper
    {
        public static TAttributeType GetAttribute<TAttributeType>(MemberInfo info, bool inheritAttr) where TAttributeType : Attribute
        {
            TAttributeType[] attrs = GetAttributes<TAttributeType>(info, inheritAttr);
            if (attrs != null)
                return attrs[0];
            return null;
        }

        public static TAttributeType[] GetAttributes<TAttributeType>(MemberInfo info, bool inheritAttr) where TAttributeType : Attribute
        {
            if (info == null) return null;
            Object[] attrs = info.GetCustomAttributes(typeof(TAttributeType), true);
            if ((attrs == null) || (attrs.Length == 0))
                return null;
            List<TAttributeType> result = new List<TAttributeType>();
            foreach (TAttributeType attr in attrs)
                if (inheritAttr || (attr.GetType().Equals(typeof(TAttributeType))))
                    result.Add(attr);
            if (result.Count > 0)
                return result.ToArray();
            return null;
        }

        public static TAttributeType[] GetAttributes<TAttributeType>(MemberInfo info) where TAttributeType : Attribute
        {
            return GetAttributes<TAttributeType>(info, false);
        }

        public static TAttributeType GetAttribute<TAttributeType>(MemberInfo info) where TAttributeType : Attribute
        {
            return GetAttribute<TAttributeType>(info, false);
        }

        public static TAttributeType GetAttribute<TAttributeType, TType>() where TAttributeType : Attribute
        {
            return GetAttribute<TAttributeType>(typeof(TType));
        }

        public static bool MemberHasAttribute<TAttributeType>(MemberInfo info) where TAttributeType : Attribute
        {
            return MemberHasAttribute<TAttributeType>(info, false);
        }

        public static bool MemberHasAttribute<TAttributeType>(MemberInfo info, bool inheritAttr) where TAttributeType : Attribute
        {
            return GetAttribute<TAttributeType>(info, inheritAttr) != null;
        }

        public static bool MemberHasAttribute<TAttributeType, TType>() where TAttributeType : Attribute
        {
            return MemberHasAttribute<TAttributeType>(typeof(TType));
        }

        public static List<Type> FilterByAncestor(IEnumerable<Type> types, Type ancestorType)
        {
            List<Type> result = new List<Type>();
            foreach (Type type in types)
                if (ancestorType.IsAssignableFrom(type))
                    result.Add(type);
            return result;
        }

        public static List<Type> FilterByAncestor<TAncestorType>(IEnumerable<Type> types)
        {
            return FilterByAncestor(types, typeof(TAncestorType));
        }

        public static PropertyInfo[] GetPropertiesWithAttribute<TAttributeType>(Type type, bool inheritAttr) where TAttributeType : Attribute
        {
            List<PropertyInfo> result = new List<PropertyInfo>();
            PropertyInfo[] props = type.GetProperties();
            foreach (PropertyInfo p in props)
                if (MemberHasAttribute<TAttributeType>(p, inheritAttr))
                    result.Add(p);
            return result.ToArray();
        }

        public static PropertyInfo[] GetPropertiesWithAttribute<TAttributeType>(Type type) where TAttributeType : Attribute
        {
            return GetPropertiesWithAttribute<TAttributeType>(type, false);
        }

        public static PropertyInfo[] GetPropertiesWithAttribute<TAttributeType, TType>() where TAttributeType : Attribute
        {
            return GetPropertiesWithAttribute<TAttributeType>(typeof(TType));
        }

        public static PropertyInfo[] GetPropertiesWithAttribute<TAttributeType, TType>(bool inheritAttr) where TAttributeType : Attribute
        {
            return GetPropertiesWithAttribute<TAttributeType>(typeof(TType), inheritAttr);
        }

        public static Boolean TypeHasProperty(Type type, String propertyName)
        {
            return type.GetProperty(propertyName) != null;
        }

        public static Boolean TypeHasProperty<TType>(String propertyName)
        {
            return TypeHasProperty(typeof(TType), propertyName);
        }

        public static TResultType ReadStaticProperty<TResultType>(Type type, String propertyName)
        {
            return ConversionHelper.Coerce<TResultType>(ReadStaticProperty(type, propertyName));
        }

        public static Object ReadStaticProperty(Type type, String propertyName)
        {
            return ReflectionHelper.InvokeMember(type, propertyName, null,
                BindingFlags.Static
                | BindingFlags.GetProperty
                | BindingFlags.Public
                | BindingFlags.NonPublic
                | BindingFlags.FlattenHierarchy);
        }

        public static Object ReadProperty(Object instance, String propertyName)
        {
            return ReflectionHelper.InvokeMember(instance.GetType(), propertyName, instance,
                BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance);
        }

        public static TResultType ReadProperty<TResultType>(Object instance, String propertyName)
        {
            return ConversionHelper.Coerce<TResultType>(ReadProperty(instance, propertyName));
        }

        public static Object InvokeMember(Type type, String memberName, Object target, BindingFlags flags, params object[] args)
        {
            return type.InvokeMember(memberName, flags, null, target, args);
        }

        public static void WriteProperty(Object instance, String propertyName, Object value)
        {
            PropertyInfo prop = instance.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.NonPublic);
            if (prop != null)
                prop.SetValue(instance, value, null);
        }

        public static void WritePropertyCoerce(Object instance, String propertyName, Object value)
        {
            PropertyInfo prop = instance.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.NonPublic);
            WriteProperty(instance, propertyName, ConversionHelper.Coerce(value, prop.PropertyType));
        }

        public static TResultType InvokeInstanceMethod<TResultType>(Object instance, String methodName, params object[] args)
        {
            return (TResultType)ReflectionHelper.InvokeMember(instance.GetType(), methodName, instance,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod, args);
        }

        public static Object InvokeDefaultConstructor(Type type)
        {
            return Activator.CreateInstance(type);
        }

        public static TType InvokeDefaultConstructor<TType>()
        {
            return Activator.CreateInstance<TType>();
        }

        public static Object InvokeInstanceMethod(Object instance, String methodName, params object[] args)
        {
            return InvokeInstanceMethod<Object>(instance, methodName, args);
        }

        public static TTYPE FirstEnumItem<TTYPE>()
        {
            return (TTYPE)FirstEnumItem(typeof(TTYPE));
        }

        private static System.Collections.Hashtable _cachedEnumItems = new System.Collections.Hashtable();
        public static Object FirstEnumItem(Type type)
        {
            Assert.NullArgument(type, "type");
            Object result = _cachedEnumItems[type];
            if (result == null)
            {
                if (!type.IsEnum)
                    throw new ArgumentException("Type " + type.FullName + " is not a Enum type");
                FieldInfo[] members = type.GetFields(BindingFlags.Public | BindingFlags.Static);
                if (members.Length > 0)
                {
                    result = InvokeMember(type, members[0].Name, null, BindingFlags.Public | BindingFlags.Static | BindingFlags.GetField);
                    _cachedEnumItems[type] = result;
                    return result;

                }
                return 0;
            }
            return result;
        }

        public static String[] GetEnumItemsName(Type type)
        {
            Assert.NullArgument(type, "type");
            var items = new List<String>();
            if (!type.IsEnum)
                throw new ArgumentException("Type " + type.FullName + " is not a Enum type");
            FieldInfo[] members = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (var member in members)
                items.Add(member.Name);
            return items.ToArray();
        }

        public static Object GetEnumItemByName(Type type, String name, bool ignoreCase)
        {
            Assert.NullArgument(type, "type");
            Assert.NullArgument(name, "name");
            if (!type.IsEnum)
                throw new ArgumentException("Type " + type.FullName + " is not a Enum");
            BindingFlags flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.GetField;
            if (ignoreCase)
                flags = flags | BindingFlags.IgnoreCase;
            FieldInfo member = type.GetField(name, flags);
            if (member != null)
                return InvokeMember(type, member.Name, null, BindingFlags.Public | BindingFlags.Static | BindingFlags.GetField);
            return 0;
        }

        public static Object GetEnumItemByName(Type type, String name)
        {
            return GetEnumItemByName(type, name, false);
        }

        public static TEnumType GetEnumItemByName<TEnumType>(String name, bool ignoreCase)
        {
            return (TEnumType)GetEnumItemByName(typeof(TEnumType), name, ignoreCase);
        }

        public static TEnumType GetEnumItemByName<TEnumType>(String name)
        {
            return (TEnumType)GetEnumItemByName(typeof(TEnumType), name, false);
        }

        public static String FormatComponentName(String str)
        {
            String result = str.Replace(" ", "");
            return StringHelper.Filter(result.ToLower(), "abcdefghijklmnopqrstuvwxyz1234567890");
        }

        public static IDictionary<String, Object> AnonymousToDictionary(Object item)
        {
            if (item is IDictionary)
                return DictionaryToDictionary((IDictionary)item);
            else
            {
                IDictionary<String, Object> dic = new Dictionary<string, object>();
                var properties = System.ComponentModel.TypeDescriptor.GetProperties(item);
                foreach (System.ComponentModel.PropertyDescriptor property in properties)
                {
                    dic.Add(property.Name, property.GetValue(item));
                }
                return dic;
            }
        }

        public static IDictionary<String, Object> DictionaryToDictionary(IDictionary source)
        {
            IDictionaryEnumerator en = source.GetEnumerator();
            IDictionary<String, Object> result = new Dictionary<String, Object>();
            while (en.MoveNext())
                result.Add(en.Key.ToString(), en.Value);
            return result;
        }
    }
}
