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

namespace Ranet.AgOlap.Controls.MemberChoice
{
    public class OlapHelper
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="connectioString">Строка соединения с БД OLAP</param>
        public OlapHelper(String connectioString)
        {
            m_ConnectionString = connectioString;
        }

        #region Свойства
        private String m_ConnectionString = String.Empty;
        /// <summary>
        /// Строка соединения с БД
        /// </summary>
        public String ConnectionString
        {
            get
            {
                return m_ConnectionString;
            }
        }
        #endregion Свойства

        #region private
        //private AdomdConnection m_Connection;
        ///// <summary>
        ///// Соединение с БД
        ///// </summary>		
        //private AdomdConnection OLAPConnection
        //{
        //    get
        //    {
        //        try
        //        {
        //            if (m_Connection == null)
        //            {
        //                m_Connection = new AdomdConnection();
        //                m_Connection.ConnectionString = ConnectionString;
        //                m_Connection.Open();
        //            }

        //            return m_Connection;
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception(ex.Message + "\n ConnectionString: " + ConnectionString, ex);
        //        }
        //    }
        //}
        #endregion private

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
        #endregion Методы для преобразования строк

        //#region Загрузка из OLAP
        ///// <summary>
        ///// Выполняет запрос к OLAP
        ///// </summary>
        ///// <param name="Query">MDX - запрос</param>
        ///// <returns></returns>
        //protected CellSet ExecuteQuery(String Query)
        //{
        //    try
        //    {
        //        AdomdConnection conn = AdomdConnectionPool.GetConnection(m_ConnectionString);
        //        {
        //            //conn.Open();
        //            using (AdomdCommand cmd = new AdomdCommand(Query, conn))
        //            {
        //                CellSet cellSet = cmd.ExecuteCellSet();
        //                return cellSet;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(string.Format(Localization.Exc_ErrorExecuteQuery, Query, m_ConnectionString), ex);
        //    }
        //}

        ///// <summary>
        ///// Возвращает Caption для KPI по его Name
        ///// </summary>
        ///// <param name="cubeName">имя куба</param>
        ///// <param name="name">имя KPI</param>
        ///// <returns></returns>
        //public String GetKpiCaption(String cubeName, String name)
        //{
        //    try
        //    {
        //        //using (AdomdConnection conn = new AdomdConnection(m_ConnectionString))
        //        AdomdConnection conn = AdomdConnectionPool.GetConnection(m_ConnectionString);
        //        {
        //            //conn.Open();

        //            CubeDef cubeDef = conn.Cubes.Find(OlapHelper.ConvertToNormalStyle(cubeName));
        //            if (cubeDef == null)
        //                return null;

        //            try
        //            {
        //                foreach (Kpi kpi in cubeDef.Kpis)
        //                {
        //                    if (kpi.Name == name)
        //                        return kpi.Caption;
        //                }
        //                return null;
        //            }
        //            catch (NotSupportedException)
        //            {
        //                return null;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // TODO: Localize!!!
        //        throw new Exception(ex.Message + "\n Строка соединения: " + ConnectionString);
        //    }
        //}
        //#endregion Загрузка из OLAP
    }
}
