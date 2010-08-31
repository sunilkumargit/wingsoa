/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Wing.Olap.Controls.Forms
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
            if (IsEnsured)
                return;

            IsEnsured = true;

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