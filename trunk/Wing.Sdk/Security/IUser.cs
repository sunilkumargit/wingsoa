using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Security
{
    public interface IUser
    {
        ISchema Schema { get; }
        String Login { get; }
        String Name { get; set; }
        String Email { get; set; }
        bool Active { get; set; }
        String PasswordHash { get; }
        void SetPassword(String password);
        bool IsValidPassword(String password);
    }
}
