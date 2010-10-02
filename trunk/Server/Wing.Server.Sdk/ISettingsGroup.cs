using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Server
{
    public interface ISettingsGroup
    {
        String GroupName { get; }
        String[] GetSectionsNames();
        ISettingsSection GetSection(String name, bool createIfNotExists = true);
    }
}