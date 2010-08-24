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
using Ranet.AgOlap.Controls.ToolBar;
using Ranet.AgOlap.Controls.General;
using Ranet.Olap.Core.Storage;
using Ranet.Olap.Core;

namespace Ranet.AgOlap.Controls.PivotGrid.Conditions
{
    public partial class CustomCellConditionsEditor : UserControl
    {
        RanetToolBarButton AddButton;
        RanetToolBarButton DeleteButton;
        RanetToolBarButton DeleteAllButton;

        RanetToolBarButton m_ExportButton;
        RanetToolBarButton m_ImportButton;

        public CustomCellConditionsEditor()
        {
            InitializeComponent();

            lblStyleDetails.Text = Localization.StyleDetails_Label;
            lblStyles.Text = Localization.Styles_Label;

            DescriptorsList.SelectionChanged += new EventHandler<SelectionChangedEventArgs<CellConditionsDescriptor>>(DescriptorsList_SelectionChanged);
            DescriptorDetails.EditEnd += new EventHandler(DescriptorDetails_EditEnd);

            AddButton = new RanetToolBarButton();
            AddButton.Content = UiHelper.CreateIcon(UriResources.Images.New16);
            AddButton.Click += new RoutedEventHandler(m_AddButton_Click);
            ToolTipService.SetToolTip(AddButton, Localization.Toolbar_NewButton_ToolTip);
            ToolBar.AddItem(AddButton);

            DeleteButton = new RanetToolBarButton();
            DeleteButton.Content = UiHelper.CreateIcon(UriResources.Images.RemoveCross16);
            DeleteButton.Click += new RoutedEventHandler(m_DeleteButton_Click);
            DeleteButton.IsEnabled = false;
            ToolTipService.SetToolTip(DeleteButton, Localization.Toolbar_DeleteButton_ToolTip);
            ToolBar.AddItem(DeleteButton);

            DeleteAllButton = new RanetToolBarButton();
            DeleteAllButton.Content = UiHelper.CreateIcon(UriResources.Images.RemoveAllCross16);
            DeleteAllButton.Click += new RoutedEventHandler(m_DeleteAllButton_Click);
            DeleteAllButton.IsEnabled = false;
            ToolTipService.SetToolTip(DeleteAllButton, Localization.Toolbar_DeleteAllButton_ToolTip);
            ToolBar.AddItem(DeleteAllButton);

            m_ImportButton = new RanetToolBarButton();
            m_ImportButton.Content = UiHelper.CreateIcon(UriResources.Images.FileImport16);
            m_ImportButton.Click += new RoutedEventHandler(m_ImportButton_Click);
            ToolTipService.SetToolTip(m_ImportButton, Localization.LoadSettings_ToolTip);
            ToolBar.AddItem(m_ImportButton);

            m_ExportButton = new RanetToolBarButton();
            m_ExportButton.Content = UiHelper.CreateIcon(UriResources.Images.FileExport16);
            m_ExportButton.Click += new RoutedEventHandler(m_ExportButton_Click);
            ToolTipService.SetToolTip(m_ExportButton, Localization.SaveSettings_ToolTip);
            ToolBar.AddItem(m_ExportButton);

            DescriptorsList.DeleteAllButton_IsEnabledChanged += new DependencyPropertyChangedEventHandler(DescriptorsList_DeleteAllButton_IsEnabledChanged);
            DescriptorsList.DeleteButton_IsEnabledChanged += new DependencyPropertyChangedEventHandler(DescriptorsList_DeleteButton_IsEnabledChanged);
        }

        void DescriptorsList_DeleteButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool)
            {
                DeleteButton.IsEnabled = (bool)(e.NewValue);
            }
        }

        void DescriptorsList_DeleteAllButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool)
            {
                DeleteAllButton.IsEnabled = (bool)(e.NewValue);
            }
        }

        void m_DeleteAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(Localization.DeleteAll_Question, Localization.Warning, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DescriptorsList.DeleteAll();
            }
        }

        void m_DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(Localization.DeleteCurrent_Question, Localization.Warning, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DescriptorsList.DeleteCurrent();
            }
        }

        void m_AddButton_Click(object sender, RoutedEventArgs e)
        {
            DescriptorsList.AddNew();
        }

        #region Экспорт/Импорт
        IStorageManager m_StorageManager = null;
        public IStorageManager StorageManager
        {
            get
            {
                return m_StorageManager;
            }
            set {
                m_StorageManager = value;
            }
        }

        ILogService m_LogManager = null;
        public virtual ILogService LogManager
        {
            get
            {
                return m_LogManager;
            }
            set
            {
                m_LogManager = value;
            }
        }

        void m_ExportButton_Click(object sender, RoutedEventArgs e)
        {
            EndEdit();
            ExportSettings(null);
        }

        void m_ImportButton_Click(object sender, RoutedEventArgs e)
        {
            ObjectLoadDialog dlg = new ObjectLoadDialog(StorageManager) { ContentType = StorageContentTypes.CustomCellStyles };
            dlg.DialogOk += new EventHandler(LoadDialog_DialogOk);
            dlg.LogManager = LogManager;
            dlg.Show();
        }

        void ExportSettings(object tag)
        {
            ObjectSaveAsDialog dlg = new ObjectSaveAsDialog(StorageManager) { ContentType = StorageContentTypes.CustomCellStyles };
            dlg.DialogOk += new EventHandler(SaveAsDialog_DialogOk);
            dlg.Show();
            dlg.Tag = tag;
        }

        public event EventHandler<CustomEventArgs<ObjectDescription>> SaveStyles;
        public event EventHandler<CustomEventArgs<ObjectStorageFileDescription>> LoadStyles;

        void SaveAsDialog_DialogOk(object sender, EventArgs e)
        {
            ObjectSaveAsDialog dlg = sender as ObjectSaveAsDialog;
            if (dlg == null)
                return;

            ObjectDescription descr = dlg.Object;
            if (descr != null && !String.IsNullOrEmpty(descr.Name))
            {
                EventHandler<CustomEventArgs<ObjectDescription>> handler = SaveStyles;
                if (handler != null)
                {
                    handler(this, new CustomEventArgs<ObjectDescription>(descr));
                }
            }
            else
            {
                MessageBox.Show(Localization.ObjectSaveDialog_NameIsEmpty_Message, Localization.Warning, MessageBoxButton.OK);
            }
        }

        void LoadDialog_DialogOk(object sender, EventArgs e)
        {
            ObjectLoadDialog dlg = sender as ObjectLoadDialog;
            if (dlg == null)
                return;

            if (dlg.CurrentObject != null && !String.IsNullOrEmpty(dlg.CurrentObject.ContentFileName))
            {
                EventHandler<CustomEventArgs<ObjectStorageFileDescription>> handler = LoadStyles;
                if (handler != null)
                {
                    handler(this, new CustomEventArgs<ObjectStorageFileDescription>(dlg.CurrentObject));
                }
            }
        }
        #endregion Экспорт/Импорт

        void DescriptorDetails_EditEnd(object sender, EventArgs e)
        {
            DescriptorsList.Refresh();
        }

        void DescriptorsList_SelectionChanged(object sender, SelectionChangedEventArgs<CellConditionsDescriptor> e)
        {
            EndEdit();
            DescriptorDetails.Initialize(e.NewValue);
        }

        public void EndEdit()
        {
            if (DescriptorDetails.IsEnabled && DescriptorDetails.Descriptor != null)
            {
                DescriptorDetails.EndEdit();
            }

            DescriptorsList.Refresh();
        }

        public void Initialize(List<CellConditionsDescriptor> cellsConditions)
        {
            DescriptorsList.Initialize(cellsConditions);
            if (cellsConditions == null || cellsConditions.Count == 0)
            {
                DescriptorDetails.Initialize(null);
            }
        }

        public List<CellConditionsDescriptor> CellsConditions
        {
            get { return DescriptorsList.List; }
        }
    }
}
