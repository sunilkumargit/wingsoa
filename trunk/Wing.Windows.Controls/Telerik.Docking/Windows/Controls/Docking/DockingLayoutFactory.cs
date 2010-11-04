namespace Telerik.Windows.Controls.Docking
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Xml;
    using Telerik.Windows.Controls;

    internal class DockingLayoutFactory
    {
        private static Converter<string, object> boolParser = s => ParseBool(s);
        private RadDocking docking;
        private static Converter<string, object> doubleParser = s => ParseDouble(s);
        private static Converter<string, object> identityParser = s => s;
        private static Converter<string, object> intParser = s => ParseInt(s);

        public DockingLayoutFactory(RadDocking docking)
        {
            this.docking = docking;
        }

        public void CleanUpLayout()
        {
            foreach (RadSplitContainer container in this.docking.SplitContainers.ToArray<RadSplitContainer>())
            {
                this.CleanUpLayout(container);
            }
            RadSplitContainer documentHost = this.docking.DocumentHost as RadSplitContainer;
            if (documentHost != null)
            {
                this.CleanUpLayout(documentHost);
            }
        }

        public void CleanUpLayout(RadPane pane)
        {
            this.docking.NotifyElementCleaning(pane);
            pane.RemoveFromParent();
            this.docking.NotifyElementCleaned(pane);
        }

        public void CleanUpLayout(RadPaneGroup group)
        {
            this.docking.NotifyElementCleaning(group);
            group.RemoveFromParent();
            foreach (RadPane pane in group.EnumeratePanes().ToArray<RadPane>())
            {
                this.CleanUpLayout(pane);
            }
            this.docking.NotifyElementCleaned(group);
        }

        public void CleanUpLayout(RadSplitContainer container)
        {
            this.docking.NotifyElementCleaning(container);
            container.RemoveFromParent();
            foreach (ISplitItem item in container.Items.Cast<ISplitItem>().ToArray<ISplitItem>())
            {
                RadPaneGroup group = item as RadPaneGroup;
                RadSplitContainer childContainer = item as RadSplitContainer;
                if (group != null)
                {
                    this.CleanUpLayout(group);
                }
                else if (childContainer != null)
                {
                    this.CleanUpLayout(childContainer);
                }
            }
            this.docking.NotifyElementCleaned(container);
        }

        private static DependencyObject GetElementByTypeName(string elementTypeName)
        {
            if (elementTypeName == "RadDocking")
            {
                return new RadDocking();
            }
            if (elementTypeName == "RadSplitContainer")
            {
                return new RadSplitContainer();
            }
            if (elementTypeName == "RadPaneGroup")
            {
                return new RadPaneGroup();
            }
            if (elementTypeName == "RadPane")
            {
                return new RadPane();
            }
            if (elementTypeName != "RadDocumentPane")
            {
                throw new ArgumentOutOfRangeException("elementTypeName");
            }
            return new RadDocumentPane();
        }

        private static string GetElementTypeName(DependencyObject element)
        {
            if ((!(element is RadDocking) && !(element is RadSplitContainer)) && ((!(element is RadPaneGroup) && !(element is RadPane)) && !(element is ToolWindow)))
            {
                throw new ArgumentOutOfRangeException("element");
            }
            return element.GetType().Name.Replace("`", "____");
        }

        public void LoadDocking(XmlReader reader)
        {
            while ((reader.NodeType != XmlNodeType.Element) || (reader.Name != GetElementTypeName(this.docking)))
            {
                if (!reader.Read())
                {
                    return;
                }
            }
            int dockingDepth = reader.Depth;
            Dictionary<string, string> attributes = ReadAttributes(reader);
            if (attributes.ContainsKey("SerializationTag"))
            {
                this.docking.NotifyElementLoading(attributes["SerializationTag"]);
                RadDocking.SetSerializationTag(this.docking, attributes["SerializationTag"]);
            }
            while (reader.Read() && (reader.Depth > dockingDepth))
            {
                if ((reader.NodeType == XmlNodeType.Element) && ((reader.Depth - 1) == dockingDepth))
                {
                    if ((reader.Name == "DocumentHost") && reader.Read())
                    {
                        this.docking.DocumentHost = this.LoadSplitContainer(reader);
                    }
                    else if ((reader.Name == "SplitContainers") && !reader.IsEmptyElement)
                    {
                        int currentDepth = reader.Depth;
                        while (reader.Read() && (reader.Depth > currentDepth))
                        {
                            if ((reader.NodeType == XmlNodeType.Element) && ((reader.Depth - 1) == currentDepth))
                            {
                                this.docking.Items.Add(this.LoadSplitContainer(reader));
                            }
                        }
                    }
                }
            }
            this.docking.NotifyElementLoaded(this.docking);
        }

        private static void LoadFloatingLocation(RadSplitContainer container, Dictionary<string, string> attributes)
        {
            double floatingX = double.NaN;
            double floatingY = double.NaN;
            Point initialFloatingLocation = RadDocking.GetFloatingLocation(container);
            if (attributes.ContainsKey("FloatingX"))
            {
                floatingX = ParseDouble(attributes["FloatingX"]);
            }
            if (attributes.ContainsKey("FloatingY"))
            {
                floatingY = ParseDouble(attributes["FloatingY"]);
            }
            Point floatingLocation = new Point(double.IsNaN(floatingX) ? initialFloatingLocation.X : floatingX, double.IsNaN(floatingY) ? initialFloatingLocation.Y : floatingY);
            RadDocking.SetFloatingLocation(container, floatingLocation);
        }

        private static void LoadFloatingSize(RadSplitContainer container, Dictionary<string, string> attributes)
        {
            double floatingWidth = double.NaN;
            double floatingHeight = double.NaN;
            Size initialFloatingSize = RadDocking.GetFloatingSize(container);
            if (attributes.ContainsKey("FloatingWidth"))
            {
                floatingWidth = ParseDouble(attributes["FloatingWidth"]);
            }
            if (attributes.ContainsKey("FloatingHeight"))
            {
                floatingHeight = ParseDouble(attributes["FloatingHeight"]);
            }
            Size floatingSize = new Size(double.IsNaN(floatingWidth) ? initialFloatingSize.Width : floatingWidth, double.IsNaN(floatingHeight) ? initialFloatingSize.Height : floatingHeight);
            RadDocking.SetFloatingSize(container, floatingSize);
        }

        public RadPane LoadPane(XmlReader reader)
        {
            string elementName = reader.Name;
            Dictionary<string, string> attributes = ReadAttributes(reader);
            RadPane pane = null;
            if (attributes.ContainsKey("SerializationTag"))
            {
                pane = this.docking.NotifyElementLoading(attributes["SerializationTag"]) as RadPane;
            }
            if (pane == null)
            {
                pane = GetElementByTypeName(elementName) as RadPane;
            }
            if (pane == null)
            {
                throw new InvalidOperationException();
            }
            if (attributes.ContainsKey("SerializationTag"))
            {
                RadDocking.SetSerializationTag(pane, attributes["SerializationTag"]);
            }
            ReadProperty(attributes, pane, RadPane.IsPinnedProperty, "IsPinned", boolParser);
            ReadProperty(attributes, pane, RadPane.IsHiddenProperty, "IsHidden", boolParser);
            ReadProperty(attributes, pane, RadPane.TitleProperty, "Title", identityParser);
            ReadProperty(attributes, pane, HeaderedContentControl.HeaderProperty, "Header", identityParser);
            ReadProperty(attributes, pane, RadPane.CanUserCloseProperty, "CanUserClose", boolParser);
            ReadProperty(attributes, pane, RadPane.CanUserPinProperty, "CanUserPin", boolParser);
            ReadProperty(attributes, pane, RadPane.CanDockInDocumentHostProperty, "CanDockInDocumentHost", boolParser);
            ReadProperty(attributes, pane, RadPane.CanFloatProperty, "CanFloat", boolParser);
            ReadProperty(attributes, pane, RadPane.AutoHideWidthProperty, "AutoHideWidth", doubleParser);
            ReadProperty(attributes, pane, RadPane.AutoHideHeightProperty, "AutoHideHeight", doubleParser);
            if (attributes.ContainsKey("IsDockable"))
            {
                pane.IsDockable = bool.Parse(attributes["IsDockable"]);
            }
            this.docking.NotifyElementLoaded(pane);
            return pane;
        }

        public DependencyObject LoadPaneGroup(XmlReader reader)
        {
            int paneGroupDepth = reader.Depth;
            string elementName = reader.Name;
            Dictionary<string, string> attributes = ReadAttributes(reader);
            RadPaneGroup group = null;
            if (attributes.ContainsKey("SerializationTag"))
            {
                group = this.docking.NotifyElementLoading(attributes["SerializationTag"]) as RadPaneGroup;
            }
            if (group == null)
            {
                group = GetElementByTypeName(elementName) as RadPaneGroup;
            }
            if (group == null)
            {
                throw new InvalidOperationException();
            }
            ReadProperty(attributes, group, RadDocking.IsAutogeneratedProperty, "IsAutoGenerated", boolParser);
            if (attributes.ContainsKey("SerializationTag"))
            {
                RadDocking.SetSerializationTag(group, attributes["SerializationTag"]);
            }
            if (attributes.ContainsKey("RelativeWidth") && attributes.ContainsKey("RelativeHeight"))
            {
                ProportionalStackPanel.SetRelativeSize(group, new Size(ParseDouble(attributes["RelativeWidth"]), ParseDouble(attributes["RelativeHeight"])));
            }
            ReadProperty(attributes, group, ProportionalStackPanel.SplitterChangeProperty, "SplitterChange", doubleParser);
            while (reader.Read() && (reader.Depth > paneGroupDepth))
            {
                if (((reader.NodeType == XmlNodeType.Element) && (reader.Name == "Items")) && !reader.IsEmptyElement)
                {
                    int currentDepth = reader.Depth;
                    while (reader.Read() && (reader.Depth > currentDepth))
                    {
                        if ((reader.NodeType == XmlNodeType.Element) && ((reader.Depth - 1) == currentDepth))
                        {
                            group.Items.Add(this.LoadPane(reader));
                        }
                    }
                }
            }
            if (attributes.ContainsKey("SelectedIndex"))
            {
                group.SelectedIndex = Math.Max(-1, Math.Min(group.Items.Count - 1, ParseInt(attributes["SelectedIndex"])));
            }
            this.docking.NotifyElementLoaded(group);
            return group;
        }

        public RadSplitContainer LoadSplitContainer(XmlReader reader)
        {
            int splitContainerDepth = reader.Depth;
            string elementName = reader.Name;
            Dictionary<string, string> attributes = ReadAttributes(reader);
            RadSplitContainer container = null;
            if (attributes.ContainsKey("SerializationTag"))
            {
                container = this.docking.NotifyElementLoading(attributes["SerializationTag"]) as RadSplitContainer;
            }
            if (container == null)
            {
                container = GetElementByTypeName(elementName) as RadSplitContainer;
            }
            if (container == null)
            {
                throw new InvalidOperationException();
            }
            ReadProperty(attributes, container, RadDocking.IsAutogeneratedProperty, "IsAutoGenerated", boolParser);
            if (attributes.ContainsKey("SerializationTag"))
            {
                RadDocking.SetSerializationTag(container, attributes["SerializationTag"]);
            }
            if (container.ParentContainer == null)
            {
                if (attributes.ContainsKey("Dock"))
                {
                    DockState dock = ParseEnum<DockState>(attributes["Dock"]);
                    RadDocking.SetDockState(container, dock);
                    if (attributes.ContainsKey("Width"))
                    {
                        container.Width = ParseDouble(attributes["Width"]);
                    }
                    else if (attributes.ContainsKey("Height"))
                    {
                        container.Height = ParseDouble(attributes["Height"]);
                    }
                }
                else if (attributes.ContainsKey("InitialPosition"))
                {
                    DockState initialPosition = ParseEnum<DockState>(attributes["InitialPosition"]);
                    container.InitialPosition = initialPosition;
                    switch (container.InitialPosition)
                    {
                        case DockState.FloatingDockable:
                        case DockState.FloatingOnly:
                            LoadFloatingSize(container, attributes);
                            LoadFloatingLocation(container, attributes);
                            ReadProperty(attributes, container, RadSplitContainer.WindowZIndexProperty, "WindowZIndex", intParser);
                            ReadProperty(attributes, container, RadSplitContainer.IsInOpenWindowProperty, "IsInOpenWindow", boolParser);
                            break;
                    }
                }
            }
            if (attributes.ContainsKey("Orientation"))
            {
                container.Orientation = ParseEnum<Orientation>(attributes["Orientation"]);
            }
            if (attributes.ContainsKey("RelativeWidth") && attributes.ContainsKey("RelativeHeight"))
            {
                ProportionalStackPanel.SetRelativeSize(container, new Size(ParseDouble(attributes["RelativeWidth"]), ParseDouble(attributes["RelativeHeight"])));
            }
            ReadProperty(attributes, container, ProportionalStackPanel.SplitterChangeProperty, "SplitterChange", doubleParser);
            while (reader.Read() && (reader.Depth > splitContainerDepth))
            {
                if (((reader.NodeType == XmlNodeType.Element) && (reader.Name == "Items")) && !reader.IsEmptyElement)
                {
                    int currentDepth = reader.Depth;
                    while (reader.Read() && (reader.Depth > currentDepth))
                    {
                        if ((reader.NodeType == XmlNodeType.Element) && ((reader.Depth - 1) == currentDepth))
                        {
                            DependencyObject splitItem = this.LoadSplitItem(reader);
                            if (splitItem != null)
                            {
                                container.Items.Add(splitItem);
                            }
                        }
                    }
                }
            }
            this.docking.NotifyElementLoaded(container);
            return container;
        }

        public DependencyObject LoadSplitItem(XmlReader reader)
        {
            if (reader.Name == "RadSplitContainer")
            {
                return this.LoadSplitContainer(reader);
            }
            if (reader.Name == "RadPaneGroup")
            {
                return this.LoadPaneGroup(reader);
            }
            return null;
        }

        private static bool ParseBool(string s)
        {
            return bool.Parse(s);
        }

        private static double ParseDouble(string s)
        {
            return double.Parse(s, CultureInfo.InvariantCulture);
        }

        private static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        private static int ParseInt(string s)
        {
            return int.Parse(s, CultureInfo.InvariantCulture);
        }

        private static Dictionary<string, string> ReadAttributes(XmlReader reader)
        {
            Dictionary<string, string> result = new Dictionary<string, string>(reader.AttributeCount);
            if (reader.MoveToFirstAttribute())
            {
                result.Add(reader.Name, reader.Value);
                while (reader.MoveToNextAttribute() && (reader.NodeType == XmlNodeType.Attribute))
                {
                    result.Add(reader.Name, reader.Value);
                }
            }
            return result;
        }

        private static void ReadProperty(Dictionary<string, string> attributes, DependencyObject d, DependencyProperty property, string propertyName, Converter<string, object> parser)
        {
            if (attributes.ContainsKey(propertyName))
            {
                d.SetValue(property, parser(attributes[propertyName]));
            }
            else
            {
                d.ClearValue(property);
            }
        }

        public void SaveLayout(XmlWriter writer)
        {
            this.docking.NotifyElementSaving(this.docking);
            WriteStartElement(writer, this.docking);
            RadSplitContainer documentHostSplitContainer = this.docking.DocumentHost as RadSplitContainer;
            if (documentHostSplitContainer != null)
            {
                writer.WriteStartElement("DocumentHost");
                this.SaveLayout(writer, documentHostSplitContainer);
                writer.WriteEndElement();
            }
            writer.WriteStartElement("SplitContainers");
            foreach (RadSplitContainer container in this.docking.SplitContainers)
            {
                this.SaveLayout(writer, container);
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
            this.docking.NotifyElementSaved(this.docking);
        }

        public void SaveLayout(XmlWriter writer, RadPane pane)
        {
            this.docking.NotifyElementSaving(pane);
            WriteStartElement(writer, pane);
            WriteDependencyProperty(writer, pane, RadPane.IsPinnedProperty, "IsPinned");
            WriteDependencyProperty(writer, pane, RadPane.IsHiddenProperty, "IsHidden");
            WriteDependencyProperty(writer, pane, RadPane.IsDockableProperty, "IsDockable");
            WriteDependencyProperty(writer, pane, RadPane.TitleProperty, "Title");
            WriteDependencyProperty(writer, pane, HeaderedContentControl.HeaderProperty, "Header");
            WriteDependencyProperty(writer, pane, RadPane.CanUserCloseProperty, "CanUserClose");
            WriteDependencyProperty(writer, pane, RadPane.CanUserPinProperty, "CanUserPin");
            WriteDependencyProperty(writer, pane, RadPane.CanDockInDocumentHostProperty, "CanDockInDocumentHost");
            WriteDependencyProperty(writer, pane, RadPane.CanFloatProperty, "CanFloat");
            WriteDependencyProperty(writer, pane, RadPane.AutoHideWidthProperty, "AutoHideWidth");
            WriteDependencyProperty(writer, pane, RadPane.AutoHideHeightProperty, "AutoHideHeight");
            writer.WriteEndElement();
            this.docking.NotifyElementSaved(pane);
        }

        public void SaveLayout(XmlWriter writer, RadPaneGroup group)
        {
            this.docking.NotifyElementSaving(group);
            RadDocking.GetIsAutogenerated(group);
            WriteStartElement(writer, group);
            if (group.ReadLocalValue(ProportionalStackPanel.RelativeSizeProperty) != DependencyProperty.UnsetValue)
            {
                Size relativeSize = ProportionalStackPanel.GetRelativeSize(group);
                writer.WriteAttributeString("RelativeWidth", ToString(relativeSize.Width));
                writer.WriteAttributeString("RelativeHeight", ToString(relativeSize.Height));
            }
            WriteDependencyProperty(writer, group, RadDocking.IsAutogeneratedProperty, "IsAutoGenerated");
            WriteDependencyProperty(writer, group, ProportionalStackPanel.SplitterChangeProperty, "SplitterChange");
            writer.WriteAttributeString("SelectedIndex", ToString(group.SelectedIndex));
            writer.WriteStartElement("Items");
            foreach (object item in group.EnumeratePanes())
            {
                RadPane childPane = item as RadPane;
                if (childPane != null)
                {
                    this.SaveLayout(writer, childPane);
                }
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
            this.docking.NotifyElementSaved(group);
        }

        public void SaveLayout(XmlWriter writer, RadSplitContainer container)
        {
            this.docking.NotifyElementSaving(container);
            RadDocking.GetIsAutogenerated(container);
            WriteStartElement(writer, container);
            if (container.ParentContainer == null)
            {
                ToolWindow parentWindow = container.ParentToolWindow;
                if (parentWindow != null)
                {
                    writer.WriteAttributeString("InitialPosition", (RadDocking.CheckIsDockable(container.Parent) ? DockState.FloatingDockable : DockState.FloatingOnly).ToString());
                    writer.WriteAttributeString("FloatingWidth", ToString(parentWindow.Width));
                    writer.WriteAttributeString("FloatingHeight", ToString(parentWindow.Height));
                    writer.WriteAttributeString("FloatingX", ToString(parentWindow.HorizontalOffset));
                    writer.WriteAttributeString("FloatingY", ToString(parentWindow.VerticalOffset));
                    writer.WriteAttributeString("IsInOpenWindow", (parentWindow.Visibility == Visibility.Visible).ToString());
                    WriteDependencyProperty(writer, parentWindow, Canvas.ZIndexProperty, "WindowZIndex");
                }
                else if (!(container.Parent is DocumentHost))
                {
                    DockState dock = RadDocking.GetDockState(container);
                    writer.WriteAttributeString("Dock", dock.ToString());
                    if (((dock == DockState.DockedLeft) || (dock == DockState.DockedRight)) && !double.IsNaN(container.Width))
                    {
                        writer.WriteAttributeString("Width", ToString(container.Width));
                    }
                    else if (((dock == DockState.DockedTop) || (dock == DockState.DockedBottom)) && !double.IsNaN(container.Height))
                    {
                        writer.WriteAttributeString("Height", ToString(container.Height));
                    }
                }
            }
            if (container.ReadLocalValue(ProportionalStackPanel.RelativeSizeProperty) != DependencyProperty.UnsetValue)
            {
                Size relativeSize = ProportionalStackPanel.GetRelativeSize(container);
                writer.WriteAttributeString("RelativeWidth", ToString(relativeSize.Width));
                writer.WriteAttributeString("RelativeHeight", ToString(relativeSize.Height));
            }
            WriteDependencyProperty(writer, container, RadDocking.IsAutogeneratedProperty, "IsAutoGenerated");
            WriteDependencyProperty(writer, container, RadSplitContainer.OrientationProperty, "Orientation");
            WriteDependencyProperty(writer, container, ProportionalStackPanel.SplitterChangeProperty, "SplitterChange");
            writer.WriteStartElement("Items");
            foreach (object item in container.Items)
            {
                RadSplitContainer childContainer = item as RadSplitContainer;
                RadPaneGroup childGroup = item as RadPaneGroup;
                if (childGroup != null)
                {
                    this.SaveLayout(writer, childGroup);
                }
                else if (childContainer != null)
                {
                    this.SaveLayout(writer, childContainer);
                }
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
            this.docking.NotifyElementSaved(container);
        }

        private static string ToString(double v)
        {
            return v.ToString(CultureInfo.InvariantCulture);
        }

        private static string ToString(int v)
        {
            return v.ToString(CultureInfo.InvariantCulture);
        }

        private static void WriteDependencyProperty(XmlWriter writer, DependencyObject element, DependencyProperty property, string propertyName)
        {
            if (element.ReadLocalValue(property) != DependencyProperty.UnsetValue)
            {
                object obj = element.GetValue(property);
                if (obj != null)
                {
                    writer.WriteAttributeString(propertyName, obj.ToString());
                }
            }
        }

        private static void WriteStartElement(XmlWriter writer, DependencyObject element)
        {
            writer.WriteStartElement(GetElementTypeName(element));
            if (element.ReadLocalValue(RadDocking.SerializationTagProperty) != DependencyProperty.UnsetValue)
            {
                writer.WriteAttributeString("SerializationTag", RadDocking.GetSerializationTag(element));
            }
        }
    }
}

