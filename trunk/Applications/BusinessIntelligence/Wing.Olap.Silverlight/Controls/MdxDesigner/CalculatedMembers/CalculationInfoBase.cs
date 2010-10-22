using System;
using System.Xml.Serialization;

namespace Wing.Olap.Controls.MdxDesigner.CalculatedMembers
{
    [XmlInclude(typeof(CalcMemberInfo))]
    [XmlInclude(typeof(CalculatedNamedSetInfo))]
    public class CalculationInfoBase
    {
        String m_Name = String.Empty;
        /// <summary>
        /// Имя вычисляемого элемента
        /// </summary>
        public String Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        #region IClonable Members

        public virtual object Clone()
        {
            return this;
        }

        #endregion
    }
}
