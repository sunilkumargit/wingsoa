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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Ranet.AgOlap.Controls.Buttons;
using Ranet.AgOlap.Controls.Forms;

namespace Ranet.AgOlap.Controls.General
{
    public class PropertiesDialog
    {
        public readonly FloatingDialog Dlg;
        public readonly PropertiesControl PropertiesControl;

        public PropertiesDialog()
        {
            Dlg = new FloatingDialog();
            Dlg.MinHeight = 150;
            Dlg.MinWidth = 200;

            Dlg.Height = 300;
            Dlg.Width = 400;

            Dlg.Caption = Localization.PropertiesDialog_Caption;

            Grid container = new Grid();
            container.Margin = new Thickness(5, 5, 5, 5);
            container.RowDefinitions.Add(new RowDefinition());
            container.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            PropertiesControl = new PropertiesControl();
            container.Children.Add(PropertiesControl);

            // Кнопки диалога
            StackPanel buttonsPanel = new StackPanel() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
            container.Children.Add(buttonsPanel);
            Grid.SetRow(buttonsPanel, 3);
            RanetButton OkButton = new RanetButton() { Width = 70, Height = 22, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 5, 0, 0) };
            OkButton.Content = "OK";
            OkButton.Click += (ev_sender, args) => { Dlg.Close(); };
            buttonsPanel.Children.Add(OkButton);

            Dlg.SetContent(container);
        }

        public void Show()
        {
            Dlg.Show();
        }

        public void Show(String dialogCaption)
        {
            Dlg.Caption = dialogCaption;
            Show();
        }
    }
}
