using System;

namespace Wing
{
    [System.Diagnostics.DebuggerStepThrough]
    public static class ExceptionHelper
    {
        public static String ToLineException(Exception ex)
        {
            String s = "";
            Exception e = ex;
            int c = 1;
            while (e != null)
            {
                s += " (" + c.ToString() + ") " + e.Message;
                e = e.InnerException;
                c++;
            }
            s = ex.TargetSite.ToString() + " -> " + s;
            return s;
        }

        public static void RaiseException(String message, Exception innerException)
        {
            throw new Exception(message, innerException);
        }

        public static void RaiseException(String message)
        {
            RaiseException(message, null);
        }

        public static TExceptionType NavigateInnerExceptions<TExceptionType>(Exception ex) where TExceptionType : Exception
        {
            while (ex != null && !ex.GetType().Equals(typeof(TExceptionType)))
                ex = ex.InnerException;
            return (TExceptionType)ex;
        }
    }
}
