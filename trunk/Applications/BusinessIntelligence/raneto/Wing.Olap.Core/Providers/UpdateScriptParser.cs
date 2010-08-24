using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ranet.Olap.Core.Providers.ClientServer;
using System.Text.RegularExpressions;

namespace Ranet.Olap.Core.Providers
{
    public static class UpdateScriptParser
    {
        private const string UPDATE_COMMAND_TEMPLATE = @"UPDATE CUBE [{0}]
SET "; //({1}) = {2}
        public static List<string> GetUpdateScripts(
            string cubeName,
            string updeteScript,
            List<UpdateEntry> entries)
        {
            string commandText = string.Empty;

            if (string.IsNullOrEmpty(updeteScript))
            {
                commandText = string.Format(UPDATE_COMMAND_TEMPLATE, cubeName);
            }

            List<string> commands = new List<string>();

            for (int i = 0; i < entries.Count; i++)
            {
                UpdateEntry entry = entries[i];
                StringBuilder sb = new StringBuilder();
                foreach (ShortMemberInfo mi in entry.Tuple)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append(',');
                    }
                    sb.Append(mi.UniqueName);
                }
                string tuple = sb.ToString();
                sb = null;

                string lexeme = Environment.NewLine;
                if (i > 0)
                {
                    if (string.IsNullOrEmpty(updeteScript))
                    {
                        lexeme += ',';
                    }
                }

                if (string.IsNullOrEmpty(updeteScript))
                {
                    lexeme += string.Format("({0}) = {1}", tuple, entry.NewValue);
                    commandText += lexeme;
                }
                else
                {
                    string cmd = updeteScript;
                    ParseHierarchies(ref cmd, entry.Tuple, entry.NewValue, entry.OldValue);
                    lexeme += cmd;
                    commands.Add(lexeme);
                }

            }

            if (string.IsNullOrEmpty(updeteScript))
            {
                commands.Add(commandText);
            }

            return commands;
        }

        public static void ParseHierarchies(ref string script, List<ShortMemberInfo> dr, object newValue, object oldValue)
        {
            //Regex r = new Regex("\\$\\$([^\\$]+)\\$\\$", RegexOptions.None);
            Regex r = new Regex("<%([^%<>]+)%>", RegexOptions.None);
            Match m = r.Match(script);

            StringBuilder sb = new StringBuilder();
            int startIndex = 0;
            while (m.Success)
            {
                Group group = m.Groups[1];
                Capture capture = m.Captures[0];
                sb.Append(script.Substring(startIndex, capture.Index - startIndex));

                string name = group.ToString();
                string value = null;

                if (name == "newValue" || name == "oldValue")
                {
                    if (name == "newValue")
                    {
                        if (newValue != null)
                        {
                            value = newValue.ToString().Replace(',', '.');
                        }
                        else
                        {
                            value = string.Empty;
                        }
                    }
                    if (name == "oldValue")
                    {
                        if (oldValue != null && !Convert.IsDBNull(oldValue) && !string.Empty.Equals(oldValue))
                        {
                            value = Convert.ToString(oldValue).Replace(',', '.');
                        }
                        else
                        {
                            value = "null";
                        }
                    }
                }
                else
                {
                    foreach (ShortMemberInfo mi in dr)
                    {
                        if (mi.HierarchyUniqueName == name)
                        {
                            value = mi.UniqueName;
                            break;
                        }
                    }
                }
                if (value == null)
                {
                    sb.Append(capture.Value);
                }
                else
                {
                    sb.Append(value);
                }

                startIndex = capture.Index + capture.Length;
                m = m.NextMatch();
            }
            sb.Append(script.Substring(startIndex, script.Length - startIndex));
            script = sb.ToString();
        }
    }
}
