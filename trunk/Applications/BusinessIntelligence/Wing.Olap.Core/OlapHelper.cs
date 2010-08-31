/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Olap.Core
{
    public class OlapHelper
    {
        #region Методы для преобразования строк
        /// <summary>
        /// Преобразует строку в формат пригодный для запросов - обрамляет [] ели нужно
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static public String ConvertToQueryStyle(String toConvert)
        {
            try
            {
                String str = toConvert.Trim();
                String result = String.Empty;
                if (!String.IsNullOrEmpty(str) && str.Length > 1)
                {
                    result = str;

                    //Обрамляем исходную строку [] если она еще не обрамлена
                    if (str[0] == '[' && str[str.Length - 1] == ']')
                    {
                        // Строка уже обрамлена
                    }
                    else
                    {
                        result = "[" + str + "]";
                    }
                }
                return result;
            }
            catch
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Преобразует строку из формата пригодный для запросов  в нормальный формат (например, убирает обрамляющие [])
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static public String ConvertToNormalStyle(String str)
        {
            try
            {
                String result = String.Empty;
                if (!String.IsNullOrEmpty(str) && str.Length > 1)
                {
                    result = str;

                    //Убираем обрамляющие []
                    if (str[0] == '[' && str[str.Length - 1] == ']')
                        result = str.Substring(1, str.Length - 2);
                }
                return result;
            }
            catch
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Преобразует строку в формат пригодный для запросов - обрамляет () еcли нужно.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static public String ConvertSubCubeToQueryStyle(String toConvert)
        {
            if (String.IsNullOrEmpty(toConvert))
                return String.Empty;
            try
            {
                String result = toConvert.Trim();
                if (result != null && result.Length > 1)
                {
                    //Обрамляем исходную строку () если она еще не обрамлена. Причем если исходная строка обрамлена [], то обрамлять не нужно
                    if ((result[0] == '(' && result[result.Length - 1] == ')') ||
                        (result[0] == '[' && result[result.Length - 1] == ']'))
                    { 
                        // Строка уже обрамлена
                    }
                    else
                    {
                        result = "(" + result + ")";
                    }
                }
                return result;
            }
            catch
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Преобразует строку из формата пригодный для запросов  в нормальный формат (например, убирает обрамляющие ())
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static public String ConvertSubCubeToNormalStyle(String toConvert)
        {
            if (String.IsNullOrEmpty(toConvert))
                return String.Empty;

            try
            {
                String result = toConvert.Trim();
                if (result != null && result.Length > 1)
                {
                    //Убираем обрамляющие ()
                    if (result[0] == '(' && result[result.Length - 1] == ')')
                        result = result.Substring(1, result.Length - 2);
                }
                return result;
            }
            catch
            {
                return String.Empty;
            }
        }
        #endregion Методы для преобразования строк
    }
}
