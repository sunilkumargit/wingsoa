using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Wing
{
    public static class CollectionExtensions
    {
        public static readonly char SERIALIZATION_WILDCHAR = '|';

        public static List<TType> Slice<TType>(this IEnumerable<TType> collection, int pageSize, int page, int startIndex, bool includePagesAhead)
        {
            var result = new List<TType>();
            //calcular o indice da pagina
            startIndex += Math.Max(pageSize * page - pageSize, 0);
            //fazer um loop até o startIndex
            var en = collection.GetEnumerator();
            while (startIndex-- > 0 && en.MoveNext()) { }
            //adicionar o restante no resultado
            var count = 0;
            while ((++count <= pageSize || includePagesAhead) && en.MoveNext())
                result.Add(en.Current);
            return result;
        }

        public static List<TType> Slice<TType>(this IEnumerable<TType> collection, int pageSize, int page, int startIndex)
        {
            return Slice(collection, pageSize, page, startIndex, false);
        }

        public static List<TType> Slice<TType>(this IEnumerable<TType> collection, int pageSize, int page)
        {
            return Slice(collection, pageSize, page, 0, false);
        }

        public static void AddRange(this IList list, IEnumerable items)
        {
            foreach (var item in items)
                list.Add(item);
        }

        public static void AddRange<TTYPE>(this IList<TTYPE> list, IEnumerable<TTYPE> items)
        {
            foreach (var item in items)
                list.Add(item);
        }

        public static object CountPages(int count, int pageSize, bool enableZeroPages)
        {
            int res = ((int)count / pageSize);
            if (res * pageSize - pageSize > count)
                res--;
            if (res == 0 && !enableZeroPages)
                return 1;
            return res;
        }

        public static object CountPages(int count, int pageSize)
        {
            return CountPages(count, pageSize, false);
        }


        public static String GetToken(this IDictionary<String, String> dict)
        {
            return ConversionHelper.StringToHexToken(dict.SerializeToString());
        }

        public static String SerializeToString(this IDictionary<String, String> dict)
        {
            //montar a lista
            var res = new StringBuilder();
            foreach (var keyValue in dict)
                res.AppendFormat("{0:X}{1}{2}{3:X}{1}{4}", keyValue.Key.Length,
                    SERIALIZATION_WILDCHAR, keyValue.Key, keyValue.Value.AsString().Length, keyValue.Value);
            return res.ToString();
        }

        public static String SerializeToString(this IList<String> list)
        {
            var res = new StringBuilder();
            foreach (String item in list)
            {
                if (item == null)
                    throw new Exception("A lista não pode conter elementos nulos para poder ser serializada");
                res.AppendFormat("{0:X}{1}{2}", item.Length, SERIALIZATION_WILDCHAR, item);
            }
            return res.ToString();
        }

        public static String GetToken(this IList<String> list)
        {
            return ConversionHelper.StringToHexToken(list.SerializeToString());
        }

        public static void DeserializeFromString(this IList<String> list, String str)
        {
            int ptr = 0;
            while (ptr < str.Length)
            {
                var pipePos = str.IndexOf(SERIALIZATION_WILDCHAR, ptr, Math.Min(ptr + 10, str.Length - ptr));
                //tamanho 
                var len = int.Parse(str.Substring(ptr, pipePos - ptr), System.Globalization.NumberStyles.HexNumber);
                ptr = pipePos + 1;
                //valor
                list.Add(str.Substring(ptr, len));
                ptr += len;
            }
        }

        public static void SetToken(this IDictionary<String, String> dict, String token)
        {
            dict.DeserializeFromString(ConversionHelper.HexTokenToString(token));
        }

        public static void SetToken(this IList<String> list, String token)
        {
            list.DeserializeFromString(ConversionHelper.HexTokenToString(token));
        }

        public static void DeserializeFromString(this IDictionary<String, String> dict, String str)
        {
            string lastKey = null;
            int ptr = 0;
            while (ptr < str.Length)
            {
                var pipePos = str.IndexOf(SERIALIZATION_WILDCHAR, ptr, Math.Min(ptr + 10, str.Length - ptr));
                //tamanho 
                var len = int.Parse(str.Substring(ptr, pipePos - ptr), System.Globalization.NumberStyles.HexNumber);
                ptr = pipePos + 1;
                //valor
                var vlr = str.Substring(ptr, len);
                ptr += len;
                //key ou valor?
                if (lastKey == null)
                    lastKey = vlr;
                else
                {
                    dict[lastKey] = vlr;
                    lastKey = null;
                }
            }
        }

        public static String SerializeToString(this IDictionary<String, List<String>> dict)
        {
            var innerDic = new Dictionary<String, String>();
            foreach (var keyPair in dict)
                innerDic[keyPair.Key] = keyPair.Value.SerializeToString();
            return innerDic.SerializeToString();
        }

        public static void DeserializeFromString(this IDictionary<String, List<String>> dict, String str)
        {
            var innerDic = new Dictionary<String, String>();
            innerDic.DeserializeFromString(str);
            foreach (var keyPair in innerDic)
            {
                List<String> list = null;
                if (!dict.TryGetValue(keyPair.Key, out list))
                    list = new List<string>();
                list.DeserializeFromString(keyPair.Value);
                dict[keyPair.Key] = list;
            }
        }

        public static String GetToken(this IDictionary<String, List<String>> dict)
        {
            return ConversionHelper.StringToHexToken(dict.SerializeToString());
        }

        public static void SetToken(this IDictionary<String, List<String>> dict, String token)
        {
            var str = ConversionHelper.HexTokenToString(token);
            dict.DeserializeFromString(str);
        }

        public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IList<T> list)
        {
            return new ReadOnlyCollection<T>(list);
        }

        public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> enumerable)
        {
            return ToReadOnlyCollection(new List<T>(enumerable));
        }

        public static void AddIfNotExists<T>(this List<T> list, T item)
        {
            if (!list.Contains(item))
                list.Add(item);
        }

        public static void Enqueue<T>(this Queue<T> queue, IEnumerable<T> items)
        {
            foreach (var item in items)
                queue.Enqueue(item);
        }
    }
}
