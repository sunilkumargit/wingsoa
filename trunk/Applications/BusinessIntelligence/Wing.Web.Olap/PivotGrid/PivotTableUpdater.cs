/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Ranet.UILibrary.Olap
 
    Ranet.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Ranet.UILibrary.Olap.  If not, see <http://www.gnu.org/licenses/>.
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Ranet.UILibrary.Olap under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading;
using Microsoft.AnalysisServices.AdomdClient;
using System.Text.RegularExpressions;
using Ranet.Web.Olap;
using Ranet.Olap.Core.Providers.ClientServer;

namespace Ranet.Olap.Web.PivotGrid
{
    public class PivotTableUpdater
    {
        private const uint MD_MASK_ENABLED = 0x00000000;
        private const uint MD_MASK_NOT_ENABLED = 0x10000000;
        private enum UpdateStatus : uint
        {
            CELL_UPDATE_ENABLED = 0x00000001,
            CELL_UPDATE_ENABLED_WITH_UPDATE = 0x00000002,
            CELL_UPDATE_NOT_ENABLED_FORMULA = 0x10000001,
            CELL_UPDATE_NOT_ENABLED_NONSUM_MEASURE = 0x10000002,
            CELL_UPDATE_NOT_ENABLED_NACELL_VIRTUALCUBE = 0x10000003,
            CELL_UPDATE_NOT_ENABLED_SECURE = 0x10000005,
            CELL_UPDATE_NOT_ENABLED_CALCLEVEL = 0x10000006,
            CELL_UPDATE_NOT_ENABLED_CANNOTUPDATE = 0x10000007,
            CELL_UPDATE_NOT_ENABLED_INVALIDDIMENSIONTYPE = 0x10000009,
            CELL_UPDATE_UNKNOWN = 0xFFFFFFFF
        }

        private const string UPDATE_COMMAND_TEMPLATE = @"UPDATE CUBE [{0}]
SET ";

        internal static String UpdateSync(String updeteScript, UpdateCubeArgs args)
        {
            String result = String.Empty;

            string cubeName = args.CubeName;
            string connectionString = args.ConnectionString;
            string commandText = string.Empty;

            if (string.IsNullOrEmpty(updeteScript))
            {
                commandText = string.Format(UPDATE_COMMAND_TEMPLATE, args.CubeName);
            }

            List<string> commands = new List<string>();

            for (int i = 0; i < args.Entries.Count; i++)
            {
                UpdateEntry entry = args.Entries[i];
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

            try
            {
                try
                {
                    //bool error = false;

                    foreach (string cmdText in commands)
                    {
                        try
                        {
                            AdomdConnection conn = AdomdConnectionPool.GetConnection(connectionString);
                            {
                                using (AdomdCommand cmd = new AdomdCommand(cmdText, conn))
                                {
                                    result = cmdText;
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                        catch (Exception exc)
                        {
                            //error = true;
                            throw exc;
                        }
                    }
                }
                finally
                {
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return result;
        }

        internal static void ParseHierarchies(ref string script, List<ShortMemberInfo> dr, object newValue, object oldValue)
        {
            Regex r = new Regex(@"<%([^%<>]+)%>", RegexOptions.Compiled);
            Match m = r.Match(script);
            while (m.Success)
            {
                Group g = m.Groups[1];
                string name = g.ToString();
                string value = string.Empty;
                //if (dr.Table.Columns.Contains(name))

                if (name == "newValue" || name == "oldValue")
                {
                    if (name == "newValue")
                    {
                        if (newValue != null)
                        {
                            value = newValue.ToString();
                        }
                        else
                        {
                            value = String.Empty;
                        }
                    }
                    if (name == "oldValue")
                    {
                        if (oldValue != null && !Convert.IsDBNull(oldValue) && !string.Empty.Equals(oldValue))
                        {
                            value = Convert.ToString(oldValue);
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

                script = script.Substring(0, m.Index) + value + script.Substring(m.Index + m.Length);
                m = r.Match(script);
            }
        }

        private delegate void WriteExceptionInvoker(Exception exc, bool checkInner);

        private delegate void WriteLineInvoker2(string fmt, params object[] par);
        private delegate void WriteLineInvoker1(string fmt);
        private static void LogQuery(IServiceProvider services, string query)
        {
        }

        private static void AppendAxis(StringBuilder sb, Axis filterAxis)
        {
            foreach (Tuple tuple in filterAxis.Set.Tuples)
            {
                foreach (Member member in tuple.Members)
                {
                    if (sb.Length != 0)
                    {
                        sb.Append(',');
                    }
                    sb.Append(member.UniqueName);
                }
            }
        }

        public static void Commit(/*IBindingContext context, */string connectionString)
        {
            ExecuteCommand("COMMIT TRANSACTION", connectionString/*, context, Localization.Msg_CommitChanges*/);
        }

        public static void Rollback(/*IBindingContext context, */string connectionString)
        {
            ExecuteCommand("ROLLBACK TRANSACTION", connectionString/*, context, Localization.Msg_RollbackChanges*/);
        }

        private static void ExecuteCommand(string commandText, string connectionString/*, IBindingContext context, string actionName*/)
        {
            {
                try
                {
                    AdomdConnection conn = AdomdConnectionPool.GetConnection(connectionString);
                    using (AdomdCommand cmd = new AdomdCommand(commandText, conn))
                    {
                        cmd.Execute();
                    }
                }
                catch
                {
                }
                finally
                {
                }
            }
        }

    }
}
