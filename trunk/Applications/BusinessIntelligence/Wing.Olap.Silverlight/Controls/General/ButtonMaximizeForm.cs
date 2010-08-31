/*
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
using Wing.Olap.Controls.Buttons;

namespace Wing.Olap.Controls.General
{
    public class ButtonMaximizeForm : UserControl
    {
        public readonly RanetButton Button;
        bool m_IsMaximized = false;
        public bool IsMaximized
        {
            get { return m_IsMaximized; }
            set
            {
                m_IsMaximized = value;
                if (value)
                {
                    // Свернуть
                    Button.Content = UiHelper.CreateIcon(UriResources.Images.Remove16);
                }
                else 
                {
                    // Развернуть
                    Button.Content = UiHelper.CreateIcon(UriResources.Images.Add16);
                }
            }
        }

        public ButtonMaximizeForm()
        {
            Button = new RanetButton();
            Button.Width = Button.Height = 16;
            this.Content = Button;
            IsMaximized = false;
        }

    }
}
