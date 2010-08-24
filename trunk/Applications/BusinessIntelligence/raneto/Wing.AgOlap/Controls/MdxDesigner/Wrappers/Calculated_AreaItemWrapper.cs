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
using Ranet.AgOlap.Controls.MdxDesigner.CalculatedMembers;

namespace Ranet.AgOlap.Controls.MdxDesigner.Wrappers
{
    public class Calculated_AreaItemWrapper : AreaItemWrapper
    {
        String m_Name = String.Empty;
        public String Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        public Calculated_AreaItemWrapper()
        {
        
        }

        public Calculated_AreaItemWrapper(CalculationInfoBase info)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            Caption = info.Name;
            Name = info.Name;
        }
    }
}
