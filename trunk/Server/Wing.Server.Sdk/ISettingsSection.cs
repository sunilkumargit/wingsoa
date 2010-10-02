using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Server
{
    public interface ISettingsSection
    {
        String SectionName { get; }
        String GetString(String name);
        int GetInt(String name);
        bool GetBoolean(String name);
        DateTime GetDateTime(String name);
        void Write(String name, String value);
        void Write(String name, int value);
        void Write(String name, bool value);
        void Write(String name, DateTime dateTime);
    }
}
