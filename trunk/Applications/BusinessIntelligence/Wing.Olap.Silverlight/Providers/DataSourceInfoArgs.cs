/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;

namespace Wing.Olap.Providers
{
    public class DataSourceInfoArgs
    {
        public String ConnectionString = String.Empty;

        public String MDXQuery = String.Empty;
        public String Parsed_MDXQuery = String.Empty;

        public String UpdateScript = String.Empty;
        public String Parsed_UpdateScript = String.Empty;

        public String MovedAxes_MDXQuery = String.Empty;
        public String Parsed_MovedAxes_MDXQuery = String.Empty;

        public DataSourceInfoArgs()
        { 
        }
    }
}
