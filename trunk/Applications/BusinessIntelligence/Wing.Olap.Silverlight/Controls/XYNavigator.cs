﻿/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Windows;
using System.Windows.Controls;
using Wing.Olap.Controls.Buttons;
using Wing.Olap.Controls.General;

namespace Wing.Olap.Controls
{
    public enum AligmentType
    { 
        Bottom,
        Top,
        Left,
        Right,
        Center
    }

    public class XYNavigator : UserControl
    {
        bool m_ReverseVertical = false;
        public bool ReverseVertical
        {
            get { return m_ReverseVertical; }
            set { 
                m_ReverseVertical = value;
                RefreshButtonsAlignment();
            }
        }

        AligmentType m_ButtonsAlignment = AligmentType.Bottom;
        public AligmentType ButtonsAlignment
        {
            get { return m_ButtonsAlignment; }
            set { 
                m_ButtonsAlignment = value;
                RefreshButtonsAlignment();
            }
        }

        RanetButton m_UpButton;
        RanetButton m_LeftButton;
        RanetButton m_RightButton;
        RanetButton m_DownButton;

        Grid LayoutRoot;
        public XYNavigator()
        {
            LayoutRoot = new Grid();
            LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            m_UpButton = new RanetButton() { Width = 18, Height = 18 };
            m_UpButton.Content = UiHelper.CreateIcon(UriResources.Images.Up16);
            m_UpButton.Click += new RoutedEventHandler(m_UpButton_Click);

            m_DownButton = new RanetButton() { Width = 18, Height = 18 };
            m_DownButton.Content = UiHelper.CreateIcon(UriResources.Images.Down16);
            m_DownButton.Click += new RoutedEventHandler(m_DownButton_Click);

            m_LeftButton = new RanetButton() { Width = 18, Height = 18 };
            m_LeftButton.Content = UiHelper.CreateIcon(UriResources.Images.Left16);
            m_LeftButton.Click += new RoutedEventHandler(m_LeftButton_Click);

            m_RightButton = new RanetButton() { Width = 18, Height = 18 };
            m_RightButton.Content = UiHelper.CreateIcon(UriResources.Images.Right16);
            m_RightButton.Click += new RoutedEventHandler(m_RightButton_Click);

            RefreshButtonsAlignment();
            this.Content = LayoutRoot;
        }

        void RefreshButtonsAlignment()
        {
            LayoutRoot.Children.Clear();

            // По-умолчанию настройки для положения Center
            // Расположение:  
            //      Y++
            // X--       X++
            //      Y--    
            int down_col = 1;
            int down_row = 2;
            int up_col = 1;
            int up_row = 0;
            int left_col = 0;
            int left_row = 1;
            int right_col = 2;
            int right_row = 1;

            // Картинки для кнопок вверх/вниз
            if (ReverseVertical)
            {
                m_UpButton.Content = UiHelper.CreateIcon(UriResources.Images.Down16);
                m_DownButton.Content = UiHelper.CreateIcon(UriResources.Images.Up16);
            }
            else
            {
                m_UpButton.Content = UiHelper.CreateIcon(UriResources.Images.Up16);
                m_DownButton.Content = UiHelper.CreateIcon(UriResources.Images.Down16);
            }

            // Кнопки вверх/вниз
            switch (m_ButtonsAlignment)
            {
                case AligmentType.Center:
                    if (ReverseVertical)
                    {
                        up_row = 2;
                        down_row = 0;
                    }
                    else
                    {
                        up_row = 0;
                        down_row = 2;
                    }
                    break;
                case AligmentType.Bottom:
                    if (ReverseVertical)
                    {
                        up_row = 2;
                        down_row = 1;
                    }
                    else
                    {
                        up_row = 1;
                        down_row = 2;
                    }
                    break;
                case AligmentType.Top:
                    if (ReverseVertical)
                    {
                        up_row = 1;
                        down_row = 0;
                    }
                    else
                    {
                        up_row = 0;
                        down_row = 1;
                    }
                    break;
                case AligmentType.Left:
                    up_col = down_col = 0;
                    if (ReverseVertical)
                    {
                        up_row = 2;
                        down_row = 0;
                    }
                    else
                    {
                        up_row = 0;
                        down_row = 2;
                    }
                    break;
                case AligmentType.Right:
                    up_col = down_col = 2;
                    if (ReverseVertical)
                    {
                        up_row = 2;
                        down_row = 0;
                    }
                    else
                    {
                        up_row = 0;
                        down_row = 2;
                    }
                    break;
            }

            switch (m_ButtonsAlignment)
            {
                case AligmentType.Bottom:
                    left_row = 2;
                    right_row = 2;
                    break;
                case AligmentType.Top:
                    left_row = 0;
                    right_row = 0;
                    break;
                case AligmentType.Left:
                    left_col = 0;
                    right_col = 1;
                    break;
                case AligmentType.Right:
                    left_col = 1;
                    right_col = 2;
                    break;
            }

            LayoutRoot.Children.Add(m_UpButton);
            Grid.SetRow(m_UpButton, up_row);
            Grid.SetColumn(m_UpButton, up_col);

            LayoutRoot.Children.Add(m_DownButton);
            Grid.SetRow(m_DownButton, down_row);
            Grid.SetColumn(m_DownButton, down_col);

            LayoutRoot.Children.Add(m_LeftButton);
            Grid.SetRow(m_LeftButton, left_row);
            Grid.SetColumn(m_LeftButton, left_col);

            LayoutRoot.Children.Add(m_RightButton);
            Grid.SetRow(m_RightButton, right_row);
            Grid.SetColumn(m_RightButton, right_col);

            RefreshButtonsState();
        }

        void m_RightButton_Click(object sender, RoutedEventArgs e)
        {
            X_CurrentValue++;
        }

        void m_LeftButton_Click(object sender, RoutedEventArgs e)
        {
            X_CurrentValue--;
        }

        void m_DownButton_Click(object sender, RoutedEventArgs e)
        {
            Y_CurrentValue--;
        }

        void m_UpButton_Click(object sender, RoutedEventArgs e)
        {
            Y_CurrentValue++;
        }

        public event EventHandler CurrentValueChanged;

        void Raise_CurrentValueChanged()
        {
            EventHandler handler = CurrentValueChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        int m_X_MinValue = int.MinValue;
        public int X_MinValue
        {
            get { return m_X_MinValue; }
            set
            {
                m_X_MinValue = value;
                RefreshButtonsState();
            }
        }

        int m_X_MaxValue = int.MaxValue;
        public int X_MaxValue
        {
            get { return m_X_MaxValue; }
            set
            {
                m_X_MaxValue = value;
                RefreshButtonsState();
            }
        }

        int m_X_CurrentValue = 0;
        public int X_CurrentValue
        {
            get { return m_X_CurrentValue; }
            set
            {
                if (value != m_X_CurrentValue)
                {
                    int newValue = value;
                    if (X_MinValue <= value && value <= X_MaxValue)
                    {
                        newValue = value;
                    }
                    else
                    {
                        if (X_MinValue > value)
                        {
                            newValue = X_MinValue;
                        }
                        if (X_MaxValue < value)
                        {
                            newValue = X_MaxValue;
                        }
                    }
                    if (m_X_CurrentValue != newValue)
                    {
                        m_X_CurrentValue = newValue;
                        Raise_CurrentValueChanged();
                    }
                    RefreshButtonsState();
                }
            }
        }

        int m_Y_CurrentValue = 0;
        public int Y_CurrentValue
        {
            get { return m_Y_CurrentValue; }
            set
            {
                if (value != m_Y_CurrentValue)
                {
                    int newValue = value;
                    if (Y_MinValue <= value && value <= Y_MaxValue)
                    {
                        newValue = value;
                    }
                    else
                    {
                        if (Y_MinValue > value)
                        {
                            newValue = Y_MinValue;
                        }
                        if (Y_MaxValue < value)
                        {
                            newValue = Y_MaxValue;
                        }
                    }
                    if (m_Y_CurrentValue != newValue)
                    {
                        m_Y_CurrentValue = newValue;
                        Raise_CurrentValueChanged();
                    }
                    RefreshButtonsState();
                }
            }
        }

        int m_X_DefaultValue = 0;
        public int X_DefaultValue
        {
            get { return m_X_DefaultValue; }
            set
            {
                m_X_DefaultValue = value;
                RefreshButtonsState();
            }
        }

        int m_Y_DefaultValue = 0;
        public int Y_DefaultValue
        {
            get { return m_Y_DefaultValue; }
            set
            {
                m_Y_DefaultValue = value;
                RefreshButtonsState();
            }
        }

        int m_Y_MinValue = int.MinValue;
        public int Y_MinValue
        {
            get { return m_Y_MinValue; }
            set
            {
                m_Y_MinValue = value;
                RefreshButtonsState();
            }
        }

        int m_Y_MaxValue = int.MaxValue;
        public int Y_MaxValue
        {
            get { return m_Y_MaxValue; }
            set
            {
                m_Y_MaxValue = value;
                RefreshButtonsState();
            }
        }

        void RefreshButtonsState()
        {

        }
    }
}
