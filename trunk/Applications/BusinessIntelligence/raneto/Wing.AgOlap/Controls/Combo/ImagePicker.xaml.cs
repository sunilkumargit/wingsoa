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
using System.Windows.Media.Imaging;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Collections;
using System.IO;

namespace Ranet.AgOlap.Controls.Combo
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
