using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.EntityStore;
using Wing.Security.Model;
using Wing.Security.Impl;

namespace Wing.Security
{
    class AccountService : IAccountService
    {
        private IEntityStore _store;

        private Dictionary<String, IUser> _users = new Dictionary<string, IUser>();
        private Dictionary<String, IRole> _roles = new Dictionary<string, IRole>();
        private Dictionary<String, ISchema> _schemas = new Dictionary<string, ISchema>();
        private Dictionary<String, IRole[]> _rolesInSchema = new Dictionary<string, IRole[]>();
        private Dictionary<String, ISchema[]> _schemasForRole = new Dictionary<string, ISchema[]>();
        private Dictionary<String, IUser[]> _usersInRole = new Dictionary<string, IUser[]>();
        private Dictionary<String, IRole[]> _rolesForUser = new Dictionary<string, IRole[]>();

        private bool _allUsersLoaded = false;
        private bool _allRolesLoaded = false;
        private bool _allSchemasLoaded = false;

        public AccountService()
        {
            var _storeManager = ServiceLocator.GetInstance<IEntityStoreManager>();
            _store = _storeManager.CreateStore("accounts");
            _store.RegisterEntity<UserModel>();
            _store.RegisterEntity<RoleModel>();
            _store.RegisterEntity<SchemaModel>();
            _store.RegisterEntity<RoleSchemaRelation>();
            _store.RegisterEntity<UserRoleRelation>();
        }

        private IUser GetUserInternal(String login, bool throwIfNotExists)
        {
            Assert.EmptyString(login, "login");
            IUser result = null;
            login = login.ToLower();
            if (!_users.TryGetValue(login, out result))
            {
                var qry = _store.CreateQuery<UserModel>();
                qry.AddFilterEqual("Login", login);
                var model = qry.FindFirst();
                if (model != null)
                {
                    result = new UserImpl(GetSchemaInternal(model.SchemaId, true), model);
                    _users[login] = result;
                }
            }
            if (result == null && throwIfNotExists)
                throw new InvalidOperationException("Não existe um usuário com o login " + throwIfNotExists);

            return result;
        }

        private ISchema GetSchemaInternal(String schemaId, bool throwIfNotExists)
        {
            Assert.EmptyString(schemaId, "schemaId");
            ISchema result = null;
            schemaId = schemaId.ToLower();
            if (!_schemas.TryGetValue(schemaId, out result))
            {
                var qry = _store.CreateQuery<SchemaModel>();
                qry.AddFilterEqual("SchemaId", schemaId);
                var model = qry.FindFirst();
                if (model != null)
                {
                    result = new SchemaImpl(model);
                    _schemas[schemaId] = result;
                }
            }
            if (result == null && throwIfNotExists)
                throw new InvalidOperationException("Não existe um esquema de usuários com o id " + schemaId);
            return result;
        }

        public IRole GetRoleInternal(String roleName, bool throwIfNotExists)
        {
            IRole result = null;
            roleName = roleName.ToLower();
            if (!_roles.TryGetValue(roleName, out result))
            {
                var qry = _store.CreateQuery<RoleModel>();
                qry.AddFilterEqual("RoleName", roleName);
                var model = qry.FindFirst();
                if (model != null)
                {
                    result = new RoleImpl(model);
                    _roles[roleName] = result;
                }
            }
            if (result == null && throwIfNotExists)
                throw new InvalidOperationException(String.Format("Não existe um papel com o nome " + roleName));
            return result;
        }

        private Object __lockObject = new Object();
        private void Sync(Action action)
        {
            lock (__lockObject)
            {
                action.Invoke();
            }
        }


        public IUser GetUser(string login)
        {
            return GetUserInternal(login, false);
        }

        public IRole GetRole(string roleName)
        {
            return GetRoleInternal(roleName, false);
        }

        public ISchema GetSchema(string schemaId)
        {
            return GetSchemaInternal(schemaId, false);
        }

        public IUser[] GetUsers()
        {
            if (!_allUsersLoaded)
            {
                Sync(() =>
                {
                    if (_allUsersLoaded)
                        return;
                    _allUsersLoaded = true;
                    GetSchemas();
                    var qry = _store.CreateQuery<UserModel>();
                    var users = qry.Find();
                    IUser user = null;
                    foreach (var userModel in users)
                    {
                        if (!_users.TryGetValue(userModel.Login.ToLower(), out user))
                        {
                            user = new UserImpl(GetSchema(userModel.SchemaId), userModel);
                            _users[user.Login.ToLower()] = user;
                        }
                    }
                });
            }
            return _users.Values.OrderBy(u => u.Login).ToArray();
        }

        public IRole[] GetRoles()
        {
            if (!_allRolesLoaded)
            {
                Sync(() =>
                {
                    if (_allRolesLoaded)
                        return;
                    _allRolesLoaded = true;
                    var qry = _store.CreateQuery<RoleModel>();
                    var roles = qry.Find();
                    IRole role = null;
                    foreach (var roleModel in roles)
                    {
                        if (!_roles.TryGetValue(roleModel.RoleName.ToLower(), out role))
                        {
                            role = new RoleImpl(roleModel);
                            _roles[role.Name.ToLower()] = role;
                        }
                    }
                });
            }
            return _roles.Values.OrderBy(r => r.Name).ToArray();
        }

        public ISchema[] GetSchemas()
        {
            if (!_allSchemasLoaded)
            {
                Sync(() =>
                {
                    if (_allSchemasLoaded)
                        return;
                    _allSchemasLoaded = true;
                    var qry = _store.CreateQuery<SchemaModel>();
                    var schemas = qry.Find();
                    ISchema schema = null;
                    foreach (var schemaModel in schemas)
                    {
                        if (!_schemas.TryGetValue(schemaModel.SchemaId.ToLower(), out schema))
                        {
                            schema = new SchemaImpl(schemaModel);
                            _schemas[schema.Id.ToLower()] = schema;
                        }
                    }
                });
            }
            return _schemas.Values.OrderBy(s => s.Name).ToArray();
        }

        public IUser[] GetUsersInSchema(string schemaId)
        {
            return GetUsers().Where(u => u.Schema.Id.EqualsIgnoreCase(schemaId)).ToArray();
        }

        public IUser[] GetUsersInRole(string roleName)
        {
            var role = GetRoleInternal(roleName, true);
            IUser[] result = null;
            if (!_usersInRole.TryGetValue(role.Name, out result))
            {
                Sync(() =>
                {
                    if (!_usersInRole.TryGetValue(role.Name, out result))
                    {
                        var qry = _store.CreateQuery<UserRoleRelation>();
                        qry.AddFilterEqual("RoleName", role.Name);
                        var recs = qry.Find();
                        var list = new List<IUser>();
                        foreach (var rec in recs)
                            list.Add(GetUserInternal(rec.Login, false));
                        result = list.ToArray();
                        _usersInRole[role.Name] = result;
                    }
                });
            }
            return result;
        }

        public IRole[] GetRolesForUser(string login)
        {
            var user = GetUserInternal(login, true);
            IRole[] result = null;
            if (!_rolesForUser.TryGetValue(user.Login, out result))
            {
                Sync(() =>
                {
                    if (!_rolesForUser.TryGetValue(user.Login, out result))
                    {
                        var qry = _store.CreateQuery<UserRoleRelation>();
                        qry.AddFilterEqual("Login", user.Login);
                        var recs = qry.Find();
                        var list = new List<IRole>();
                        foreach (var rec in recs)
                            list.Add(GetRoleInternal(rec.RoleName, false));
                        result = list.ToArray();
                        _rolesForUser[user.Name] = result;
                    }
                });
            }
            return result;
        }

        public IRole[] GetRolesInSchema(string schemaId)
        {
            var schema = GetSchemaInternal(schemaId, true);
            IRole[] result = null;
            if (!_rolesInSchema.TryGetValue(schema.Id, out result))
            {
                Sync(() =>
                {
                    if (!_rolesInSchema.TryGetValue(schema.Id, out result))
                    {
                        var qry = _store.CreateQuery<RoleSchemaRelation>();
                        qry.AddFilterEqual("SchemaId", schema.Id);
                        var recs = qry.Find();
                        var list = new List<IRole>();
                        foreach (var rec in recs)
                            list.Add(GetRoleInternal(rec.RoleName, false));
                        result = list.ToArray();
                        _rolesInSchema[schema.Id] = result;
                    }
                });
            }
            return result;

        }

        public ISchema CreateSchema(string schemaId, string name)
        {
            Assert.EmptyString(schemaId, "schemaId");
            Assert.EmptyString(name, "name");
            ISchema schema = null;
            Sync(() =>
            {
                schema = GetSchemaInternal(schemaId, false);
                if (schema == null)
                {
                    var model = new SchemaModel()
                    {
                        SchemaId = schemaId,
                        Name = name
                    };
                    if (_store.Save(model))
                    {
                        schema = new SchemaImpl(model);
                        _schemas[schemaId.ToLower()] = schema;
                    }
                    else
                        throw new Exception("Não foi possivel criar o esquema de usuários - erro ao persistir as informações");
                }
                else
                    throw new Exception("Já existe um esquema de usuários com o id " + schemaId);
            });
            return schema;
        }

        public IRole CreateRole(string roleName, string group)
        {
            Assert.EmptyString(roleName, "roleName");
            IRole role = null;
            Sync(() =>
            {
                role = GetRoleInternal(roleName, false);
                if (role == null)
                {
                    var model = new RoleModel()
                    {
                        RoleName = roleName,
                        GroupName = group
                    };
                    if (_store.Save(model))
                    {
                        role = new RoleImpl(model);
                        _roles[roleName.ToLower()] = role;
                    }
                    else
                        throw new Exception("Não foi possivel criar o esquema de usuários - erro ao persistir as informações");
                }
                else
                    throw new Exception("Já existe um papel com o nome " + roleName);
            });
            return role;
        }

        public IUser CreateUser(ISchema schema, string login, string name, string email)
        {
            Assert.NullArgument(schema, "schema");
            Assert.EmptyString(login, "login");
            Assert.EmptyString(name, "name");
            Assert.EmptyString(email, "email");

            IUser user = null;
            Sync(() =>
            {
                user = GetUserInternal(login, false);
                if (user == null)
                {
                    var model = new UserModel()
                    {
                        Login = login,
                        Name = name,
                        Email = email,
                        SchemaId = schema.Id
                    };
                    if (_store.Save(model))
                    {
                        user = new UserImpl(schema, model);
                        _users[login.ToLower()] = user;
                    }
                    else
                        throw new Exception("Não foi possivel criar o esquema de usuários - erro ao persistir as informações");
                }
                else
                    throw new Exception("Já existe um usuário com o login " + login);
            });
            return user;
        }

        public void AddUserInRole(string login, string roleName)
        {
            var user = GetUserInternal(login, true);
            var role = GetRoleInternal(roleName, true);

            if (!role.IsInSchema(user.Schema))
                throw new Exception(String.Format("O papel {0} não faz parte do esquema ao qual o usuário {1} pertence.", roleName, login));

            Sync(() =>
            {
                if (!user.IsInRole(role))
                {
                    var relation = new UserRoleRelation()
                    {
                        Login = user.Login,
                        RoleName = role.Name
                    };
                    if (_store.Save(relation))
                    {
                        _usersInRole.Remove(role.Name);
                        _rolesForUser.Remove(user.Login);
                    }
                    else
                        throw new Exception("Não foi possivel adicionar o usuário ao papel.");
                }
            });
        }

        public void RemoveUserFromRole(string login, string roleName)
        {
            var user = GetUserInternal(login, true);
            var role = GetRoleInternal(roleName, true);
            Sync(() =>
            {
                if (user.IsInRole(role.Name))
                {
                    var qry = _store.CreateQuery<UserRoleRelation>();
                    qry.AddFilterEqual("Login", user.Login);
                    qry.AddFilterEqual("RoleName", role.Name);
                    var relation = qry.FindFirst();
                    if (relation != null)
                    {
                        if (_store.Remove(relation))
                        {
                            _usersInRole.Remove(role.Name);
                            _rolesForUser.Remove(user.Login);
                        }
                    }
                }
            });
        }

        public void AddRoleInSchema(string roleName, string schemaId)
        {
            var role = GetRoleInternal(roleName, true);
            var schema = GetSchemaInternal(schemaId, true);

            Sync(() =>
            {
                if (!role.IsInSchema(schema))
                {
                    var relation = new RoleSchemaRelation()
                    {
                        SchemaId = schema.Id,
                        RoleName = role.Name
                    };
                    if (_store.Save(relation))
                    {
                        _schemasForRole.Remove(role.Name);
                        _rolesInSchema.Remove(schema.Id);
                    }
                    else
                        throw new Exception("Não foi possivel adicionar o papel ao esquema do usuário.");
                }
            });
        }

        public void RemoveRoleFromSchema(string roleName, string schemaId)
        {
            var role = GetRoleInternal(roleName, true);
            var schema = GetSchemaInternal(schemaId, true);

            Sync(() =>
            {
                if (role.IsInSchema(schema.Id))
                {
                    var qry = _store.CreateQuery<RoleSchemaRelation>();
                    qry.AddFilterEqual("SchemaId", schema.Id);
                    qry.AddFilterEqual("RoleName", role.Name);
                    var relation = qry.FindFirst();
                    if (relation != null)
                    {
                        if (_store.Remove(relation))
                        {
                            _schemasForRole.Remove(role.Name);
                            _rolesInSchema.Remove(schema.Id);
                        }
                    }

                }
            });
        }

        public bool IsUserInRole(string login, string roleName)
        {
            Assert.EmptyString(login, "login");
            Assert.EmptyString(roleName, "roleName");
            return GetRolesForUser(login).Any(r => r.Name.EqualsIgnoreCase(roleName));
        }

        public bool IsRoleInSchema(string roleName, string schemaId)
        {
            Assert.EmptyString(roleName, "roleName");
            Assert.EmptyString(schemaId, "schemaId");
            return GetRolesInSchema(schemaId).Any(r => r.Name.EqualsIgnoreCase(roleName));
        }
    }
}
