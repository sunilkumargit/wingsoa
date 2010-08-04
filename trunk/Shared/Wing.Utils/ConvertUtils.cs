using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Wing.Utils
{
    public static class ConvertUtils
    {
        public static Boolean ToBoolean(Object o)
        {
            if (o == null)
                return false;

            if (o.ToString() == "on")
                return true;

            Boolean result;
            if (!Boolean.TryParse(o.ToString(), out result))
                return false;
            else
                return result;
        }

        public static DateTime ToDateTime(Object o)
        {
            if (o == null)
                return DateTime.MinValue;

            DateTime result;
            if (!DateTime.TryParse(o.ToString(), out result))
                return DateTime.MinValue;
            else
                return result;
        }

        public static Decimal ToDecimal(Object o)
        {
            var result = 0M;
            if (o != null)
            {
                try
                {
                    result = Decimal.Parse(o.ToString(), System.Globalization.NumberStyles.Any);
                }
                catch { }
            }
            return result;
        }

        public static Int64 ToInt64(Object o)
        {
            if (o == null)
                return 0;

            Int64 result;
            if (!Int64.TryParse(o.ToString(), out result))
                return 0;

            return result;
        }

        public static Int32 ToInt32(Object o)
        {
            if (o == null)
                return 0;

            Int32 result;
            if (!Int32.TryParse(o.ToString(), out result))
                return 0;

            return result;
        }

        public static Int16 ToInt16(Object o)
        {
            if (o == null)
                return 0;

            Int16 result;
            if (!Int16.TryParse(o.ToString(), out result))
                return 0;

            return result;
        }

        public static Double ToDouble(Object o)
        {
            if (o == null)
                return 0;

            Double result;
            if (!Double.TryParse(o.ToString(), out result))
                return 0;

            return result;
        }

        public static String ToBase64String(String str)
        {
            byte[] bytes = UTF8Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(bytes);
        }

        public static String ToBase64String(Byte[] value)
        {
            return Convert.ToBase64String(value);
        }

        public static String FromBase64String(String str)
        {
            byte[] bytes = Convert.FromBase64String(str);
            return UTF8Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        public static Object Coerce(Object value, Type type)
        {
            Object result = value;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)
                || type.Equals(typeof(Nullable)))
            {
                if (value == null)
                    return null;
                else
                    type = Nullable.GetUnderlyingType(type);
            }

            if (type.Equals(typeof(Int64)))
                result = ToInt64(value);
            else if (type.Equals(typeof(Int32)))
                result = ToInt32(value);
            else if (type.Equals(typeof(String)))
                result = StringUtils.Parse(value);
            else if (type.Equals(typeof(Decimal)))
                result = ToDecimal(value);
            else if (type.Equals(typeof(DateTime)))
                result = ToDateTime(value);
            else if (type.Equals(typeof(Boolean)))
                result = ToBoolean(value);
            else if (type.Equals(typeof(Int16)))
                result = ToInt16(value);
            else if (type.Equals(typeof(Double)))
                result = ToDouble(value);

            return result;
        }

        public static T Coerce<T>(Object value)
        {
            return (T)Coerce(value, typeof(T));
        }

        public static Object Default(Type type)
        {
            if (type.Equals(typeof(Int64))
                || type.Equals(typeof(Int32))
                || type.Equals(typeof(Int16))
                || type.Equals(typeof(Decimal))
                || type.Equals(typeof(Double)))
                return Coerce(0, type);
            else if (type.Equals(typeof(DateTime)))
                return ToDateTime(null);
            else if (type.Equals(typeof(String)))
                return "";
            return null;
        }

        public static TType Default<TType>()
        {
            return Coerce<TType>(Default(typeof(TType)));
        }

        public static Object[] ToFlattenArrayOfObject<T>(ref T[] array)
        {
            var result = new List<Object>();
            if (array != null)
            {
                for (var i = 0; i < array.Length; i++)
                {
                    var item = array[i];
                    if (item.GetType().IsArray)
                    {
                        foreach (var o in (ICollection)item)
                            result.Add(o);
                    }
                    else
                        result.Add(item);
                }
            }
            return result.ToArray();
        }
    }
}
