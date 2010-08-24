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

namespace Ranet.AgOlap.Controls.General
{
    public interface ILogService
    {
        void LogError(object sender, String message);
        void LogWarning(object sender, String message);
        void LogInformation(object sender, String message);
    }

    public class DefaultLogManager : ILogService
    {
        #region ILogService Members
        public void LogError(object sender, String message)
        {
            String caption = GetControlCaption(sender);
            if (String.IsNullOrEmpty(caption))
                caption = Localization.Error;

            MessageBox.Show(message, caption, MessageBoxButton.OK);
        }

        public void LogWarning(object sender, string message)
        {
            String caption = GetControlCaption(sender);
            if (String.IsNullOrEmpty(caption))
                caption = Localization.Warning;

            MessageBox.Show(message, caption, MessageBoxButton.OK);
        }

        public void LogInformation(object sender, string message)
        {
            String caption = GetControlCaption(sender);
            System.Diagnostics.Debug.WriteLine(DateTime.Now + " " + caption + ": " + message);
        }
        #endregion

        public static String GetControlCaption(object sender)
        {
            String caption = String.Empty;
            if (sender is CubeChoiceCtrl)
                caption = Localization.CubeChoiceControl_Name;
            if (sender is DateChoiceCtrl)
                caption = Localization.DateChoiceControl_Name;
            if (sender is DatePickerCtrl)
                caption = Localization.DateChoiceControl_Name;
            if (sender is ObjectSaveAsDialog)
                caption = Localization.SaveAsDialog_Name;
            if (sender is ObjectLoadDialog)
                caption = Localization.LoadingDialog_Name;
            if (sender is HierarchyChoiceCtrl)
                caption = Localization.HierarchyChoiceControl_Name;
            if (sender is DimensionChoiceCtrl)
                caption = Localization.DimensionChoiceControl_Name;
            if (sender is LevelChoiceCtrl)
                caption = Localization.LevelChoiceControl_Name;
            if (sender is MeasureChoiceCtrl)
                caption = Localization.MeasureChoiceControl_Name;
            if (sender is KpiChoiceCtrl)
                caption = Localization.KpiChoiceControl_Name;
            if (sender is MemberChoiceControl)
                caption = Localization.MemberChoiceControl_Name;
            if (sender is OlapBrowserControl)
                caption = Localization.OlapBrowserControl_Name;
            if (sender is PivotMdxDesignerControl)
                caption = Localization.MdxDesignerControl_Name;
            if (sender is ServerExplorerCtrl)
                caption = Localization.ServerExplorerControl_Name;
            if (sender is TransactionStateButton)
                caption = Localization.TransactionStateButton_Name;
            if (sender is UpdateablePivotGridControl)
                caption = Localization.PivotGridControl_Name;
            if (sender is ValueCopyDesignerControl)
                caption = Localization.ValueCopyDesignerControl_Name;
            return caption;
        }
    }


    public class AgControlBase : UserControl
    {
        public virtual String URL{ get; set; }

        ILogService m_LogManager = null;
        public virtual ILogService LogManager
        {
            get {
                if (m_LogManager == null)
                    m_LogManager = new DefaultLogManager();
                return m_LogManager;
            }
            set {
                m_LogManager = value;
            }
        }

        public static Rect GetSLBounds(FrameworkElement item)
        {
            Point pos = AgControlBase.GetSilverlightPos(item);
            return new Rect(pos, new Size(item.ActualWidth, item.ActualHeight));
        }

        public static Point GetSilverlightPos(UIElement item)
        {
            Point point = new Point(0, 0);

            if (item != null)
            {
                try
                {
                    Point transformPoint = Application.Current.RootVisual.TransformToVisual(item).Transform(point);
                    return new Point(transformPoint.X * -1, transformPoint.Y * -1);
                }
                catch (ArgumentException ex)
                {
                }
            }

            return point;
        }
    }
}
