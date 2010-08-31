/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
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
using System.Text;
using System.Collections.Generic;

namespace Wing.AgOlap.Controls.MdxDesigner.CalculatedMembers
{
    public class CalcMemberInfo : CalculationInfoBase
    {
        String m_Expression = String.Empty;
        /// <summary>
        /// Выражение для вычисления
        /// </summary>
        public String Expression
        {
            get { return m_Expression; }
            set { m_Expression = value; }
        }

        List<String> m_NonEmptyBehavior;
        /// <summary>
        /// Список названий мер, учавствующих в построении NonEmptyBehavior
        /// </summary>
        public List<String> NonEmptyBehavior
        {
            get {
                if (m_NonEmptyBehavior == null)
                {
                    m_NonEmptyBehavior = new List<String>();
                }
                return m_NonEmptyBehavior; 
            }
            set { m_NonEmptyBehavior = value; }
        }

        String m_FormatString = String.Empty;
        /// <summary>
        /// Строка форматирования
        /// </summary>
        public String FormatString
        {
            get { return m_FormatString; }
            set { m_FormatString = value; }
        }

        Color m_BackColor = Colors.Transparent;
        /// <summary>
        /// Цвет фона
        /// </summary>
        public Color BackColor
        {
            get { return m_BackColor; }
            set { m_BackColor = value; }
        }

        Color m_ForeColor = Colors.Transparent;
        /// <summary>
        /// Цвет фона
        /// </summary>
        public Color ForeColor
        {
            get { return m_ForeColor; }
            set { m_ForeColor = value; }
        }

        public String GetScript()
        {
            // [Measures].[Сумма] * 5, FORMAT_STRING = "#,#.00", ASSOCIATED_MEASURE_GROUP = 'Товарно финансовые потоки', DISPLAY_FOLDER = 'Тест', NON_EMPTY_BEHAVIOR = {[Сумма] }

            if (!String.IsNullOrEmpty(Expression))
            {
                StringBuilder script = new StringBuilder();
                script.Append(Expression);

                if (!String.IsNullOrEmpty(FormatString))
                {
                    script.AppendFormat(", FORMAT_STRING = \"{0}\"", FormatString);
                }

                String nonEmpty = String.Empty;
                foreach (var item in NonEmptyBehavior)
                {
                    if (!String.IsNullOrEmpty(item))
                    {
                        if (!String.IsNullOrEmpty(nonEmpty))
                            nonEmpty += ", ";
                        nonEmpty += String.Format("[{0}]", item);
                    }
                }
                if (!String.IsNullOrEmpty(nonEmpty))
                {
                    script.AppendFormat(", NON_EMPTY_BEHAVIOR = {0}", "{" + nonEmpty + "}");
                }

                if (BackColor != Colors.Transparent)
                {
                    script.AppendFormat(", BACK_COLOR = Rgb({0},{1},{2})", BackColor.R.ToString(), BackColor.G.ToString(), BackColor.B.ToString());
                }

                if (ForeColor != Colors.Transparent)
                {
                    script.AppendFormat(", FORE_COLOR = Rgb({0},{1},{2})", ForeColor.R.ToString(), ForeColor.G.ToString(), ForeColor.B.ToString());
                }

                return script.ToString();
            }
            return String.Empty;
        }

        public override object Clone()
        {
            CalcMemberInfo ret = new CalcMemberInfo();
            ret.Name = Name;
            ret.Expression = Expression;
            ret.FormatString = FormatString;
            ret.BackColor = BackColor;
            ret.ForeColor = ForeColor;

            foreach (var item in NonEmptyBehavior)
            {
                ret.NonEmptyBehavior.Add(item);
            }
            return ret;
        }

    }
}
