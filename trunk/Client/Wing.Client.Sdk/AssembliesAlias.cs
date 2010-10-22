using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Wing.Utils;

namespace Wing.Client.Sdk
{
    public class AssembliesAlias
    {
        private static Dictionary<String, List<String>> _assembliesAlias = new Dictionary<string, List<string>>();
        private static Dictionary<String, ReadOnlyCollection<String>> _assembliesAliasReadOnly = new Dictionary<string, ReadOnlyCollection<string>>();

        public static void RegisterAssemblyAlias(String alias, String assemblyName)
        {
            alias = alias.AsString().Trim().HasValue() ? alias.Trim() : ".";
            List<String> namesList = null;
            if (!_assembliesAlias.ContainsKey(alias))
            {
                namesList = new List<string>();
                _assembliesAlias[alias] = namesList;
                _assembliesAliasReadOnly[alias] = new ReadOnlyCollection<string>(namesList);
            }
            else
                namesList = _assembliesAlias[alias];
            if (!namesList.Contains(assemblyName))
                namesList.Add(assemblyName);
        }

        public static void RegisterAssemblyAliasOfType(String alias, Type type)
        {
            var assemblyName = type.Assembly.FullName;
            if (assemblyName.IndexOf(",") > 0)
                assemblyName = assemblyName.Substring(0, assemblyName.IndexOf(","));
            RegisterAssemblyAlias(alias, assemblyName);
        }

        public static ReadOnlyCollection<String> GetAssembliesInAlias(string alias)
        {
            alias = alias.AsString().Trim().HasValue() ? alias.Trim() : ".";
            if (_assembliesAlias.ContainsKey(alias))
                return _assembliesAliasReadOnly[alias];
            return null;
        }
    }
}
