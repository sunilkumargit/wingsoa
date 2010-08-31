/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Net;
using System.Collections.Generic;

namespace Wing.Olap.Core.Providers
{
    public class PivotDataProvider
    {
        public readonly CellSetDataProvider Provider;
        
        public PivotDataProvider(CellSetDataProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            Provider = provider;
        }

        MembersAreaInfo m_ColumnsArea = null;
        public MembersAreaInfo ColumnsArea
        {
            get {
                if (m_ColumnsArea == null)
                {
                    m_ColumnsArea = new MembersAreaInfo(AreaType.ColumnsArea);
                    m_ColumnsArea.Initialize(Provider.Columns);
                }
                return m_ColumnsArea;
            }
        }

        MembersAreaInfo m_RowsArea = null;
        public MembersAreaInfo RowsArea
        {
            get
            {
                if (m_RowsArea == null)
                {
                    m_RowsArea = new MembersAreaInfo(AreaType.RowsArea);
                    m_RowsArea.Initialize(Provider.Rows);
                }
                return m_RowsArea;
            }
        }



    }
}
