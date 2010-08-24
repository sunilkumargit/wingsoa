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
using Ranet.AgOlap.Controls.Forms;
using Ranet.AgOlap.Controls.Buttons;

namespace Ranet.AgOlap.Controls.ValueCopy
{
    public class MemberChoiceDialog : IChoiceDialog
    {
        FloatingDialog m_Dialog = null;
        MemberChoiceControl m_ChoiceControl = null;
        public MemberChoiceControl ChoiceControl
        {
            get {
                return m_ChoiceControl;
            }
        }

        Grid ContentRoot;
        RanetButton OkButton;
        RanetButton CancelButton;

        bool m_ChoiceControlIsInitialized = false;
        public MemberChoiceDialog()
        {
            m_Dialog = new FloatingDialog();
            m_Dialog.Caption = Localization.MemberChoice_Caption + "..."; 

            Grid LayoutRoot = new Grid();
            LayoutRoot.RowDefinitions.Add(new RowDefinition() { });
            LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            StackPanel buttonsPanel = new StackPanel() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
            buttonsPanel.Margin = new Thickness(5, 0, 5, 5);

            OkButton = new RanetButton() { Width = 75, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 0, 10, 0) };
            OkButton.Content = Localization.DialogButton_Ok;
            OkButton.Click += new RoutedEventHandler(OkButton_Click);
            buttonsPanel.Children.Add(OkButton);

            CancelButton = new RanetButton() { Width = 75, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 0, 0, 0) };
            CancelButton.Content = Localization.DialogButton_Cancel;
            CancelButton.Click += new RoutedEventHandler(CancelButton_Click);
            buttonsPanel.Children.Add(CancelButton);

            ContentRoot = new Grid();
            ContentRoot.Margin = new Thickness(5);
            LayoutRoot.Children.Add(ContentRoot);
            LayoutRoot.Children.Add(buttonsPanel);
            Grid.SetRow(buttonsPanel, 1);

            m_ChoiceControl = new MemberChoiceControl();
            m_ChoiceControl.ApplySelection += new EventHandler(m_ChoiceControl_ApplySelection);
            //m_ChoiceControl.CancelSelection += new EventHandler(m_ChoiceControl_CancelSelection);
            m_ChoiceControl.MultiSelect = false;
            ContentRoot.Children.Add(m_ChoiceControl);

            m_Dialog.SetContent(LayoutRoot);
            m_Dialog.DialogClosed += new EventHandler<DialogResultArgs>(m_Dialog_DialogClosed);
            m_Dialog.Width = 500;
            m_Dialog.Height = 400;
        }

        void OkButton_Click(object sender, RoutedEventArgs e)
        {
            ApplySelection();
        }

        void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            m_Dialog.Close();
        }

        public void Show()
        {
            m_Dialog.Show();
            if (!m_ChoiceControlIsInitialized)
            {
                m_ChoiceControl.Initialize();
                m_ChoiceControlIsInitialized = true;
            }
        }

        void m_Dialog_DialogClosed(object sender, DialogResultArgs e)
        {
            EventHandler handler = DialogCancel;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }            
        }

        public event EventHandler DialogOk;
        public event EventHandler DialogCancel;

        void m_ChoiceControl_ApplySelection(object sender, EventArgs e)
        {
            ApplySelection();
        }

        void ApplySelection()
        {
            EventHandler handler = DialogOk;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
            m_Dialog.Close();         
        }

        public object Tag = null;
        public object UserData = null;
    }
}
