using System;

namespace Wing.Mvc.Controls
{
    public class BindingException : Exception
    {
        public BindingException(String message, Exception innerException) : base(message, innerException) { }
    }
}
