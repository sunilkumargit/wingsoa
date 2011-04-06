using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Security
{
    public static class SecurityExtensionMethods
    {
        /*
         * Role
         * 
         */

        public static IUser[] GetUsers(this IRole role)
        {
            return ServiceLocator.GetInstance<IAccountService>()
                  .GetUsersInRole(role.Name);
        }

        public static void AddUser(this IRole role, String login)
        {
            ServiceLocator.GetInstance<IAccountService>().AddUserInRole(login, role.Name);
        }

        public static void AddUser(this IRole role, IUser user)
        {
            ServiceLocator.GetInstance<IAccountService>().AddUserInRole(user.Login, role.Name);
        }

        public static void RemoveUser(this IRole role, String login)
        {
            ServiceLocator.GetInstance<IAccountService>().RemoveUserFromRole(login, role.Name);
        }

        public static void RemoveUser(this IRole role, IUser user)
        {
            ServiceLocator.GetInstance<IAccountService>().RemoveUserFromRole(user.Login, role.Name);
        }

        public static bool IsUserInRole(this IRole role, String login)
        {
            return ServiceLocator.GetInstance<IAccountService>().IsUserInRole(login, role.Name);
        }

        public static bool IsUserInRole(this IRole role, IUser user)
        {
            return ServiceLocator.GetInstance<IAccountService>().IsUserInRole(user.Login, role.Name);
        }

        public static bool IsInSchema(this IRole role, String schemaId)
        {
            return ServiceLocator.GetInstance<IAccountService>().IsRoleInSchema(role.Name, schemaId);
        }

        public static bool IsInSchema(this IRole role, ISchema schema)
        {
            return ServiceLocator.GetInstance<IAccountService>().IsRoleInSchema(role.Name, schema.Id);
        }

        /*
         * User
         *
         */

        public static IRole[] GetRoles(this IUser user)
        {
            return ServiceLocator.GetInstance<IAccountService>()
                .GetRolesForUser(user.Name);
        }

        public static void AddInRole(this IUser user, String roleName)
        {
            ServiceLocator.GetInstance<IAccountService>()
                .AddUserInRole(user.Login, roleName);
        }


        public static void AddInRole(this IUser user, IRole role)
        {
            ServiceLocator.GetInstance<IAccountService>()
                .AddUserInRole(user.Login, role.Name);
        }

        public static void RemoveFromRole(this IUser user, String roleName)
        {
            ServiceLocator.GetInstance<IAccountService>()
                .RemoveUserFromRole(user.Login, roleName);
        }

        public static void RemoveFromRole(this IUser user, IRole role)
        {
            ServiceLocator.GetInstance<IAccountService>()
                .RemoveUserFromRole(user.Login, role.Name);
        }

        public static bool IsInRole(this IUser user, String roleName)
        {
            return ServiceLocator.GetInstance<IAccountService>().IsUserInRole(user.Login, roleName);
        }

        public static bool IsInRole(this IUser user, IRole role)
        {
            return ServiceLocator.GetInstance<IAccountService>().IsUserInRole(user.Login, role.Name);
        }

        /*
         * Schema
         * 
         */

        public static IUser[] GetUsers(this ISchema schema)
        {
            return ServiceLocator.GetInstance<IAccountService>().GetUsersInSchema(schema.Id);
        }

        public static IRole[] GetRoles(this ISchema schema)
        {
            return ServiceLocator.GetInstance<IAccountService>().GetRolesInSchema(schema.Id);
        }

        public static IUser CreateUser(this ISchema schema, String login, String name, String email)
        {
            return ServiceLocator.GetInstance<IAccountService>().CreateUser(schema, login, name, email);
        }

        public static void AddRole(this ISchema schema, String roleName)
        {
            ServiceLocator.GetInstance<IAccountService>().AddRoleInSchema(roleName, schema.Id);
        }

        public static void AddRole(this ISchema schema, IRole role)
        {
            ServiceLocator.GetInstance<IAccountService>().AddRoleInSchema(role.Name, schema.Id);
        }

        public static void RemoveRole(this ISchema schema, String roleName)
        {
            ServiceLocator.GetInstance<IAccountService>().RemoveRoleFromSchema(roleName, schema.Id);
        }

        public static void RemoveRole(this ISchema schema, IRole role)
        {
            ServiceLocator.GetInstance<IAccountService>().RemoveRoleFromSchema(role.Name, schema.Id);
        }

        public static bool IsRoleInSchema(this ISchema schema, String roleName)
        {
            return ServiceLocator.GetInstance<IAccountService>().IsRoleInSchema(roleName, schema.Id);
        }

        public static bool IsRoleInSchema(this ISchema schema, IRole role)
        {
            return ServiceLocator.GetInstance<IAccountService>().IsRoleInSchema(role.Name, schema.Id);
        }
    }
}
