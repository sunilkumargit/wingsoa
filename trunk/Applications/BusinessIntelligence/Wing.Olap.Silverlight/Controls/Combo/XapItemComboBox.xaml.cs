/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Resources;
using System.Xml.Linq;
using Wing.Olap.Controls.General.ItemControls;

namespace Wing.Olap.Controls.Combo
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
