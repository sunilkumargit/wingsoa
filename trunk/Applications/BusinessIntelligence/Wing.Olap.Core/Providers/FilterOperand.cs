/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections.Generic;

namespace Wing.Olap.Core.Providers
{
    public enum ConditionTypes
    { 
        Equal,
        Contains,
        BeginWith
    }

    public class FilterOperand : FilterOperationBase
    {
        public String Property = String.Empty;
        List<String> m_PropertyLevels = null;
        public List<String> PropertyLevels
        {
            get
            {
                if (m_PropertyLevels == null)
                {
                    m_PropertyLevels = new List<String>();
                }
                return m_PropertyLevels;
            }
        }

        public ConditionTypes Condition = ConditionTypes.Equal;
        public String Value = String.Empty;

        public FilterOperand()
        { }

        public FilterOperand(String property, List<String> propertyLevels, ConditionTypes condition, String value)
        {
            Property = property;
            m_PropertyLevels = propertyLevels;
            Condition = condition;
            Value = value;
        }

    }
}
