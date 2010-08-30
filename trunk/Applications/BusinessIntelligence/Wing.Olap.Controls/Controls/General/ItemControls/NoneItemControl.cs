﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Wing.AgOlap.Controls.General.ItemControls
{
    public class NoneItemControl : ItemControlBase
    {
        public NoneItemControl()
            : base(false)
        {
            Text = Localization.ComboBoxItem_None;
        }
    }
}
