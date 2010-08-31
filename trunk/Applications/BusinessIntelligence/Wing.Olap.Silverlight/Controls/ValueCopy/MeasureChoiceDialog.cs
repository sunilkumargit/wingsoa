﻿/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
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
using Wing.Olap.Controls.Forms;
using Wing.Olap.Controls.Buttons;

namespace Wing.Olap.Controls.ValueCopy
{
    public class MeasureChoiceDialog : IChoiceDialog
    {
        FloatingDialog m_Dialog = null;
        MeasureChoiceCtrl m_ChoiceControl = null;
        public MeasureChoiceCtrl ChoiceControl
        {
            get {
                return m_ChoiceControl;
            }
        }

        bool m_ChoiceControlIsInitialized = false;
        RanetButton OkButton;

        public MeasureChoiceDialog()
        {
            m_Dialog = new FloatingDialog();
            m_Dialog.Caption = Localization.MeasureChoice_Caption + "...";

            Grid PopUpLayoutRoot = new Grid();
            PopUpLayoutRoot.RowDefinitions.Add(new RowDefinition() { });
            PopUpLayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            // Контрол выбора
            m_ChoiceControl = new MeasureChoiceCtrl();
            m_ChoiceControl.Margin = new Thickness(5);
            m_ChoiceControl.ApplySelection += new EventHandler(m_ChoiceControl_ApplySelection);
            m_ChoiceControl.SelectedItemChanged += new EventHandler<Wing.Olap.Controls.General.ItemEventArgs>(m_ChoiceControl_SelectedItemChanged);

            // Кнопки OK и Cancel
            StackPanel buttonsPanel = new StackPanel() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
            buttonsPanel.Margin = new Thickness(5, 0, 5, 5);

            OkButton = new RanetButton() { Width = 75, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 0, 10, 0) };
            OkButton.Content = Localization.DialogButton_Ok;
            OkButton.Click += new RoutedEventHandler(OkButton_Click);
            buttonsPanel.Children.Add(OkButton);

            RanetButton CancelButton = new RanetButton() { Width = 75, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 0, 0, 0) };
            CancelButton.Content = Localization.DialogButton_Cancel;
            CancelButton.Click += new RoutedEventHandler(CancelButton_Click);
            buttonsPanel.Children.Add(CancelButton);

            PopUpLayoutRoot.Children.Add(m_ChoiceControl);
            PopUpLayoutRoot.Children.Add(buttonsPanel);
            Grid.SetRow(buttonsPanel, 1);

            m_Dialog.SetContent(PopUpLayoutRoot);
            m_Dialog.DialogClosed += new EventHandler<DialogResultArgs>(m_Dialog_DialogClosed);
            m_Dialog.Width = 500;
            m_Dialog.Height = 400;

            UpdateButtonsState();
        }

        void m_ChoiceControl_SelectedItemChanged(object sender, Wing.Olap.Controls.General.ItemEventArgs e)
        {
            UpdateButtonsState();
        }

        void UpdateButtonsState()
        {
            if (m_ChoiceControl != null)
            {
                OkButton.IsEnabled = m_ChoiceControl.IsReadyToSelection;
            }
        }

        void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            m_Dialog.Close(); 
        }

        void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Raise_DialogOk();
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

        void Raise_DialogOk()
        {
            EventHandler handler = DialogOk;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        void m_ChoiceControl_ApplySelection(object sender, EventArgs e)
        {
            Raise_DialogOk();
            m_Dialog.Close(); 
        }

        public object Tag = null;
        public object UserData = null;
    }
}
