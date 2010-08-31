/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;

namespace Wing.Olap.Core.Providers
{
    public class OlapMetadataResponseException : Exception
    {
        public OlapMetadataResponseException()
        { 
        }

        public OlapMetadataResponseException(String message)
            : base(message)
        { 
        }
    }
}
