/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Wing.Olap.Controls.PivotGrid.Controls
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
