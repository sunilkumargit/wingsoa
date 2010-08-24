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

namespace Ranet.AgOlap.Controls.PivotGrid.Editors
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
