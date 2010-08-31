/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System.Collections;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Wing.Olap.Controls.Combo
{
    public partial class ImagePicker : UserControl
    {
        public ImagePicker()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            ImageComboBox.Items.Clear();
        }

        public void Initialize(Assembly assembly)
        {
            ImageComboBox.Items.Clear();
            try
            {
                if (assembly != null)
                {
                    string[] resNames = assembly.GetManifestResourceNames();
                    if (resNames != null)
                    {
                        foreach (string resname in resNames)
                        {
                            ResourceManager rm = new ResourceManager(resname.Replace(".resources", ""), assembly);
                            // No delete next string !!!
                            Stream unreal = rm.GetStream(Application.Current.Host.Source.AbsoluteUri);
                            ResourceSet rs = rm.GetResourceSet(Thread.CurrentThread.CurrentUICulture, false, true);
                            if (rs != null)
                            {
                                IDictionaryEnumerator enumerator = rs.GetEnumerator();
                                while (enumerator.MoveNext())
                                {
                                    if (enumerator.Key != null && enumerator.Value != null)
                                    {
                                        if (enumerator.Key.ToString().Contains(".png"))
                                        {
                                            BitmapImage image = new BitmapImage();
                                            var stream = enumerator.Value as Stream;
                                            if (stream != null)
                                            {
                                                image.SetSource(stream);
                                            }
                                            ImageComboBox.Items.Add(new ImageItemControl(image, enumerator.Key.ToString()));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public ImageItemControl CurrentObject
        {
            get
            {
                return ImageComboBox.SelectedItem as ImageItemControl;
            }
        }
    }
}
