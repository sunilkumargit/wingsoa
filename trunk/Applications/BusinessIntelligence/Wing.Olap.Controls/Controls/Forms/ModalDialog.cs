/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Wing.UILibrary.Olap
 
    Wing.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Wing.UILibrary.Olap.  If not, see
  	<http://www.gnu.org/licenses/> 
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Wing.UILibrary.Olap under a flexible commercial license
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
using Wing.AgOlap.Controls.Buttons;

namespace Wing.AgOlap.Controls.Forms
{
    public enum ModalDialogStyles
    { 
        OK,
        OKCancel
    }

    public class ModalDialog
    {
        public readonly FloatingDialog Dialog = null;

        public double MinWidth
        {
            get { return Dialog.MinWidth; }
            set { Dialog.MinWidth = value; }
        }

        public double MinHeight
        {
            get { return Dialog.MinHeight; }
            set { Dialog.MinHeight = value; }
        }
        
        public double Width
        {
            get { return Dialog.Width; }
            set { Dialog.Width = value; }
        }

        public double Height
        {
            get { return Dialog.Height; }
            set { Dialog.Height = value; }
        }
        
        public String Caption
        {
            get { return Dialog.Caption; }
            set { Dialog.Caption = value; }
        }

        RanetButton OkButton;
        RanetButton CancelButton;

        Grid gridContentContainer;
        public ModalDialog()
        {
            Dialog = new FloatingDialog();
            Dialog.Caption = String.Empty;

            Grid PopUpLayoutRoot = new Grid();
            PopUpLayoutRoot.RowDefinitions.Add(new RowDefinition() { });
            PopUpLayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            // Кнопки OK и Cancel
            StackPanel buttonsPanel = new StackPanel() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
            buttonsPanel.Margin = new Thickness(5, 0, 5, 5);

            OkButton = new RanetButton() { Width = 75, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0) };
            OkButton.Content = Localization.DialogButton_Ok;
            OkButton.Click += new RoutedEventHandler(OkButton_Click);
            buttonsPanel.Children.Add(OkButton);

            CancelButton = new RanetButton() { Width = 75, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(10, 0, 0, 0) };
            CancelButton.Content = Localization.DialogButton_Cancel;
            CancelButton.Click += new RoutedEventHandler(CancelButton_Click);
            buttonsPanel.Children.Add(CancelButton);

            gridContentContainer = new Grid();
            gridContentContainer.Margin = new Thickness(5);

            PopUpLayoutRoot.Children.Add(gridContentContainer);
            PopUpLayoutRoot.Children.Add(buttonsPanel);
            Grid.SetRow(buttonsPanel, 1);

            Dialog.SetContent(PopUpLayoutRoot);
            Dialog.DialogClosed += new EventHandler<DialogResultArgs>(m_Dialog_DialogClosed);
            Dialog.BeforeClosed += new EventHandler<DialogResultArgs>(m_Dialog_BeforeClosed);
            //m_Dialog.Width = 500;
            Dialog.MinWidth = OkButton.Width + CancelButton.Width + 20;
            //m_Dialog.Height = 400;
        }

        public event EventHandler<DialogResultArgs> BeforeClosed;
        void Raise_BeforeClosed(DialogResultArgs args)
        {
            EventHandler<DialogResultArgs> handler = BeforeClosed;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        void m_Dialog_BeforeClosed(object sender, DialogResultArgs e)
        {
            Raise_BeforeClosed(e);
        }

        ModalDialogStyles m_DialogStyle = ModalDialogStyles.OKCancel;
        public ModalDialogStyles DialogStyle
        {
            get{
                return m_DialogStyle;
            }
            set {
                m_DialogStyle = value;
                switch (value)
                {
                    case ModalDialogStyles.OK:
                        OkButton.Visibility = Visibility.Visible;
                        CancelButton.Visibility = Visibility.Collapsed;
                        break;
                    case ModalDialogStyles.OKCancel:
                        OkButton.Visibility = CancelButton.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        public UIElement Content
        {
            get
            {
                if (gridContentContainer.Children.Count > 0)
                    return gridContentContainer.Children[0];
                return null;
            }
            set
            {
                gridContentContainer.Children.Clear();
                gridContentContainer.Children.Add(value);
            }
        }

        void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Dialog.Close(DialogResult.Cancel); 
        }

        void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResultArgs args = Raise_DialogOk();
            if (args.Cancel)
                return;

            Dialog.Close(DialogResult.OK); 
        }

        public void Show()
        {
            Dialog.Show();
        }

        public void Close()
        {
            Dialog.Close();
        }

        void m_Dialog_DialogClosed(object sender, DialogResultArgs e)
        {
            EventHandler<DialogResultArgs> handler = DialogClosed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<DialogResultArgs> DialogOk;
        public event EventHandler<DialogResultArgs> DialogClosed;

        DialogResultArgs Raise_DialogOk()
        {
            EventHandler<DialogResultArgs> handler = DialogOk;
            DialogResultArgs args = new DialogResultArgs(DialogResult.OK);
            if (handler != null)
            {
                handler(this, args);
            }
            return args;
        }

        public object Tag = null;

        public void ListenKeys(bool value)
        {
            Dialog.ListenKeys(value);
        }
    }
}
