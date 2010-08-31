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
using System.Windows.Browser;

namespace Wing.Olap.Features
{
    public class BrowserHelper
    {
        public static bool IsMozilla
        {
            get
            {
                try
                {
                    if (HtmlPage.BrowserInformation.Name == "Netscape")
                        return true;
                }
                catch { }
                return false;
            }
        }
    }
}
