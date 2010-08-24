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
using System.Windows.Controls.Primitives;
using System.Windows.Browser;
using Ranet.AgOlap.Features;

namespace Ranet.AgOlap.Controls.Forms
{
    public enum DialogResult
    {
        Yes,
        No,
        None,
        OK,
        Cancel
    }

    public class DialogResultArgs : EventArgs
    {
        public bool Cancel = false;

        public readonly DialogResult Result = DialogResult.None;
        public DialogResultArgs(DialogResult res)
        {
            Result = res;
        }
    }

    public abstract class FloatingDialogBase
    {
        public FloatingDialogStyle m_PopUpStyle = FloatingDialogStyle.ModalDimmed;
        public FloatingDialogStyle PopUpStyle
        {
            get
            {
                return m_PopUpStyle;
            }
            set
            {
                m_PopUpStyle = value;
            }
        }

        public void Show()
        {
            Show(new Point(double.NaN, double.NaN));
        }
        
        public virtual void Show(Point position)
        {
            if (_isShowing)
            {
                return;
                //throw new InvalidOperationException();
            }

            _isShowing = true;
            EnsurePopup(PopUpStyle, position);
            _popup.IsOpen = true;
            Application.Current.Host.Content.Resized += OnPluginSizeChanged;
        }

        public event EventHandler<DialogResultArgs> BeforeClosed;
        void Raise_BeforeClosed(DialogResultArgs args)
        {
            EventHandler<DialogResultArgs> handler = BeforeClosed;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        public event EventHandler<DialogResultArgs> DialogClosed;
        void Raise_DialogClosed(DialogResult result)
        {
            EventHandler<DialogResultArgs> handler = DialogClosed;
            if (handler != null)
            {
                handler(this, new DialogResultArgs(result));
            }
        }

        public void Close()
        {
            Close(DialogResult.None);
        }

        public void Close(DialogResult result)
        {
            // Даем возможность отменить закрытие
            DialogResultArgs args = new DialogResultArgs(result);
            Raise_BeforeClosed(args);
            if (!args.Cancel)
            {
                _isShowing = false;
                if (_popup != null)
                {
                    _popup.IsOpen = false;
                    Application.Current.Host.Content.Resized -= OnPluginSizeChanged;
                }
                Raise_DialogClosed(result);
            }
        }

        protected abstract FrameworkElement GetContent();

        protected virtual void OnClickOutside() { }

        bool IsEnsured = false;
        private void EnsurePopup(FloatingDialogStyle style, Point position)
        {
            //if (_popup != null)
            //if (_grid != null)
            //    return;

            if (IsEnsured)
                return;

            IsEnsured = true;
            //_popup = new Popup();
            //_grid = new Grid();
            //_popup.Child = _grid;
            _popup.Closed += new EventHandler(_popup_Closed);
            _popup.Opened += new EventHandler(_popup_Opened);

            if (style != FloatingDialogStyle.NonModal)
            {
                // If Canvas.Background != null, you cannot click through it
                _canvas = new Canvas();
                _canvas.MouseLeftButtonDown += new MouseButtonEventHandler(_canvas_MouseLeftButtonDown);

                if (style == FloatingDialogStyle.Modal)
                {
                    _canvas.Background = new SolidColorBrush(Colors.Transparent);
                }

                else if (style == FloatingDialogStyle.ModalDimmed)
                {
                    _canvas.Background = new SolidColorBrush(Color.FromArgb(0x20, 0x80, 0x80, 0x80));
                }

                //_grid.Children.Add(_canvas);
            }

            _popup.Child = _canvas;
            _canvas.Children.Add(_content = GetContent());

            UpdateSize();

            if (position.X.CompareTo(double.NaN) == 0 && position.Y.CompareTo(double.NaN) == 0)
            {
                _content.LayoutUpdated += new EventHandler(_content_LayoutUpdated);
            }
            else
            {
                MoveTo(position);
            }
        }

        void Document_OnKeyDown(object sender, HtmlEventArgs e)
        {
            if (e != null && e.CharacterCode == 27) //Escape
            {
                Close(DialogResult.Cancel);
            }
        }

        public void ListenKeys(bool value)
        {
            if (value)
            {
                if (BrowserHelper.IsMozilla)
                {
                    HtmlPage.Document.AttachEvent("onkeydown", new EventHandler<HtmlEventArgs>(Document_OnKeyDown));
                }
                else
                {
                    HtmlPage.Document.AttachEvent("onkeypress", new EventHandler<HtmlEventArgs>(Document_OnKeyDown));
                }
            }
            else
            {
                if (BrowserHelper.IsMozilla)
                {
                    HtmlPage.Document.DetachEvent("onkeydown", new EventHandler<HtmlEventArgs>(Document_OnKeyDown));
                }
                else
                {
                    HtmlPage.Document.DetachEvent("onkeypress", new EventHandler<HtmlEventArgs>(Document_OnKeyDown));
                }
            }
        }

        void _popup_Opened(object sender, EventArgs e)
        {
            ListenKeys(true);
        }

        void _popup_Closed(object sender, EventArgs e)
        {
            ListenKeys(false);
        }

        void _canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            bool outside = true;
            if (_content != null)
            {
                // Позиция курсора
                Point mouse_pos = e.GetPosition(null);
                double X_content_pos = Canvas.GetLeft(_content);
                double Y_content_pos = Canvas.GetTop(_content);

                if (mouse_pos.X >= X_content_pos &&
                    mouse_pos.X <= X_content_pos + _content.ActualWidth &&
                    mouse_pos.Y >= Y_content_pos &&
                    mouse_pos.Y <= Y_content_pos + _content.ActualHeight)
                    outside = false;
            }
            if (outside)
                OnClickOutside();
        }

        void _content_LayoutUpdated(object sender, EventArgs e)
        {
            _content.LayoutUpdated -= new EventHandler(_content_LayoutUpdated);
            MoveToCenter();
        }

        void _content_Loaded(object sender, RoutedEventArgs e)
        {
            MoveToCenter();
        }

        void MoveTo(Point position)
        {
            if (position.Y.CompareTo(double.NaN) != 0)
                Canvas.SetTop(_content, position.Y);
            if (position.X.CompareTo(double.NaN) != 0)
                Canvas.SetLeft(_content, position.X);
        }

        void MoveToCenter()
        {
            double x = (_canvas.Width - _content.ActualWidth) / 2;
            double y = (_canvas.Height - _content.ActualHeight) / 2; ;
            x = Math.Round(x, 0);
            y = Math.Round(y, 0);
            x = Math.Max(0, x);
            y = Math.Max(0, y);

            MoveTo(new Point(x, y));
        }

        private void OnPluginSizeChanged(object sender, EventArgs e)
        {
            UpdateSize();
        }

        private void UpdateSize()
        {
            //_grid.Width = Application.Current.Host.Content.ActualWidth;
            //_grid.Height = Application.Current.Host.Content.ActualHeight;

            if (_canvas != null)
            {
                _canvas.Width = Application.Current.Host.Content.ActualWidth;
                _canvas.Height = Application.Current.Host.Content.ActualHeight;
            }
        }

        public bool IsShowing
        {
            get
            {
                return _isShowing;
            }
            set
            {
                if (value)
                {
                    Show();
                }
                else
                {
                    Close();
                }
            }
        }
        private bool _isShowing;

        public Popup PopUpControl
        {
            get
            {
                return _popup;
            }
        }
        //private Popup _popup;
        Popup _popup = new Popup();

        //private Grid _grid;

        private Canvas _canvas;

        private FrameworkElement _content;
    }

    public enum FloatingDialogStyle
    {
        NonModal,
        Modal,
        ModalDimmed
    };
}