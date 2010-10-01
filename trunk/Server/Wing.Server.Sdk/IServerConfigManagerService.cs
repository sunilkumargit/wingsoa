using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Server
{
    public interface IServerConfigManagerService
    {
        String[] GetSectionsNames();
        IServerConfigSection GetSection(String name, bool createIfNotExists = true);
    }
}