using System;
using Wing.Olap.Controls.MdxDesigner.CalculatedMembers;

namespace Wing.Olap.Controls.MdxDesigner.Wrappers
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
