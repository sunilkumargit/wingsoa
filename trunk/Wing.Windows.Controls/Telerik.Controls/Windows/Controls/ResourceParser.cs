namespace Telerik.Windows.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;
    using System.Xml;

    internal static class ResourceParser
    {
        private static List<string> assemblyParts = new List<string>();
        private const string NewXamlNamespace = "http://schemas.microsoft.com/client/2007";
        private const string OldXamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        private const string ResourceDictionaryXamlWithPrefix = "<ResourceDictionary xmlns=\"http://schemas.microsoft.com/client/2007\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" xmlns:{1}=\"{2}\"> <{0} x:Key=\"Key\"/> </ResourceDictionary>";
        private const string StyleXaml = "<Style xmlns=\"http://schemas.microsoft.com/client/2007\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" TargetType=\"{0}\" />";
        private const string StyleXamlWithPrefix = "<Style xmlns=\"http://schemas.microsoft.com/client/2007\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" xmlns:{1}=\"{2}\" TargetType=\"{0}\" />";
        private static string[] systemAssemblies = new string[] { "mscorlib.dll", "system.dll", "System.Core.dll", "System.Net.dll", "System.Windows.dll", "System.Windows.Browser.dll", "System.Xml.dll" };

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static ResourceParser()
        {
            foreach (AssemblyPart part in Deployment.Current.Parts)
            {
                assemblyParts.Add(part.Source);
            }
        }

        private static bool CheckElement(XmlReader reader)
        {
            string prefix = reader.Prefix;
            if (!string.IsNullOrEmpty(prefix))
            {
                return ((reader.Name.IndexOf(':') < 0) || CheckIfAssemblyIsLoaded(reader, prefix));
            }
            return true;
        }

        private static bool CheckIfAssemblyIsLoaded(XmlReader reader, string prefix)
        {
            string typeAssembly = GetAssemblyFromPrefix(reader, prefix);
            if (string.IsNullOrEmpty(typeAssembly))
            {
                return true;
            }
            string[] typeAssemblyParts = typeAssembly.Split(new char[] { ',' });
            if (typeAssemblyParts.Length > 1)
            {
                typeAssembly = typeAssemblyParts[0];
            }
            string assemblyName = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", new object[] { typeAssembly, "dll" });
            return ((assemblyParts.Contains(assemblyName) || (Array.IndexOf<string>(systemAssemblies, assemblyName) > -1)) || RadControl.IsInDesignMode);
        }

        private static string GetAssemblyFromPrefix(XmlReader reader, string prefix)
        {
            string fullNamespace = GetNameSpaceAndAssemblyFromPrefix(reader, prefix);
            int startIndex = fullNamespace.IndexOf('=') + 1;
            int length = fullNamespace.Length - startIndex;
            return fullNamespace.Substring(startIndex, length);
        }

        private static string GetKeyAttribute(XmlReader reader)
        {
            string key = reader.GetAttribute("Key", "http://schemas.microsoft.com/winfx/2006/xaml");
            if (string.IsNullOrEmpty(key) && (reader.LocalName == "Style"))
            {
                string targetType = reader.GetAttribute("TargetType", null);
                string prefix = null;
                int prefixIndex = targetType.IndexOf(':');
                if (prefixIndex > 0)
                {
                    prefix = targetType.Substring(0, prefixIndex);
                    string typeNamespace = GetNamespaceFromPrefix(reader, prefix);
                    int length = targetType.Length - (prefixIndex + 1);
                    string typeName = targetType.Substring(prefixIndex + 1, length);
                    return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", new object[] { typeNamespace, typeName });
                }
                Style style = XamlReader.Load(string.Format(CultureInfo.InvariantCulture, "<Style xmlns=\"http://schemas.microsoft.com/client/2007\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" TargetType=\"{0}\" />", new object[] { targetType })) as Style;
                if (style != null)
                {
                    key = style.TargetType.FullName;
                }
            }
            return key;
        }

        private static string GetNameSpaceAndAssemblyFromPrefix(XmlReader reader, string prefix)
        {
            return reader.LookupNamespace(prefix);
        }

        private static string GetNamespaceFromPrefix(XmlReader reader, string prefix)
        {
            string fullNamespace = GetNameSpaceAndAssemblyFromPrefix(reader, prefix);
            int startIndex = fullNamespace.IndexOf(':') + 1;
            int length = fullNamespace.IndexOf(';') - startIndex;
            return fullNamespace.Substring(startIndex, length);
        }

        private static bool IsStyleOrControlTemplateTargetTypeLoaded(XmlReader reader)
        {
            if (IsXamlElement<Style>(reader.Name, reader.NamespaceURI) || IsXamlElement<ControlTemplate>(reader.Name, reader.NamespaceURI))
            {
                string targetType = reader.GetAttribute("TargetType", null);
                if (string.IsNullOrEmpty(targetType))
                {
                    return true;
                }
                string prefix = null;
                int prefixIndex = targetType.IndexOf(':');
                if (prefixIndex > 0)
                {
                    prefix = targetType.Substring(0, prefixIndex);
                    return CheckIfAssemblyIsLoaded(reader, prefix);
                }
            }
            return true;
        }

        private static bool IsXamlElement<T>(string name, string ns)
        {
            if (!(name == typeof(T).Name))
            {
                return false;
            }
            if (!(ns == "http://schemas.microsoft.com/winfx/2006/xaml/presentation"))
            {
                return (ns == "http://schemas.microsoft.com/client/2007");
            }
            return true;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static ResourceDictionary Parse(Stream stream, bool checkTypes, List<string> keys)
        {
            string xaml;
            using (MemoryStream outputStream = new MemoryStream())
            {
                using (XmlWriter writer = XmlWriter.Create(outputStream))
                {
                    using (XmlReader reader = XmlReader.Create(new StreamReader(stream)))
                    {
                        ParseResources(reader, writer, checkTypes, keys);
                    }
                }
                outputStream.Seek(0L, SeekOrigin.Begin);
                xaml = new StreamReader(outputStream).ReadToEnd();
            }
            try
            {
                ContentControl control = XamlReader.Load(xaml) as ContentControl;
                return control.Resources;
            }
            catch
            {
                return null;
            }
        }

        private static void ParseElement(XmlReader reader, XmlWriter writer, bool checkTypes, List<string> keys)
        {
            if ((checkTypes && (reader.Depth == 1)) && !IsStyleOrControlTemplateTargetTypeLoaded(reader))
            {
                reader.Skip();
            }
            else if (((checkTypes && (reader.Depth == 1)) && (!IsXamlElement<Style>(reader.Name, reader.NamespaceURI) && !IsXamlElement<ControlTemplate>(reader.Name, reader.NamespaceURI))) && !CheckElement(reader))
            {
                reader.Skip();
            }
            else
            {
                bool isEmpty = reader.IsEmptyElement;
                if (reader.Depth == 0)
                {
                    if (!IsXamlElement<ResourceDictionary>(reader.LocalName, reader.NamespaceURI))
                    {
                        throw new InvalidOperationException("CanOnlyParseXAMLFilesWithResourceDictionaryAsTheRootElement");
                    }
                    writer.WriteStartElement("ContentControl", reader.NamespaceURI);
                    writer.WriteAttributes(reader, true);
                    writer.WriteStartElement("ContentControl.Resources");
                    if (isEmpty)
                    {
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                    }
                }
                else
                {
                    if (reader.Depth == 1)
                    {
                        string key = GetKeyAttribute(reader);
                        if (!string.IsNullOrEmpty(key))
                        {
                            keys.Add(key);
                        }
                    }
                    writer.WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
                    writer.WriteAttributes(reader, true);
                    if (isEmpty)
                    {
                        writer.WriteEndElement();
                    }
                }
            }
        }

        private static void ParseResources(XmlReader reader, XmlWriter writer, bool checkTypes, List<string> keys)
        {
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        ParseElement(reader, writer, checkTypes, keys);
                        break;

                    case XmlNodeType.Text:
                        writer.WriteString(reader.Value);
                        break;

                    case XmlNodeType.CDATA:
                        writer.WriteCData(reader.Value);
                        break;

                    case XmlNodeType.EntityReference:
                        writer.WriteEntityRef(reader.Name);
                        break;

                    case XmlNodeType.ProcessingInstruction:
                    case XmlNodeType.XmlDeclaration:
                        writer.WriteProcessingInstruction(reader.Name, reader.Value);
                        break;

                    case XmlNodeType.Comment:
                        writer.WriteComment(reader.Value);
                        break;

                    case XmlNodeType.DocumentType:
                        writer.WriteDocType(reader.Name, reader.GetAttribute("PUBLIC"), reader.GetAttribute("SYSTEM"), reader.Value);
                        break;

                    case XmlNodeType.Whitespace:
                    case XmlNodeType.SignificantWhitespace:
                        writer.WriteWhitespace(reader.Value);
                        break;

                    case XmlNodeType.EndElement:
                        writer.WriteFullEndElement();
                        if (reader.Depth == 0)
                        {
                            writer.WriteFullEndElement();
                        }
                        break;
                }
            }
        }
    }
}

