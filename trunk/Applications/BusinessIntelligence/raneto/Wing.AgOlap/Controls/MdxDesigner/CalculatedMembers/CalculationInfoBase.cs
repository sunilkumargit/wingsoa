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
using System.Xml.Serialization;
using Ranet.AgOlap.Controls.General;

namespace Ranet.AgOlap.Controls.MdxDesigner.CalculatedMembers
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
