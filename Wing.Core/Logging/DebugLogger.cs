using System.Diagnostics;
namespace Wing.Logging
{
    public class DebugLogger : ILogger
    {
        private string FormatMessage(string message, Category category, Priority priority)
        {
            return string.Format("[{0} {1}] {2}", category.ToString(), priority.ToString(), message);
        }

        #region ILogger Members

        public void Log(string message, Category category, Priority priority)
        {
            Debug.WriteLine(FormatMessage(message, category, priority));
        }

        public void Log(string logName, string message, Category category, Priority priority)
        {
            Debug.WriteLine("[{0}] {1}", logName, FormatMessage(message, category, priority));
        }

        public void LogException(string logName, string message, System.Exception exception, Priority priority)
        {
            Debug.Assert(false, "Exception", "[{0}] {1}", logName, FormatMessage(message, Category.Exception, priority));
        }

        public void LogException(string message, System.Exception exception, Priority priority)
        {
            Debug.Assert(false, "Exception", "{0}", FormatMessage(message, Category.Exception, priority));
        }

        #endregion
    }
}
