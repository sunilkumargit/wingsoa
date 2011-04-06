using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Security.Model;

namespace Wing.Security.Impl
{
    class RoleImpl : IRole
    {
        public RoleImpl(RoleModel model)
        {
            Name = model.RoleName;
            Group = model.GroupName;
        }

        public string Name { get; private set; }
        public string Group { get; set; }
    }
}
