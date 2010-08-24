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

namespace Ranet.AgOlap.Controls.PivotGrid.Controls
{
    public class PivotGridSizeInfo
    {
        public PivotGridSizeInfo()
        {
        }

        double m_Scale = 1;
        public double Scale
        {
            get { return m_Scale; }
            set { m_Scale = value; }
        }

        MembersAreaSizeInfo m_ColumnsAreaSize;
        public MembersAreaSizeInfo ColumnsAreaSize
        {
            get {
                if (m_ColumnsAreaSize == null)
                {
                    m_ColumnsAreaSize = new MembersAreaSizeInfo();
                }
                return m_ColumnsAreaSize;
            }
            set {
                m_ColumnsAreaSize = value;
            }
        }

        MembersAreaSizeInfo m_RowsAreaSize;
        public MembersAreaSizeInfo RowsAreaSize
        {
            get
            {
                if (m_RowsAreaSize == null)
                {
                    m_RowsAreaSize = new MembersAreaSizeInfo();
                }
                return m_RowsAreaSize;
            }
            set
            {
                m_RowsAreaSize = value;
            }
        }

        public MemberSizeInfo GetColumnSizeInfoByUniqueName(String uniqueName)
        {
            if (String.IsNullOrEmpty(uniqueName))
                return null;

            foreach (MemberSizeInfo ci in ColumnsAreaSize.MembersSize)
            {
                if (ci.MemberUniqueName == uniqueName)
                {
                    return ci;
                }
            }
            return null;
        }

        public MemberSizeInfo GetRowSizeInfoByUniqueName(String uniqueName)
        {
            if (String.IsNullOrEmpty(uniqueName))
                return null;

            foreach (MemberSizeInfo ci in RowsAreaSize.MembersSize)
            {
                if (ci.MemberUniqueName == uniqueName)
                {
                    return ci;
                }
            }
            return null;
        }
    }

    public class MembersAreaSizeInfo
    {
        public List<MemberSizeInfo> MembersSize = new List<MemberSizeInfo>();
        public List<LineSizeInfo> LinesSize = new List<LineSizeInfo>();
    }

    public class MemberSizeInfo
    {
        public MemberSizeInfo()
        {
        }

        public MemberSizeInfo(String memberUniqueName, double size)
        {
            MemberUniqueName = memberUniqueName;
            Size = size;
        }

        public String MemberUniqueName = String.Empty;
        public double Size = double.MinValue;
    }

    public class LineSizeInfo
    {
        public LineSizeInfo()
        {
        }

        public LineSizeInfo(int lineIndex, double size)
        {
            LineIndex = lineIndex;
            Size = size;
        }

        public int LineIndex = -1;
        public double Size = double.MinValue;
    }
}
