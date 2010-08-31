﻿
namespace Wing.Olap.Core.Providers.ClientServer
{
    public enum OlapActionTypes
    { 
        Unknown,
        ExportToExcel, 
        GetMetadata,
        StorageAction,
        ExecuteQuery
    }

    public class OlapActionBase
    {
        public OlapActionTypes ActionType = OlapActionTypes.Unknown;

        public OlapActionBase()
        { }
    }
}
