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
    public enum OperationTypes
    {
        And,
        Or
    }

    public class FilterOperation : FilterOperationBase
    {
        public OperationTypes Operation = OperationTypes.And;
        
        public FilterOperation() 
        { 
        }

        public FilterOperation(OperationTypes operation)
        {
            Operation = operation;           
        }
        
        List<FilterOperationBase> m_Children = new List<FilterOperationBase>();
        public List<FilterOperationBase> Children
        {
            get {
                return m_Children;
            }
        }


    }
}
