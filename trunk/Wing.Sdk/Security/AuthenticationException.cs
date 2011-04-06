using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Security
{
    public class AuthenticationException : Exception
    {
        public AuthenticationException(String message, Exception ex = null) : base(message, ex) { }
    }
}
