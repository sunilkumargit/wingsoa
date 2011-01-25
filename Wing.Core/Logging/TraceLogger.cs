//===================================================================================
// Microsoft patterns & practices
// Composite Application Guidance for Windows Presentation Foundation and Silverlight
//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===================================================================================
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
//===================================================================================
using System.Diagnostics;
using System;

namespace Wing.Logging
{
    /// <summary>
    /// Implementation of <see cref="ILogger"/> that logs to .NET <see cref="Trace"/> class.
    /// </summary>
    public class TraceLogger : ILogger
    {
        public TraceLogger(String name)
        {
            Assert.EmptyString(name, "name");
            Name = name;
        }

        public String Name { get; private set; }

        /// <summary>
        /// Write a new log entry with the specified category and priority.
        /// </summary>
        /// <param name="message">Message body to log.</param>
        /// <param name="category">Category of the entry.</param>
        /// <param name="priority">The priority of the entry.</param>
        public void Log(string message, Category category, Priority priority)
        {
            var formattedMessage = string.Format("[{0}, {1}] {2}", 
                category.ToString(), priority.ToString(), message);

            switch (category)
            {
                case Category.Debug:
                case Category.Info:
                    Trace.TraceInformation(formattedMessage);
                    break;
                case Category.Warn:
                    Trace.TraceWarning(formattedMessage);
                    break;
                case Category.Exception:
                    Trace.TraceError(formattedMessage);
                    break;
            }
        }

        public void LogException(string message, System.Exception exception, Priority priority)
        {
            Log(string.Format("{0}  Exception: {1}  Stack: {2}", message, exception.ToString(), exception.StackTrace), Category.Exception, priority);
        }
    }
}