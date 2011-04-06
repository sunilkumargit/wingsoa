using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Security
{
    public interface IAccountService
    {
        IUser GetUser(String login);
        IRole GetRole(String roleName);
        ISchema GetSchema(String schemaId);

        IUser[] GetUsers();
        IRole[] GetRoles();
        ISchema[] GetSchemas();

        IUser[] GetUsersInSchema(String schemaId);
        IUser[] GetUsersInRole(String roleName);

        IRole[] GetRolesForUser(String login);
        IRole[] GetRolesInSchema(String schemaId);

        ISchema CreateSchema(String schemaId, String name);
        IRole CreateRole(String roleName, String group);
        IUser CreateUser(ISchema schema, String login, String name, String email);

        void AddUserInRole(String userName, String roleName);
        void RemoveUserFromRole(String userName, String roleName);

        void AddRoleInSchema(String roleName, String schemaId);
        void RemoveRoleFromSchema(String roleName, String schemaId);

        bool IsUserInRole(String login, String roleName);
        bool IsRoleInSchema(String roleName, String schemaId);
    }
}
