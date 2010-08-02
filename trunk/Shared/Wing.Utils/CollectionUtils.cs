using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Wing.Utils
{
    public static class CollectionUtils
    {
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


        public static String SerializeToHexString(this IDictionary<String, String> dict)
        {
            var key = DateTime.Now.ToString("mmssfffhh");
            var result = CryptoUtils.EncodeToHex(SerializeToString(dict), key);
            //adicionar a chave usada no fim da string
            key = CryptoUtils.EncodeToHex(key);
            result += key;
            //e por fim os dois ultimos caracteres servem para identifcar o tamanho da chave
            result += key.Length.ToString("X2");
            return result;
        }

        public static String SerializeToString(this IDictionary<String, String> dict)
        {
            //montar a lista
            var res = new StringBuilder();
            foreach (String key in dict.Keys)
            {
                var vlr = dict[key].AsString();
                res.Append(key.Length.ToString("X3"));
                res.Append(key);
                res.Append(vlr.Length.ToString("X3"));
                res.Append(vlr);
            }
            return res.ToString();
        }

        public static void DeserializeFromHexString(this IDictionary<String, String> dict, String str)
        {
            //ler o tamanho da chave (2 ultimas posições)
            var keyLength = Int32.Parse(str.Substring(str.Length - 2, 2), NumberStyles.AllowHexSpecifier);
            //ler a chave usada
            var key = str.Substring(str.Length - keyLength - 2, keyLength);
            //descriptografar a chave
            key = CryptoUtils.DecodeFromHex(key);
            //por fim descriptografar o conteudo
            str = str.Substring(0, str.Length - keyLength - 2);
            dict.DeserializeFromString(CryptoUtils.DecodeFromHex(str, key));
        }

        public static void DeserializeFromString(this IDictionary<String, String> dict, String str)
        {
            string lastKey = null;
            int ptr = 0;
            while (ptr < str.Length)
            {
                //tamanho 
                var len = int.Parse(str.Substring(ptr, 3), System.Globalization.NumberStyles.HexNumber);
                ptr += 3;
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
    }
}
