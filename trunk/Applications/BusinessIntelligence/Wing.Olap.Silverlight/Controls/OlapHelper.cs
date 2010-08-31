/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Ranet.UILibrary.Olap
 
    Ranet.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Ranet.UILibrary.Olap.  If not, see
  	<http://www.gnu.org/licenses/> 
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Ranet.UILibrary.Olap under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
*/

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Ranet.AgOlap.Controls
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
                if (str != null && str.Length > 1)
                {
                    result = str;

                    //Обрамляем исходную строку [] если она еще не обрамлена
                    if (/*str.IndexOf(" ") > 0 &&*/ str[0] != '[' && str[str.Length - 1] != ']')
                        result = "[" + str + "]";
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
                if (str != null && str.Length > 1)
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
        /// Преобразует строку в формат пригодный для запросов - обрамляет () еcли нужно
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
                    //Обрамляем исходную строку () если она еще не обрамлена
                    if (/*str.IndexOf(" ") > 0 &&*/ (result[0] != '(' && result[result.Length - 1] != ')') &&
                        (result[0] != '[' && result[result.Length - 1] != ']'))
                        result = "(" + result + ")";
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
