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
using System.Text.RegularExpressions;

namespace Ranet.AgOlap.Controls.General
{
    /// <summary>
    /// Форматирует строку с текстом команды обновления, чтобы она была более читабельной для человека.
    /// </summary>
    public class OlapCmdUpdateStringFormatter
    {
        private static string m_Str1stFormatStepRegEx = @"(?<UpdatePart>update\s+(\w+)+\s*\[.*?\])\s*(?<SetPart>SET)\s*(?<TuplePart>\(.*)(?<EqualPart>\)\s*=)\s*(?<ValuePart>.*)(?<UsePart>USE_\w+)((?<ByPart>\s+BY)(?<WeightPart>.*)(?<EndParenthesis>\))){0,1}";
        private static string m_StrTupleAndValueFormatRegEx = @"(\,*\s*(\[.*?\]\.*&*(\w+){0,1})+)";
        private static string m_StrWeightFormatRegEx = @"((\,*\s*(\w+)+\s*\(\s*((\,*\s*(\[.*?\]\.*&*(\w+){0,1})+)\s*)+\s*\)\s*)|(\,*\s*(\[.*?\]\.*&*(\w+){0,1})+))";
        private static string m_StrOperationFormatRegEx = @"(\)\s*[-+/*])\s*(\()";//скобки рядом с арифметической операцией в весовой части

        /// <summary>
        /// Форматировать.
        /// </summary>
        /// <param name="strUpdateCmd"></param>
        /// <returns></returns>
        public static string Format(string strUpdateCmd)
        {
            //Пока распознает не все возможные варианты текста команды обновления, форматирует не идеально, но приемлемо
            //Сделать 1 рег. выражением нет смысла: 1. Выражение будет оч. сложное 2.Будет очень долго работать
            //(десятки секунд +, а надо формат-ть практически "на лету" при выводе куда-то )
            string strFormmattedCmd = String.Empty;
            Regex regEx = new Regex(m_Str1stFormatStepRegEx, RegexOptions.IgnoreCase);
            //1 совпадение - вся строка, разбитая на именованые группы, которые представляют собой основные куски запроса
            GroupCollection grpCol = regEx.Match(strUpdateCmd).Groups;
            //Форматирование координат Tuple СЛЕВА от знака присваивания в SET
            regEx = new Regex(m_StrTupleAndValueFormatRegEx, RegexOptions.IgnoreCase);
            string strTuple = regEx.Replace(grpCol["TuplePart"].Value, Environment.NewLine + "$1");
            //Форматирование координат Tuple СПРАВА от знака присваивания в SET
            string strValue = regEx.Replace(grpCol["ValuePart"].Value, "$1" + Environment.NewLine);
            string strWeight = String.Empty;
            if (grpCol["WeightPart"].Value != null && grpCol["WeightPart"].Value != String.Empty)//Если есть весовая часть
            {
                //Форматирование веса с учетом того, что могут попадаться вызовы ф-ций
                regEx = new Regex(m_StrWeightFormatRegEx, RegexOptions.IgnoreCase);
                strWeight = regEx.Replace(grpCol["WeightPart"].Value, "$1" + Environment.NewLine);
                //Форматирование арифметич операций в весе, если есть
                regEx = new Regex(m_StrOperationFormatRegEx, RegexOptions.IgnoreCase);
                strWeight = regEx.Replace(strWeight, String.Format("$1{0}$2{0}", Environment.NewLine));
            }
            strFormmattedCmd = String.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{7}{0}{8}{0}{9}", Environment.NewLine,
                grpCol["UpdatePart"].Value, grpCol["SetPart"].Value, strTuple, grpCol["EqualPart"].Value,
                strValue, grpCol["UsePart"].Value, grpCol["ByPart"].Value, strWeight, grpCol["EndParenthesis"].Value);
            return strFormmattedCmd;
        }
    }
}
