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
using Wing.AgOlap.Controls.Buttons;

namespace Wing.AgOlap.Controls.General
{
    public class AgPopUpControlBase : AgControlBase
    {
        protected readonly PopUpContainerControl PopUpContainer = new PopUpContainerControl();

        RanetButton OkButton;
        RanetButton CancelButton;
        Grid ContentRoot;
        Grid LayoutRoot;
        Grid PopUpLayoutRoot;

        public AgPopUpControlBase()
        {
            this.Height = 22;
            this.VerticalAlignment = VerticalAlignment.Top;
            this.Loaded += new RoutedEventHandler(AgPopUpControlBase_Loaded);

            PopUpContainer.BeforePopUp += new EventHandler(OnBeforePopUp);

            PopUpLayoutRoot = new Grid();
            PopUpLayoutRoot.RowDefinitions.Add(new RowDefinition() { });
            PopUpLayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

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
            PopUpLayoutRoot.Children.Add(ContentRoot);
            PopUpLayoutRoot.Children.Add(buttonsPanel);
            Grid.SetRow(buttonsPanel, 1);

            LayoutRoot = new Grid();
            LayoutRoot.Children.Add(PopUpContainer);

            this.Content = LayoutRoot;
        }

        public new double Height
        {
            get { return Height; }
            set
            {
                base.Height = value;
                PopUpContainer.Height = value;
            }
        }

        void AgPopUpControlBase_Loaded(object sender, RoutedEventArgs e)
        {
            PopUpContainer.ContentControl = PopUpLayoutRoot;
        }

        protected virtual void OnBeforePopUp(object sender, EventArgs e)
        {
            
        }

        IChoiceControl m_ContentControl;
        public IChoiceControl ContentControl
        {
            get { return m_ContentControl; }
            set {
                m_ContentControl = value;
                ContentRoot.Children.Clear();
                if (value is UIElement)
                {
                    ContentRoot.Children.Add(value as UIElement);
                }
            }
        }

        protected void UpdateButtonsState()
        {
            if (m_ContentControl != null)
            {
                OkButton.IsEnabled = m_ContentControl.IsReadyToSelection;
            }
        }

        void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            CancelSelection();
        }

        void OkButton_Click(object sender, RoutedEventArgs e)
        {
            ApplySelection();
        }

        protected virtual void ApplySelection()
        {
            PopUpContainer.IsDropDownOpen = false;
        }

        protected virtual void CancelSelection()
        {
            PopUpContainer.IsDropDownOpen = false;
        }
    }
}
