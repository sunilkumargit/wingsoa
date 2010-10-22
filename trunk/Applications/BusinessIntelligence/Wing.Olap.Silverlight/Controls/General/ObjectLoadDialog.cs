/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Wing.Olap.Controls.Forms;
using Wing.Olap.Controls.Storage;
using Wing.Olap.Core;
using Wing.Olap.Core.Storage;

namespace Wing.Olap.Controls.General
{
    public class ObjectLoadDialog
    {
        IStorageManager m_StorageManager = null;
        ObjectDescriptionControl m_Description;
        ObjectDescriptionListControl m_List;
        ModalDialog m_Dlg;

        ILogService m_LogManager = null;
        public virtual ILogService LogManager
        {
            get
            {
                if (m_LogManager == null)
                    m_LogManager = new DefaultLogManager();
                return m_LogManager;
            }
            set
            {
                m_LogManager = value;
            }
        }

        public ObjectLoadDialog(IStorageManager storageManager)
        {
            m_StorageManager = storageManager;
            m_Dlg = new ModalDialog();

            m_Dlg.MinHeight = 200;
            m_Dlg.MinWidth = 300;
            m_Dlg.Height = 400;
            m_Dlg.Width = 500;
            m_Dlg.Caption = Localization.LoadDialog_Caption;
            m_Dlg.DialogOk += new EventHandler<DialogResultArgs>(Dlg_DialogOk);

            Grid LayoutRoot = new Grid();
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition());
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition());

            m_List = new ObjectDescriptionListControl();
            m_List.Margin = new Thickness(0, 0, 5, 0);
            m_List.SelectionChanged += new EventHandler<SelectionChangedEventArgs<ObjectStorageFileDescription>>(m_List_SelectionChanged);
            m_List.ObjectSelected += new EventHandler<CustomEventArgs<ObjectStorageFileDescription>>(m_List_ObjectSelected);
            m_List.DeleteButtonClick += new EventHandler<CustomEventArgs<ObjectStorageFileDescription>>(m_List_DeleteButtonClick);
            m_List.DeleteAllButtonClick += new EventHandler<CustomEventArgs<ObjectStorageFileDescription>>(m_List_DeleteAllButtonClick);
            LayoutRoot.Children.Add(m_List);

            RanetGridSplitter splitter_Vert = new RanetGridSplitter();
            splitter_Vert.IsTabStop = false;
            LayoutRoot.Children.Add(splitter_Vert);
            Grid.SetColumn(splitter_Vert, 0);
            splitter_Vert.Background = new SolidColorBrush(Colors.Transparent);
            splitter_Vert.HorizontalAlignment = HorizontalAlignment.Right;
            splitter_Vert.VerticalAlignment = VerticalAlignment.Stretch;

            m_Description = new ObjectDescriptionControl() { Margin = new Thickness(1, 0, 0, 0) };
            m_Description.IsReadonly = true;
            LayoutRoot.Children.Add(m_Description);
            Grid.SetColumn(m_Description, 1);

            m_Dlg.Content = LayoutRoot;

            if (m_StorageManager != null)
            {
                m_StorageManager.InvokeCompleted -= new EventHandler<DataLoaderEventArgs>(m_StorageManager_ActionCompleted);
                m_StorageManager.InvokeCompleted += new EventHandler<DataLoaderEventArgs>(m_StorageManager_ActionCompleted);
            }
        }

        void m_List_DeleteAllButtonClick(object sender, CustomEventArgs<ObjectStorageFileDescription> e)
        {
            m_List.Initialize(null);
            m_List.IsWaiting = true;

            StorageActionArgs args = new StorageActionArgs();
            args.ActionType = StorageActionTypes.Clear;
            args.ContentType = ContentType;
            m_StorageManager.Invoke(XmlSerializationUtility.Obj2XmlStr(args, Constants.XmlNamespace), args);
        }

        void m_List_DeleteButtonClick(object sender, CustomEventArgs<ObjectStorageFileDescription> e)
        {
            var current = m_List.CurrentObject;
            if (current != null)
            {
                StorageActionArgs args = new StorageActionArgs();
                args.ActionType = StorageActionTypes.Delete;
                args.ContentType = ContentType;
                args.FileDescription = m_List.CurrentObject;
                m_StorageManager.Invoke(XmlSerializationUtility.Obj2XmlStr(args, Constants.XmlNamespace), args);
            }
        }

        void m_List_ObjectSelected(object sender, CustomEventArgs<ObjectStorageFileDescription> e)
        {
            if (CurrentObject != null)
            {
                if (String.IsNullOrEmpty(CurrentObject.ContentFileName))
                {
                    MessageBox.Show(Localization.ObjectLoadDialog_ContentError_Message, Localization.Error, MessageBoxButton.OK);
                    return;
                }

                m_Dlg.Close();
                EventHandler handler = DialogOk;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }           
            }
        }

        void m_List_SelectionChanged(object sender, SelectionChangedEventArgs<ObjectStorageFileDescription> e)
        {
            // Выводим информацию о текущем выбранном элементе
            if (CurrentObject != null)
                m_Description.Object = CurrentObject.Description;
            else
                m_Description.Object = null;
        }

        void m_StorageManager_ActionCompleted(object sender, DataLoaderEventArgs e)
        {
            if (e.Error != null)
            {
                LogManager.LogError(this, e.Error.ToString());
                return;
            }

            if (e.Result.ContentType == InvokeContentType.Error)
            {
                LogManager.LogError(this, e.Result.Content);
                return;
            }

            StorageActionArgs args = e.UserState as StorageActionArgs;
            if (args != null)
            {
                if (args.ActionType == StorageActionTypes.GetList ||
                    args.ActionType == StorageActionTypes.Clear ||
                    args.ActionType == StorageActionTypes.Delete)
                {
                    List<ObjectStorageFileDescription> list = XmlSerializationUtility.XmlStr2Obj<List<ObjectStorageFileDescription>>(e.Result.Content);
                    m_List.Initialize(list);
                    m_List.IsWaiting = false;
                }
            }
        }

        public ObjectStorageFileDescription CurrentObject 
        {
            get { return m_List.CurrentObject; }
        }

        public event EventHandler DialogOk;
        
        void Dlg_DialogOk(object sender, DialogResultArgs e)
        {
            if (CurrentObject == null)
            {
                e.Cancel = true;
                return;
            }

            if(String.IsNullOrEmpty(CurrentObject.ContentFileName))
            {
                MessageBox.Show(Localization.ObjectLoadDialog_ContentError_Message, Localization.Error, MessageBoxButton.OK);
                e.Cancel = true;
                return;
            }

            EventHandler handler = DialogOk;
            if(handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        StorageContentTypes m_ContentType = StorageContentTypes.None;
        public StorageContentTypes ContentType
        {
            get { return m_ContentType; }
            set { m_ContentType = value; }
        }

        public void Show()
        {
            m_Dlg.Show();

            m_List.Initialize(null);

            if (m_StorageManager != null)
            {
                m_List.IsWaiting = true;

                StorageActionArgs args = new StorageActionArgs();
                args.ActionType = StorageActionTypes.GetList;
                args.ContentType = ContentType;
                m_StorageManager.Invoke(XmlSerializationUtility.Obj2XmlStr(args, Constants.XmlNamespace), args);
            }
        }
    }
}
