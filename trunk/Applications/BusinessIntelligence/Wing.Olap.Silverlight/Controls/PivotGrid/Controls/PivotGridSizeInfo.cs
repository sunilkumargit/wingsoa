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
using System.Collections.Generic;

namespace Wing.Olap.Controls.PivotGrid.Controls
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
