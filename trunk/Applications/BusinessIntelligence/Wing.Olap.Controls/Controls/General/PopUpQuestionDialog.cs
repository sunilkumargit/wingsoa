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
using Wing.AgOlap.Controls.Forms;

namespace Wing.AgOlap.Controls.General
{
    public enum DialogButtons
    { 
        OK,
        OKCancel,
        YesNo
    }

    public class PopUpQuestionDialog : FloatingDialog
    {
        public object Tag = null;
        #region QuestionCtrl
        internal class QuestionCtrl : UserControl
        {
            TextBlock ContentText;
            Button Button1;
            Button Button2;

            public String Text
            {
                set {
                    ContentText.Text = value;
                }
                get {
                    return ContentText.Text;
                }
            }

            public event EventHandler<DialogResultArgs> ButtonClick;
            Grid m_LayoutRoot = null;
            StackPanel m_ButtonsStack = null;

            public QuestionCtrl()
            {
                m_LayoutRoot = new Grid() { Background = new SolidColorBrush(Colors.White) };

                m_LayoutRoot.RowDefinitions.Add(new RowDefinition());
                m_LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                ContentText = new TextBlock();
                ContentText.HorizontalAlignment = HorizontalAlignment.Center;
                ContentText.VerticalAlignment = VerticalAlignment.Center;
                ContentText.TextAlignment = TextAlignment.Center;
                ContentText.Margin = new Thickness(10, 5, 10, 5);
                ContentText.TextWrapping = TextWrapping.Wrap;
                m_LayoutRoot.Children.Add(ContentText);

                m_ButtonsStack = new StackPanel() { Margin = new Thickness(5), Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center }; 

                m_LayoutRoot.Children.Add(m_ButtonsStack);
                Grid.SetRow(m_ButtonsStack, 1);

                m_LayoutRoot.Height = 80;
                this.Content = m_LayoutRoot;
            }

            internal DialogButtons DialogType = DialogButtons.OKCancel;

            public void InitButtons()
            {
                m_ButtonsStack.Children.Clear();

                Button1 = new Button();
                Button1.Width = 75;
                Button1.Click += new RoutedEventHandler(Button_Click);

                Button2 = new Button();
                Button2.Width = 75;
                Button2.Margin = new Thickness(15, 0, 0, 0);
                Button2.Click += new RoutedEventHandler(Button_Click);

                switch (DialogType)
                {
                    case DialogButtons.OK:
                        m_ButtonsStack.Children.Add(Button1);
                        Button1.Content = "OK";
                        Button1.Tag = DialogResult.OK;
                        break;
                    case DialogButtons.YesNo:
                        m_ButtonsStack.Children.Add(Button1);
                        Button1.Content = "Yes";
                        Button1.Tag = DialogResult.Yes;
                        m_ButtonsStack.Children.Add(Button2);
                        Button2.Content = "No";
                        Button2.Tag = DialogResult.No;
                        break;
                    case DialogButtons.OKCancel:
                        m_ButtonsStack.Children.Add(Button1);
                        Button1.Content = "OK";
                        Button1.Tag = DialogResult.OK;
                        m_ButtonsStack.Children.Add(Button2);
                        Button2.Content = "Cancel";
                        Button2.Tag = DialogResult.Cancel;
                        break;
                }
            }

            void Button_Click(object sender, RoutedEventArgs e)
            {
                EventHandler<DialogResultArgs> handler = ButtonClick;
                DialogResult res = DialogResult.None;
                Button btn = sender as Button;
                if (btn.Tag is DialogResult)
                    res = (DialogResult)btn.Tag;
                if (handler != null)
                {
                    handler(this, new DialogResultArgs(res));
                }
            }
        }
        #endregion

        public PopUpQuestionDialog()
        {
            m_ContentCtrl = new QuestionCtrl();
            m_ContentCtrl.ButtonClick+=new EventHandler<DialogResultArgs>(m_ContentCtrl_ButtonClick);
        }

        void m_ContentCtrl_ButtonClick(object sender, DialogResultArgs e)
        {
            Close(e.Result);
        }

        QuestionCtrl m_ContentCtrl;
        internal QuestionCtrl ContentCtrl
        {
            get {
                return m_ContentCtrl;
            }
        }

        protected override void OnClickOutside()
        {
        }

        public new void Show()
        {
            SetContent(ContentCtrl);
            m_ContentCtrl.InitButtons();
            base.Show();
        }
    }
}
