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

namespace Ranet.AgOlap.Controls.PivotGrid.Controls
{
    public abstract class PivotGridItem : Button
    {
        PivotGridItem()
        {
            ClickMode = ClickMode.Press;
        }

        public PivotGridItem(PivotGridControl owner)
            : this()
        {
            if (owner == null)
                throw new ArgumentNullException("owner");

            Owner = owner;
        }

        public static PivotGridItem GetPivotGridItem(Point p)
        {
            foreach (UIElement element in
                VisualTreeHelper.FindElementsInHostCoordinates(p, Application.Current.RootVisual))
            {
                PivotGridItem item = element as PivotGridItem;
                if (item != null)
                    return item;
            }

            return null;
        }

        bool m_ShowUpBorder = false;
        /// <summary>
        /// Отображать верхнюю границу элемента
        /// </summary>
        public bool ShowUpBorder
        {
            get { return m_ShowUpBorder; }
            set
            {
                m_ShowUpBorder = value;
                UpdateUpBorder();
            }
        }

        protected virtual void UpdateUpBorder()
        {
            if (ShowUpBorder)
                BorderThickness = new Thickness(BorderThickness.Left, 1, BorderThickness.Right, BorderThickness.Bottom);
            else
                BorderThickness = new Thickness(BorderThickness.Left, 0, BorderThickness.Right, BorderThickness.Bottom);
        }

        bool m_ShowLeftBorder = false;
        /// <summary>
        /// Отображать левую границу элемента
        /// </summary>
        public bool ShowLeftBorder
        {
            get { return m_ShowLeftBorder; }
            set
            {
                m_ShowLeftBorder = value;
                UpdateLeftBorder();
            }
        }

        protected virtual void UpdateLeftBorder()
        {
            if (ShowLeftBorder)
                BorderThickness = new Thickness(1, BorderThickness.Top, BorderThickness.Right, BorderThickness.Bottom);
            else
                BorderThickness = new Thickness(0, BorderThickness.Top, BorderThickness.Right, BorderThickness.Bottom);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            UpdateUpBorder();
            UpdateLeftBorder();
        }

        protected readonly PivotGridControl Owner = null;

        protected double Scale
        {
            get
            {
                if (Owner == null)
                {
                    return 1;
                }
                else
                {
                    return Owner.Scale;
                }

            }
        }
    }
}
