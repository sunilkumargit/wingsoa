using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Security
{
    public interface IRole
    {
        String Name { get; }
        String Group { get; set; }
    }
}
