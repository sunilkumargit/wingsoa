/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Ranet.UILibrary.Olap
 
    Ranet.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Ranet.UILibrary.Olap.  If not, see
  	<http://www.gnu.org/licenses/> 
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Ranet.UILibrary.Olap under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Resources;
using System.Xml.Linq;
using System.Reflection;
using Ranet.AgOlap.Controls.General.ItemControls;

namespace Ranet.AgOlap.Controls.Combo
{
    public partial class XapItemComboBox : UserControl
    {
        public XapItemComboBox()
        {
            InitializeComponent();
        }

        bool m_XapIsLoaded = false;
        public bool Initialize()
        {
            if (!m_XapIsLoaded)
            {
                ItemsComboBox.Items.Clear();
                ItemsComboBox.Items.Add(new NoneItemControl());
                ItemsComboBox.SelectedIndex = 0;

                DownloadXap();
                return true;
            }
            return false;
        }

        private void DownloadXap()
        {
            WebClient downloader = new WebClient();
            downloader.OpenReadCompleted += new OpenReadCompletedEventHandler(downloader_OpenReadCompleted);
            downloader.OpenReadAsync(Application.Current.Host.Source);
        }

        void downloader_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                m_XapIsLoaded = true;
                ItemsComboBox.Items.Clear();
                ItemsComboBox.Items.Add(new NoneItemControl());

                string appManifest = new StreamReader(Application.GetResourceStream(new StreamResourceInfo(e.Result, null), new Uri("AppManifest.xaml", UriKind.Relative)).Stream).ReadToEnd();

                XElement deploymentRoot = XDocument.Parse(appManifest).Root;
                List<XElement> deploymentParts = (from assemblyParts in deploymentRoot.Elements().Elements()
                                                  select assemblyParts).ToList();

                foreach (XElement xElement in deploymentParts)
                {
                    string source = xElement.Attribute("Source").Value;
                    AssemblyPart asmPart = new AssemblyPart();
                    StreamResourceInfo streamInfo = Application.GetResourceStream(new StreamResourceInfo(e.Result, "application/binary"), new Uri(source, UriKind.Relative));
                    var asm = asmPart.Load(streamInfo.Stream);
                    if (asm != null && asm.ManifestModule != null)
                    {
                        ItemsComboBox.Items.Add(new ItemControlBase(false) { Text = asm.ManifestModule.ToString(), Tag = asm });
                    }
                }

                if (ItemsComboBox.Items.Count > 0)
                {
                    ItemsComboBox.SelectedIndex = 0;
                }
            }
            catch 
            {
            }
        }

        public ItemControlBase CurrentObject
        {
            get
            {
                var item = ItemsComboBox.SelectedItem as ItemControlBase;
                if (item != null)
                {
                    return item;
                }
                return null;
            }
        }
    }
}
