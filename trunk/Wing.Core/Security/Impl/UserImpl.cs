using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Security.Model;
using Wing.EntityStore;

namespace Wing.Security.Impl
{
    class UserImpl : IUser
    {
        public UserImpl(ISchema schema, UserModel model)
        {
            Schema = schema;
            Login = model.Login;
            Name = model.Name;
            Email = model.Email;
            Active = model.Active;
            PasswordHash = model.PasswordHash;
        }


        public ISchema Schema { get; private set; }
        public string Login { get; private set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; }
        public String PasswordHash { get; private set; }

        public void SetPassword(string password)
        {
            if (password.IsEmpty())
                PasswordHash = "";
            else
                PasswordHash = CryptographyHelper.EncodeToHex(password);
        }

        public bool IsValidPassword(string password)
        {
            return PasswordHash.IsEmpty() ?
                password.IsEmpty() :
                PasswordHash.Equals(CryptographyHelper.EncodeToHex(password));
        }
    }
}
