using System;
using System.Linq;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Collections.Generic;

namespace Wing
{
    [System.Diagnostics.DebuggerStepThrough]
    public static class ConversionHelper
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
            byte[] bytes = ASCIIEncoding.ASCII.GetBytes(str);
            return Convert.ToBase64String(bytes);
        }

        public static String ToBase64String(Byte[] value)
        {
            return Convert.ToBase64String(value);
        }

        public static String FromBase64String(String str)
        {
            byte[] bytes = Convert.FromBase64String(str);
            return ASCIIEncoding.ASCII.GetString(bytes);
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
                result = StringHelper.Parse(value);
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
            var result = new ArrayList();
            if (array != null)
            {
                for (var i = 0; i < array.Length; i++)
                {
                    var item = array[i];
                    if (item.GetType().IsArray)
                        result.AddRange((ICollection)item);
                    else
                        result.Add(item);
                }
            }
            return result.ToArray();
        }

        public static String StringToHexToken(String str)
        {
            var random = new Random(DateTime.Now.Millisecond);
            var keySize = random.Next(5, 7);
            var key = "";
            while (key.Length < keySize)
                key += random.Next(0, 15).ToString("X"); // FFFFF

            var result = CryptographyHelper.EncodeToHex(str, key);

            //colocar a chave na string criptografada, nos primeiros 255 caracters.
            var keyPos = RandomHelper.Next(0, Math.Max(Math.Min(result.Length - 2, 255), 0));
            key = CryptographyHelper.EncodeToHex(key);
            // primeiro a tamanho da chave e depois a cahve
            result = result.Insert(keyPos, String.Format("{0}{1}", key.Length.ToString("X2"), key));
            // no fim da string vai a posicao em que a chave foi posta
            result += keyPos.ToString("X2");
            return result.ToLowerInvariant();
        }

        public static String HexTokenToString(String token)
        {
            try
            {
                //ler a posicao em que a chave está
                var keyPos = Int32.Parse(token.Substring(token.Length - 2, 2), NumberStyles.AllowHexSpecifier);
                //ler o tamanho da chave
                var keyLength = Int32.Parse(token.Substring(keyPos, 2), NumberStyles.AllowHexSpecifier);
                //ler a chave usada
                var key = token.Substring(keyPos + 2, keyLength);
                //por fim descriptografar o conteudo, para isso é preciso remover a chave do meio da string e também a sua posição do final da string
                token = token.Remove(keyPos, keyLength + 2);
                token = token.Substring(0, token.Length - 2);
                return CryptographyHelper.DecodeFromHex(token, CryptographyHelper.DecodeFromHex(key));
            }
            catch
            {
                throw new Exception("A sequencia de caracteres não é um token válido");
            }
        }

        public static PrimitiveDataGroup GetPrimitiveDataGroup(Type type)
        {
            if (type == typeof(String) || type == typeof(char))
                return PrimitiveDataGroup.String;
            if (type == typeof(int) || type == typeof(Int64) || type == typeof(Int16)
                || type == typeof(uint) || type == typeof(UInt16) || type == typeof(UInt64)
                || type == typeof(double) || type == typeof(float)
                || type == typeof(byte) || type == typeof(Decimal))
                return PrimitiveDataGroup.Number;
            if (type == typeof(DateTime))
                return PrimitiveDataGroup.DateTime;
            if (type == typeof(Boolean))
                return PrimitiveDataGroup.Boolean;
            if (type == typeof(Guid))
                return PrimitiveDataGroup.Guid;
            throw new Exception("Tipo não se encaixa em nenhum tipo básico: " + type.FullName);
        }

        public static bool IsFloatingPointType(Type type)
        {
            return type == typeof(Double) || type == typeof(Decimal) || type == typeof(float);
        }

        public static bool IsNullValue(object fieldValue)
        {
            if (fieldValue == null)
                return true;
            var type = fieldValue.GetType();
            //                if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            return false;
        }

        public static string ToLower(this Boolean value)
        {
            return value.ToString().ToLower();
        }

        public static object[] CoerceArray(ICollection collection, Type toType)
        {
            var result = new List<Object>(collection.Count);
            foreach (var item in collection)
                result.Add(Coerce(item, toType));
            return result.ToArray();
        }

        public static T[] CoerceArray<T>(ICollection collection)
        {
            return CoerceArray(collection, typeof(T)).Cast<T>().ToArray();
        }
    }
}