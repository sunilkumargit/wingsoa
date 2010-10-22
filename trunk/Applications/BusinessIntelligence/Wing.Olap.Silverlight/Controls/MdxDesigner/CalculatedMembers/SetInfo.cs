using System;

namespace Wing.Olap.Controls.MdxDesigner.CalculatedMembers
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
