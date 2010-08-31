/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Wing.Olap.Controls.Combo
{
    public partial class ImageChoiceControl : UserControl
    {
        public ImageChoiceControl()
        {
            InitializeComponent();

            lblAssembly.Text = Localization.Assembly_Label;
            lblImages.Text = Localization.Images_Label;
            lblPreview.Text = Localization.ImagePreview_Label;

            ImagePreviewCtrl.Stretch = Stretch.None;
            ImagePreviewCtrl.Width = ImagePreviewCtrl.Height = 0;
            comboAssembly.ItemsComboBox.SelectionChanged += new SelectionChangedEventHandler(ItemsComboBox_SelectionChanged);
            ImagesList.SelectionChanged += new EventHandler<Wing.Olap.Controls.General.SelectionChangedEventArgs<ImageDescriptor>>(ImagesList_SelectionChanged);
        }

        void ImagesList_SelectionChanged(object sender, Wing.Olap.Controls.General.SelectionChangedEventArgs<ImageDescriptor> e)
        {
            BitmapImage img = null;
            if (e != null && e.NewValue != null)
            {
                ImagePreviewCtrl.Source = e.NewValue.Image;
                img = e.NewValue.Image;
            }
            else
            {
                ImagePreviewCtrl.Source = null;
                ImagePreviewCtrl.Width = ImagePreviewCtrl.Height = 0;
            }

            ImagePreviewCtrl.Width = img != null ? img.PixelWidth : 0;
            ImagePreviewCtrl.Height = img != null ? img.PixelHeight : 0;

            
            ImagePreviewCtrl.UpdateLayout();
            
        }

        void ItemsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshImagesList();
        }

        public void Initialize()
        {
            InitializeImagesList(null);
            if (!comboAssembly.Initialize())
            {
                RefreshImagesList();
            }
        }

        void RefreshImagesList()
        {
            Assembly asm = null;
            if (comboAssembly.CurrentObject != null)
            {
                asm = comboAssembly.CurrentObject.Tag as Assembly;
            }

            InitializeImagesList(asm);
        }

        public void InitializeImagesList(Assembly assembly)
        {
            ImagesList.IsWaiting = true;
            List<ImageDescriptor> list = new List<ImageDescriptor>();
            list.Add(ImageDescriptor.Empty);
            try
            {
                if (assembly != null)
                {
                    String asm = String.Empty;
                    if(!String.IsNullOrEmpty(assembly.FullName))
                    {
                        asm = assembly.FullName;
                        if (asm.IndexOf(",") > -1)
                        {
                            asm = asm.Substring(0, asm.IndexOf(","));
                        }
                    }
                    
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

                                            String uri = UriResources.GetResourceString(asm, enumerator.Key.ToString());
                                            try
                                            {
                                                image.UriSource = new Uri(uri, UriKind.Relative);
                                            }
                                            catch { }
                                            /* через поток (но тогда Uri неизвестен)
                                            var stream = enumerator.Value as Stream;
                                            if (stream != null)
                                            {
                                                image.SetSource(stream);
                                            }*/
                                            list.Add(new ImageDescriptor(image, enumerator.Key.ToString(), uri));
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
            finally 
            {
                ImagesList.Initialize(list, list[0]);
                ImagesList.IsWaiting = false;
            }
        }

        public ImageDescriptor CurrentObject
        {
            get {
                return ImagesList.CurrentObject;
            }
        }
    }
}
