using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Ranet.AgOlap.Controls.General
{
    public partial class ButtonMinMaxForm : UserControl
    {
        public ButtonMinMaxForm()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(ButtonMinMaxForm_Loaded);
            this.btnMinMaxButton.Checked += new RoutedEventHandler(btnMinMaxButton_Checked);
            this.btnMinMaxButton.Unchecked += new RoutedEventHandler(btnMinMaxButton_Unchecked);
            ToolTipService.SetToolTip(this, Localization.Dialog_MaximizeButton_ToolTip);
        }

        void btnMinMaxButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ToolTipService.SetToolTip(this, Localization.Dialog_MaximizeButton_ToolTip);
        }

        void btnMinMaxButton_Checked(object sender, RoutedEventArgs e)
        {
            ToolTipService.SetToolTip(this, Localization.Dialog_RestoreDownButton_ToolTip);
        }

        void ButtonMinMaxForm_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        public bool IsMaximized
        {
            get { return btnMinMaxButton.IsChecked.HasValue ? !btnMinMaxButton.IsChecked.Value : false; }
            set
            {
                btnMinMaxButton.IsChecked = new bool?(!value);
            }
        }
    }
}
