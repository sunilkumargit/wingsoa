/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Security.Principal;

namespace Wing.Olap.Core.Storage
{
    public interface IStorageProvider
    {
        void Save(IPrincipal currentPrincipal, String name, String content);
        String Load(IPrincipal currentPrincipal, String name);
    }
}
