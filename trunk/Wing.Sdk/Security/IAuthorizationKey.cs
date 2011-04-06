using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Wing.Security
{
    public interface IAuthorizationKey
    {
        IAuthorizationService Manager { get; }
        String AuthorizationKey { get; }
        String Caption { get; }
        int Order { get; }
        bool IsRoot { get; }
        IAuthorizationKey Parent { get; }
        ReadOnlyCollection<IAuthorizationKey> SubKeys { get; }
        bool IsUserGranted(String username);
    }
}
