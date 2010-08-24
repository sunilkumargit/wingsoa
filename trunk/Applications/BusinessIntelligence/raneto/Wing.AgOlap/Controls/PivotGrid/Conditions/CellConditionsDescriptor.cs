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
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Globalization;
using System.IO;
using System.Windows.Media.Imaging;
using Jayrock.Json;
using Ranet.Olap.Core.Data;

namespace Ranet.AgOlap.Controls.PivotGrid.Conditions
{
    public class CellConditionsDescriptor
    {
        public CellConditionsDescriptor()
        { 
        }

        public CellConditionsDescriptor(String memberUniqueName)
        {
            m_MemberUniqueName = memberUniqueName;    
        }

        private String m_MemberUniqueName = string.Empty;
        /// <summary>
        /// Объект, для которого настраиваются условия
        /// </summary>
        public String MemberUniqueName
        {
            get
            {
                return m_MemberUniqueName;
            }
            set
            {
                m_MemberUniqueName = value;
            }
        }

        private IList<CellCondition> m_Conditions = null;
        /// <summary>
        /// Коллекция условий
        /// </summary>
        public IList<CellCondition> Conditions
        {
            get
            {
                if (m_Conditions == null)
                    m_Conditions = new List<CellCondition>();
                return m_Conditions;
            }
            set
            {
                m_Conditions = value;
            }
        }

        /// <summary>
        /// Возвращает список условий, который удовлетворяет значение ячейки
        /// </summary>
        /// <param name="value">Значение ячейки. double.NaN если значение равно NULL</param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public static List<CellCondition> TestToConditions(double value, CellConditionsDescriptor conditionDescriptor)
        {
            if (conditionDescriptor == null)
                return null;

            List<CellCondition> list = new List<CellCondition>();
            foreach (CellCondition cond in conditionDescriptor.Conditions)
            {
                // Если значение ячейки равно null, то value == double.NaN
                // В этом случае пройдут только условия CellConditionType.None. Остальные во избежание казусов прокидываем
                if (value == double.NaN && cond.ConditionType != CellConditionType.None)
                    continue;

                double val1, val2;
                try
                {
                    val1 = Convert.ToDouble(cond.Value1);
                    val2 = Convert.ToDouble(cond.Value2);
                }
                catch
                {
                    continue;
                }

                bool ok = false;
                switch (cond.ConditionType)
                {
                    case CellConditionType.Between:
                        if (val1 < value && value < val2)
                            ok = true;
                        break;
                    case CellConditionType.NotBetween:
                        if (value <= val1 || val2 <= value)
                            ok = true;
                        break;
                    case CellConditionType.Equal:
                        if (val1 == value)
                            ok = true;
                        break;
                    case CellConditionType.Greater:
                        if (value > val1)
                            ok = true;
                        break;
                    case CellConditionType.GreaterOrEqual:
                        if (value >= val1)
                            ok = true;
                        break;
                    case CellConditionType.Less:
                        if (value < val1)
                            ok = true;
                        break;
                    case CellConditionType.LessOrEqual:
                        if (value <= val1)
                            ok = true;
                        break;
                    case CellConditionType.NotEqual:
                        if (value != val1)
                            ok = true;
                        break;
                    case CellConditionType.None:
                        ok = true;
                        break;
                }

                if (ok)
                    list.Add(cond);
            }
            return list;
        }

        public String Serialize()
        {
            StringBuilder sb = new StringBuilder();

            XmlWriter writer = XmlWriter.Create(sb);
            Serialize(writer);
            writer.Close();
            return sb.ToString();
        }

        const string XML_CellConditionsDescriptor = "ccd";
        const string XML_MemberUniqueName = "un";
        const string XML_Conditions = "conditions";

        void Serialize(XmlWriter writer)
        {
            if (writer == null)
                return;

            writer.WriteStartElement(XML_CellConditionsDescriptor);

            var data = new List<object>();

            data.Add(MemberUniqueName);
            var conditions = new List<object>();
            foreach (var cond in Conditions)
            {
                var condition_data = new List<object>();
                condition_data.Add(cond.ConditionType.ToString());
                condition_data.Add(cond.Value1);
                condition_data.Add(cond.Value2);

                var cellAppearance = new List<object>();
                cellAppearance.Add(ToJsonColor(cond.Appearance.BackColor));
                cellAppearance.Add(ToJsonColor(cond.Appearance.BorderColor));
                cellAppearance.Add(ToJsonColor(cond.Appearance.ForeColor));
                cellAppearance.Add(cond.Appearance.CustomImageUri);

                var options = new List<object>();
                options.Add(cond.Appearance.Options.IgnoreAllOptions);
                options.Add(cond.Appearance.Options.ShowValue);
                options.Add(cond.Appearance.Options.UseAllOptions);
                options.Add(cond.Appearance.Options.UseBackColor);
                options.Add(cond.Appearance.Options.UseBorderColor);
                options.Add(cond.Appearance.Options.UseForeColor);
                options.Add(cond.Appearance.Options.UseImage);
                options.Add(cond.Appearance.Options.UseProgressBar);
                cellAppearance.Add(options.ToArray());

                var progressBarOptions = new List<object>();
                progressBarOptions.Add(ToJsonColor(cond.Appearance.ProgressBarOptions.StartColor));
                progressBarOptions.Add(ToJsonColor(cond.Appearance.ProgressBarOptions.EndColor));
                progressBarOptions.Add(cond.Appearance.ProgressBarOptions.MinValue);
                progressBarOptions.Add(cond.Appearance.ProgressBarOptions.MaxValue);
                progressBarOptions.Add(cond.Appearance.ProgressBarOptions.IsIndeterminate);
                cellAppearance.Add(progressBarOptions.ToArray());

                condition_data.Add(cellAppearance.ToArray());
                conditions.Add(condition_data);
            }
            data.Add(conditions.ToArray());

            var result = Jayrock.Json.Conversion.JsonConvert.ExportToString(data.ToArray());
            writer.WriteString(result);

            writer.WriteEndElement();
        }

        public static object[] ToJsonColor(Color color)
        {
            var data = new List<object>();
            data.Add(color.A);
            data.Add(color.R);
            data.Add(color.G);
            data.Add(color.B);
            return data.ToArray();
        }

        public static Color FromJsonColor(JsonArray array)
        {
            if (array != null && array.Length == 4)
            {
                return Color.FromArgb(Convert.ToByte(array.GetValue(0)),
                    Convert.ToByte(array.GetValue(1)),
                    Convert.ToByte(array.GetValue(2)),
                    Convert.ToByte(array.GetValue(3)));
            }
            return Colors.Transparent;
        }

        public static CellConditionsDescriptor Deserialize(String str)
        {
            if (!String.IsNullOrEmpty(str))
            {
                StringReader str_reader = new StringReader(str);
                XmlReader reader = XmlReader.Create(str_reader);

                return CellConditionsDescriptor.Deserialize(reader);
            }
            else
                return null;
        }

        static CellConditionsDescriptor Deserialize(XmlReader reader)
        {
            if (reader != null)
            {
                try
                {
                    CellConditionsDescriptor target = null;

                    if (!(reader.NodeType == XmlNodeType.Element &&
                        reader.Name == XML_CellConditionsDescriptor))
                    {
                        reader.ReadToFollowing(XML_CellConditionsDescriptor);
                    }

                    if (reader.NodeType == XmlNodeType.Element &&
                        reader.Name == XML_CellConditionsDescriptor)
                    {
                        target = new CellConditionsDescriptor();
                        reader.Read();

                        var data = Jayrock.Json.Conversion.JsonConvert.Import(reader.Value) as JsonArray;
                        target.MemberUniqueName = data[0] != null ? CellSetData.ConvertFromJson(data[0]).ToString() : String.Empty;

                        var conditions = data.GetArray(1);
                        for (int i = 0; i < conditions.Length; i++)
                        {
                            var cond_data = conditions.GetArray(i);

                            CellCondition cond = new CellCondition();
                            String type = cond_data.GetValue(0) != null ? cond_data.GetValue(0).ToString() : CellConditionType.None.ToString();
                            cond.ConditionType = (CellConditionType)(CellConditionType.Parse(typeof(CellConditionType), type, true));
                            cond.Value1 = Convert.ToDouble(cond_data.GetValue(1));
                            cond.Value2 = Convert.ToDouble(cond_data.GetValue(2));

                            var cellAppearance = cond_data.GetArray(3);
                            cond.Appearance.BackColor = FromJsonColor(cellAppearance.GetArray(0));
                            cond.Appearance.BorderColor = FromJsonColor(cellAppearance.GetArray(1));
                            cond.Appearance.ForeColor = FromJsonColor(cellAppearance.GetArray(2));
                            cond.Appearance.CustomImageUri = Convert.ToString(cellAppearance.GetValue(3));

                            var options = cellAppearance.GetArray(4);
                            cond.Appearance.Options.IgnoreAllOptions = Convert.ToBoolean(options[0]);
                            cond.Appearance.Options.ShowValue = Convert.ToBoolean(options[1]);
                            cond.Appearance.Options.UseAllOptions = Convert.ToBoolean(options[2]);
                            cond.Appearance.Options.UseBackColor = Convert.ToBoolean(options[3]);
                            cond.Appearance.Options.UseBorderColor = Convert.ToBoolean(options[4]);
                            cond.Appearance.Options.UseForeColor = Convert.ToBoolean(options[5]);
                            cond.Appearance.Options.UseImage = Convert.ToBoolean(options[6]);
                            cond.Appearance.Options.UseProgressBar = Convert.ToBoolean(options[7]);

                            var progressBarOptions = cellAppearance.GetArray(5);
                            cond.Appearance.ProgressBarOptions.StartColor = FromJsonColor(progressBarOptions.GetArray(0));
                            cond.Appearance.ProgressBarOptions.EndColor = FromJsonColor(progressBarOptions.GetArray(1));
                            cond.Appearance.ProgressBarOptions.MinValue = Convert.ToDouble(progressBarOptions.GetValue(2));
                            cond.Appearance.ProgressBarOptions.MaxValue = Convert.ToDouble(progressBarOptions.GetValue(3));
                            cond.Appearance.ProgressBarOptions.IsIndeterminate = Convert.ToBoolean(progressBarOptions.GetValue(4));

                            target.Conditions.Add(cond);
                        }

                        if (reader.NodeType == XmlNodeType.EndElement &&
                            reader.Name == XML_CellConditionsDescriptor)
                        {
                            reader.ReadEndElement();
                        }
                    }
                    return target;
                }
                catch (XmlException ex)
                {
                    throw ex;
                    //return null;
                }
            }
            return null;
        }
    }
}
