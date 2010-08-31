/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Net;
using Wing.Olap.Core.Metadata;
using Wing.Olap.Core.Providers.ClientServer;

namespace Wing.Olap.Controls.General.ClientServer
{
    public class MetadataQuery : OlapActionBase
    {
        public MetadataQuery()
        {
            ActionType = OlapActionTypes.GetMetadata;
        }

        public MetadataQueryType QueryType = MetadataQueryType.GetMeasures;

        public String Connection = String.Empty;
        public String CubeName = String.Empty;
        public String DimensionUniqueName = String.Empty;
        public String HierarchyUniqueName = String.Empty;
        public String LevelUniqueName = String.Empty;
        public String KPIName = String.Empty;
        public String MeasureUniqueName = String.Empty;
    }
}
