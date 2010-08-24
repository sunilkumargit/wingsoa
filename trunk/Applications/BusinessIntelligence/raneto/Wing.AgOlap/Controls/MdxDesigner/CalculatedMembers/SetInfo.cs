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

namespace Ranet.AgOlap.Controls.MdxDesigner.CalculatedMembers
{
    public class CalculatedNamedSetInfo : CalculationInfoBase
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

        public override object Clone()
        {
            CalculatedNamedSetInfo ret = new CalculatedNamedSetInfo();
            ret.Name = Name;
            ret.Expression = Expression;
            return ret;
        }
    }
}
