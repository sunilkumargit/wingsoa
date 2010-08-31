/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System.Collections.Generic;

namespace Wing.Olap.Core.Providers
{
    public interface IProperties
    {
        Dictionary<string, object> PropertiesDictionary { get; }
    }
}
