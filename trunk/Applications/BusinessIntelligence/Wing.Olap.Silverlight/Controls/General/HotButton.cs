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

namespace Wing.Olap.Controls.General
{
    public abstract class HotButton : Button
    {
        public HotButton()
        {
            this.MouseEnter += new MouseEventHandler(AddButton_MouseEnter);
            this.MouseLeave += new MouseEventHandler(AddButton_MouseLeave);
        }

        protected Image EnterImage;
        protected Image LeaveImage;


        void AddButton_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Content = LeaveImage;
        }

        void AddButton_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Content = EnterImage;
        }
    }
}
