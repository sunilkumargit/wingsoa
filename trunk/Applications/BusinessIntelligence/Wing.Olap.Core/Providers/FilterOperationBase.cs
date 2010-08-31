/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Net;
using System.Xml.Serialization;

namespace Wing.Olap.Core.Providers
{
    [XmlInclude(typeof(FilterOperand))]
    [XmlInclude(typeof(FilterOperation))]

    public class FilterOperationBase
    {
        public FilterOperationBase() { }
    }
}
