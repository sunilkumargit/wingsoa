﻿using System;
using Wing.Logging;

namespace Wing.Bootstrap
{
    internal class ServerBootLoggerAdapter : ILogger
    {
        private IBootLogger _bootLogger;

        public String Name { get { return "SYSTEM"; } }

        public ServerBootLoggerAdapter(IBootLogger iBootLogger)
        {
            this._bootLogger = iBootLogger;
        }

        private string FormatString(string str, int length)
        {
            str = str ?? "";
            if (str.Length > length)
                return str.Substring(0, length);
            else if (str.Length < length)
                return str.PadRight(length, ' ');
            return str;
        }

        public void Log(string message, Category category, Priority priority)
        {
            _bootLogger.Log(String.Format("{0}|{1}: {2}",
                FormatString(category.ToString(), 9),
                FormatString(priority.ToString(), 6),
                message));
        }

        public void LogException(string message, Exception exception, Priority priority)
        {
            _bootLogger.Log(String.Format("{0}|{1}: {2}",
                            FormatString(Category.Exception.ToString(), 9),
                           FormatString(priority.ToString(), 6),
                           message), exception);
        }
    }
}
