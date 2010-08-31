/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Olap.Core.Metadata
{
    public enum MetadataQueryType
    {
        GetCubes,
        GetMeasures,
        GetMeasure,
        GetKPIs,
        GetLevels,
        GetLevel,
        GetDimensions,
        GetHierarchies,
        GetDimension,
        GetHierarchy,
        GetLevelProperties,
        GetKPI,
        GetCubeMetadata,
        GetCubeMetadata_AllMembers,
        GetMeasureGroups
    }
}
