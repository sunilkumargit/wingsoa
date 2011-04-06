using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Security
{
    public interface IAuthorizationService
    {
        bool IsUserAuthorized(String username, String authorizationKey);
        IAuthorizationKey CreateKey(String keyPath, String caption, int order = 0);
        IAuthorizationKey CreateKey(String parentKey, String key, String caption, int order = 0);
        IAuthorizationKey GetKey(String basePath = "");
        void SetRoleStatus(String rolename, String authorizationKey, AuthorizationStatus status);
        void SetUserStatus(String username, String authorizationKey, AuthorizationStatus status);
        AuthorizationStatus GetRoleStatus(String rolename, String authorizationKey);
        AuthorizationStatus GetUserStatus(String username, String authorizationKey);
    }
}
