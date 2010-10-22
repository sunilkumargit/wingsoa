/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Wing.Olap.Controls.PivotGrid.Editors
{
    public class TextBoxCellEditor : TextBox, ICustomCellEditor
    {
        public TextBoxCellEditor()
            : base()
        {
            base.LostFocus += new RoutedEventHandler(TextBoxCellEditor_LostFocus);
            base.KeyDown += new KeyEventHandler(TextBoxCellEditor_KeyDown);
        }

        void TextBoxCellEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RaiseEndEdit(e);
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Escape)
            {
                RaiseCancelEdit(e);
                e.Handled = true;
                return;
            }

            switch (e.Key)
            { 
                case Key.Left:
                case Key.Right:
                case Key.Up:
                case Key.Down:
                    e.Handled = true;    
                    break;
            }
        }

        void TextBoxCellEditor_LostFocus(object sender, RoutedEventArgs e)
        {
            RaiseEndEdit(EventArgs.Empty);
        }

        private void RaiseCancelEdit(EventArgs e)
        {
            EventHandler handler2 = this.PivotGridEditorCancelEdit;
            if (handler2 != null)
            {
                handler2(this, e);
            }
        }

        private void RaiseEndEdit(EventArgs e)
        {
            EventHandler handler2 = this.PivotGridEditorEndEdit;
            if (handler2 != null)
            {
                handler2(this, e);
            }
        }

        #region ICustomCellEditor Members

        public event EventHandler PivotGridEditorCancelEdit;

        public event EventHandler PivotGridEditorEndEdit;

        public object Value
        {
            get
            {
                return base.Text;
            }
            set
            {
                if (value != null)
                    base.Text = value.ToString();
                else
                    base.Text = String.Empty;
                // Смещаем курсор в конец значения
                base.SelectionStart = base.Text.Length;
                base.SelectionLength = 0;
            }
        }

        public Control Editor
        {
            get { return this as Control; }
        }

        #endregion ICustomCellEditor Members
    }
}
