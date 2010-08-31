/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Net;

namespace Wing.Olap.Providers
{
    public class PivotInitializeArgs
    {
        public PivotInitializeArgs()
        {
        }

        public String Connection = String.Empty;
        public String Query = String.Empty;
        public String UpdateScript = String.Empty;

    }
}
