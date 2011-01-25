using System;
using System.Linq;

namespace Wing
{
    [System.Diagnostics.DebuggerStepThrough]
    public static class ConvertUtilsExtensions
    {
        public static T As<T>(this Object o)
        {
            return ConversionHelper.Coerce<T>(o);
        }

        public static Boolean AsBoolean(this Int32 o)
        {
            return ConversionHelper.ToBoolean(o);
        }

        public static Boolean AsBoolean(this Int16 o)
        {
            return ConversionHelper.ToBoolean(o);
        }

        public static Boolean AsBoolean(this Int64 o)
        {
            return ConversionHelper.ToBoolean(o);
        }

        public static Boolean AsBoolean(this String o)
        {
            return ConversionHelper.ToBoolean(o);
        }

        public static Boolean AsBoolean(this bool? o)
        {
            return o != null && o.HasValue && o.Value;
        }

        public static DateTime AsDateTime(this String o)
        {
            return ConversionHelper.ToDateTime(o);
        }

        public static DateTime AsDateTime(this DateTime o)
        {
            return o;
        }

        public static DateTime AsDateTime(this DateTime? o)
        {
            return o.HasValue ? o.Value : DateTime.MinValue;
        }

        public static Decimal AsDecimal(this String o)
        {
            return ConversionHelper.ToDecimal(o);
        }

        public static Decimal AsDecimal(this Int32 o)
        {
            return ConversionHelper.ToDecimal(o);
        }

        public static Decimal AsDecimal(this Int16 o)
        {
            return ConversionHelper.ToDecimal(o);
        }

        public static Decimal AsDecimal(this Int64 o)
        {
            return ConversionHelper.ToDecimal(o);
        }

        public static Decimal AsDecimal(this Double o)
        {
            return ConversionHelper.ToDecimal(o);
        }

        public static Decimal AsDecimal(this float o)
        {
            return ConversionHelper.ToDecimal(o);
        }

        public static Decimal AsDecimal(this Decimal? o)
        {
            return o.HasValue ? o.Value : 0;
        }

        public static Int64 AsInt64(this Int16 o)
        {
            return ConversionHelper.ToInt64(o);
        }

        public static Int64 AsInt64(this Int32 o)
        {
            return ConversionHelper.ToInt64(o);
        }

        public static Int64 AsInt64(this String o)
        {
            return ConversionHelper.ToInt64(o);
        }

        public static Int64 AsInt64(this double o)
        {
            return ConversionHelper.ToInt64(o);
        }

        public static Int64 AsInt64(this Decimal o)
        {
            return ConversionHelper.ToInt64(o);
        }

        public static Int64 AsInt64(this float o)
        {
            return ConversionHelper.ToInt64(o);
        }

        public static Int64 AsInt64(this Int64? o)
        {
            return o.HasValue ? o.Value : 0;
        }

        public static Int32 AsInt32(this String o)
        {
            return ConversionHelper.ToInt32(o);
        }

        public static Int32 AsInt32(this Int64 o)
        {
            return ConversionHelper.ToInt32(o);
        }

        public static Int32 AsInt32(this Int32? o)
        {
            return o.HasValue ? o.Value : 0;
        }

        public static Int32 AsInt32(this Int16 o)
        {
            return ConversionHelper.ToInt32(o);
        }

        public static Int32 AsInt32(this double o)
        {
            return ConversionHelper.ToInt32(o);
        }

        public static Int32 AsInt32(this decimal o)
        {
            return ConversionHelper.ToInt32(o);
        }

        public static Int32 AsInt32(this float o)
        {
            return ConversionHelper.ToInt32(o);
        }

        public static Int16 AsInt16(this String o)
        {
            return ConversionHelper.ToInt16(o);
        }

        public static Int16 AsInt16(this Int32 o)
        {
            return ConversionHelper.ToInt16(o);
        }

        public static Int16 AsInt16(this Int64 o)
        {
            return ConversionHelper.ToInt16(o);
        }

        public static Int16 AsInt16(this Int16? o)
        {
            return o.HasValue ? o.Value : (short)0;
        }

        public static Int16 AsInt16(this double o)
        {
            return ConversionHelper.ToInt16(o);
        }

        public static Int16 AsInt16(this decimal o)
        {
            return ConversionHelper.ToInt16(o);
        }

        public static Int16 AsInt16(this float o)
        {
            return ConversionHelper.ToInt16(o);
        }

        public static Double AsDouble(this String o)
        {
            return ConversionHelper.ToDouble(o);
        }

        public static Double AsDouble(this Int32 o)
        {
            return ConversionHelper.ToDouble(o);
        }

        public static Double AsDouble(this Int16 o)
        {
            return ConversionHelper.ToDouble(o);
        }

        public static Double AsDouble(this Int64 o)
        {
            return ConversionHelper.ToDouble(o);
        }

        public static Double AsDouble(this float o)
        {
            return ConversionHelper.ToDouble(o);
        }

        public static Double AsDouble(this decimal o)
        {
            return ConversionHelper.ToDouble(o);
        }

        public static Double AsDouble(this double? o)
        {
            return o.HasValue ? o.Value : 0;
        }

        public static String AsString(this Object o)
        {
            return (o ?? "").ToString();
        }

        public static bool In<T>(this T value, params T[] values)
        {
            if (values == null || values.Length == 0)
                return false;
            return values.Contains(value);
        }
    }
}
