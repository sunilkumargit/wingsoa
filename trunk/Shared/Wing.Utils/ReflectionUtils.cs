using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Wing.Utils
{
    public static class ReflectionUtils
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

            if ((attrs == null)
                || (attrs.Length == 0))
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

        public static bool HasAttribute<TAttributeType>(MemberInfo info) where TAttributeType : Attribute
        {
            return HasAttribute<TAttributeType>(info, false);
        }

        public static bool HasAttribute<TAttributeType>(MemberInfo info, bool inheritAttr) where TAttributeType : Attribute
        {
            return GetAttribute<TAttributeType>(info, inheritAttr) != null;
        }

        public static bool HasAttribute<TAttributeType, TType>() where TAttributeType : Attribute
        {
            return HasAttribute<TAttributeType>(typeof(TType));
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
                if (HasAttribute<TAttributeType>(p, inheritAttr))
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

        public static Boolean HasProperty(Type type, String propertyName)
        {
            return type.GetProperty(propertyName) != null;
        }

        public static Boolean HasProperty<TType>(String propertyName)
        {
            return HasProperty(typeof(TType), propertyName);
        }

        public static TResultType ReadStaticProperty<TResultType>(Type type, String propertyName)
        {
            return ConvertUtils.Coerce<TResultType>(ReadStaticProperty(type, propertyName));
        }

        public static Object ReadStaticProperty(Type type, String propertyName)
        {
            return ReflectionUtils.InvokeMember(type, propertyName, null,
                BindingFlags.Static
                | BindingFlags.GetProperty
                | BindingFlags.Public
                | BindingFlags.NonPublic
                | BindingFlags.FlattenHierarchy);
        }

        public static Object ReadProperty(Object instance, String propertyName, BindingFlags extraBindings)
        {
            return ReflectionUtils.InvokeMember(instance.GetType(), propertyName, instance,
                BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance | extraBindings);
        }

        public static TResultType ReadProperty<TResultType>(Object instance, String propertyName)
        {
            return ConvertUtils.Coerce<TResultType>(ReadProperty(instance, propertyName, BindingFlags.Default));
        }

        public static TResultType ReadProperty<TResultType>(Object instance, String propertyName, BindingFlags extraBindings)
        {
            return ConvertUtils.Coerce<TResultType>(ReadProperty(instance, propertyName, extraBindings));
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
            WriteProperty(instance, propertyName, ConvertUtils.Coerce(value, prop.PropertyType));
        }

        public static TResultType InvokeMethod<TResultType>(Object instance, String methodName, BindingFlags extraBindings, params object[] args)
        {
            return (TResultType)ReflectionUtils.InvokeMember(instance.GetType(), methodName, instance,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod | extraBindings, args);
        }

        public static TResultType InvokeMethod<TResultType>(Object instance, String methodName, params object[] args)
        {
            return InvokeMethod<TResultType>(instance, methodName, BindingFlags.Default, args);
        }

        public static Object InvokeDefaultConstructor(Type type)
        {
            return type.GetConstructor(Type.EmptyTypes).Invoke(new Object[0]);
        }

        public static TType InvokeDefaultConstructor<TType>()
        {
            return (TType)InvokeDefaultConstructor(typeof(TType));
        }

        public static Object InvokeMethod(Object instance, String methodName, params object[] args)
        {
            return InvokeMethod<Object>(instance, methodName, args);
        }

        public static TTYPE FirstEnumItem<TTYPE>()
        {
            return (TTYPE)FirstEnumItem(typeof(TTYPE));
        }

        private static Dictionary<Type, Object> _cachedEnumItems = new Dictionary<Type, Object>();
        public static Object FirstEnumItem(Type type)
        {
            Assert.NullArgument(type, "type");
            Object result = null;
            if (!_cachedEnumItems.TryGetValue(type, out result))
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
            return StringUtils.Filter(result.ToLower(), "abcdefghijklmnopqrstuvwxyz1234567890");
        }

        public static IDictionary<String, Object> AnonymousToDictionary(Object item)
        {
            if (item is IDictionary)
                return DictionaryToDictionary((IDictionary)item);
            else
            {
                IDictionary<String, Object> dic = new Dictionary<string, object>();
                var properties = item.GetType().GetProperties();
                foreach (var property in properties)
                {
                    dic.Add(property.Name, property.GetValue(item, null));
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
