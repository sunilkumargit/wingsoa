/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Wing.Olap.Commands;
using Wing.Olap.Controls.Buttons;
using Wing.Olap.Controls.ContextMenu;
using Wing.Olap.Controls.DataSourceInfo;
using Wing.Olap.Controls.Forms;
using Wing.Olap.Controls.General;
using Wing.Olap.Controls.General.ClientServer;
using Wing.Olap.Controls.PivotGrid;
using Wing.Olap.Controls.PivotGrid.Conditions;
using Wing.Olap.Controls.PivotGrid.Controls;
using Wing.Olap.Controls.ToolBar;
using Wing.Olap.Controls.ValueCopy;
using Wing.Olap.Controls.ValueDelivery;
using Wing.Olap.Core;
using Wing.Olap.Core.Data;
using Wing.Olap.Core.Metadata;
using Wing.Olap.Core.Providers;
using Wing.Olap.Core.Providers.ClientServer;
using Wing.Olap.Core.Storage;
using Wing.Olap.Providers;

namespace Wing.Olap.Controls
{
	public partial class UpdateablePivotGridControl : AgControlBase
	{
		const string MEMBER_ACTION = "MEMBER_ACTION";
		const string SERVICE_COMMAND = "SERVICE_COMMAND";

		PivotGridControl m_PivotGrid;
		public PivotGridControl PivotGrid
		{
			get
			{
				return m_PivotGrid;
			}
		}

		Grid LayoutRoot = new Grid();
		RanetToolBar ToolBar = new RanetToolBar();

		RanetToolBarButton RefreshButton = null;
		protected RanetToolBarButton BackButton = null;
		protected RanetToolBarButton ForwardButton = null;
		protected RanetToolBarButton ToBeginButton = null;
		protected RanetToolBarButton ToEndButton = null;
		protected RanetToolBarSplitter NavigationToolBarSplitter = null;
		protected RanetToolBarButton GoToFocusedCellButton = null;

		protected RanetToolBarButton RestoreDefaultSizeButton = null;
		protected RanetToggleButton HideEmptyColumnsButton = null;
		protected RanetToggleButton HideEmptyRowsButton = null;
		protected RanetToggleButton EditButton = null;
		protected RanetToggleButton UseChangesCasheButton = null;
		protected RanetToolBarButton ConfirmEditButton = null;
		protected RanetToolBarButton CancelEditButton = null;
		protected RanetToolBarButton ExportToExcelButton = null;
		protected RanetToolBarButton CopyToClipboardButton = null;
		protected RanetToolBarButton PasteFromClipboardButton = null;
		protected RanetToggleButton RotateAxesButton = null;
		protected RanetToggleButton HideHintsButton = null;
		protected RanetToggleButton ConditionsDesignerButton = null;
		protected ZoomingToolBarControl ZoomControl = null;

		public event RoutedEventHandler ExportToExcelClick;
		protected void OnExportToExcelClick()
		{
			if (ExportToExcelClick != null)
				ExportToExcelClick(this, new RoutedEventArgs());
		}

		bool m_ShowToolBar = true;
		public bool ShowToolBar
		{
			get { return m_ShowToolBar; }
			set
			{
				m_ShowToolBar = value;
				if (value)
				{
					ToolBar.Visibility = Visibility.Visible;
				}
				else
				{ ToolBar.Visibility = Visibility.Collapsed; }
			}
		}

		bool m_IsWaiting = false;
		protected bool IsWaiting
		{
			get { return m_IsWaiting; }
			set
			{
				if (m_IsWaiting != value)
				{
					if (value)
					{
						this.Cursor = Cursors.Wait;
						grdIsWaiting.Visibility = Visibility.Visible;
					}
					else
					{
						this.Cursor = Cursors.Arrow;
						grdIsWaiting.Visibility = Visibility.Collapsed;
						// Set Focus to PivotGrid
						this.LayoutUpdated += new EventHandler(UpdateablePivotGridControl_LayoutUpdated);
					}
					this.IsEnabled = !value;
					m_IsWaiting = value;
					this.UpdateLayout();
				}
			}
		}

		void UpdateablePivotGridControl_LayoutUpdated(object sender, EventArgs e)
		{
			this.LayoutUpdated -= new EventHandler(UpdateablePivotGridControl_LayoutUpdated);
			PivotGrid.Focus();
		}

		public bool UseModifyDataConfirm = false;


		Grid grdIsWaiting;
		public UpdateablePivotGridControl()
		{
			RowDefinition row0 = new RowDefinition();
			row0.Height = GridLength.Auto;
			RowDefinition row1 = new RowDefinition();

			LayoutRoot.RowDefinitions.Add(row0);
			LayoutRoot.RowDefinitions.Add(row1);

			RefreshButton = new RanetToolBarButton();
			RefreshButton.Click += new RoutedEventHandler(RefreshButton_Click);
			RefreshButton.Content = UiHelper.CreateIcon(UriResources.Images.Refresh16);
			ToolTipService.SetToolTip(RefreshButton, Localization.PivotGrid_RefreshButton_ToolTip);

			ToBeginButton = new RanetToolBarButton();
			ToBeginButton.Click += new RoutedEventHandler(ToBeginButton_Click);
			ToBeginButton.Content = UiHelper.CreateIcon(UriResources.Images.ToBegin16);
			ToolTipService.SetToolTip(ToBeginButton, Localization.PivotGrid_ToBeginButton_ToolTip);

			BackButton = new RanetToolBarButton();
			BackButton.Click += new RoutedEventHandler(BackButton_Click);
			BackButton.Content = UiHelper.CreateIcon(UriResources.Images.Back16);
			ToolTipService.SetToolTip(BackButton, Localization.PivotGrid_BackButton_ToolTip);

			ForwardButton = new RanetToolBarButton();
			ForwardButton.Click += new RoutedEventHandler(ForwardButton_Click);
			ForwardButton.Content = UiHelper.CreateIcon(UriResources.Images.Forward16);
			ToolTipService.SetToolTip(ForwardButton, Localization.PivotGrid_ForwardButton_ToolTip);

			ToEndButton = new RanetToolBarButton();
			ToEndButton.Click += new RoutedEventHandler(ToEndButton_Click);
			ToEndButton.Content = UiHelper.CreateIcon(UriResources.Images.ToEnd16);
			ToolTipService.SetToolTip(ToEndButton, Localization.PivotGrid_ToEndButton_ToolTip);

			EditButton = new RanetToggleButton();
			ToolTipService.SetToolTip(EditButton, Localization.PivotGrid_EditButton_ToolTip);
			EditButton.Checked += new RoutedEventHandler(EditButton_Click);
			EditButton.Unchecked += new RoutedEventHandler(EditButton_Click);
			EditButton.Content = UiHelper.CreateIcon(UriResources.Images.EditCells16);

			UseChangesCasheButton = new RanetToggleButton();
			ToolTipService.SetToolTip(UseChangesCasheButton, Localization.PivotGrid_UseChangesCasheButton_ToolTip);
			UseChangesCasheButton.Checked += new RoutedEventHandler(UseChangesCasheButton_Checked);
			UseChangesCasheButton.Unchecked += new RoutedEventHandler(UseChangesCasheButton_Checked);
			UseChangesCasheButton.Content = UiHelper.CreateIcon(UriResources.Images.UseChangesCache16);

			CopyToClipboardButton = new RanetToolBarButton();
			ToolTipService.SetToolTip(CopyToClipboardButton, Localization.PivotGrid_CopyToClipboardButton_ToolTip);
			CopyToClipboardButton.Click += new RoutedEventHandler(CopyToClipboardoButton_Click);
			CopyToClipboardButton.Content = UiHelper.CreateIcon(UriResources.Images.Copy16);

			PasteFromClipboardButton = new RanetToolBarButton();
			ToolTipService.SetToolTip(PasteFromClipboardButton, Localization.PivotGrid_PasteFromClipboardButton_ToolTip);
			PasteFromClipboardButton.Click += new RoutedEventHandler(PasteFromClipboardoButton_Click);
			PasteFromClipboardButton.Content = UiHelper.CreateIcon(UriResources.Images.Paste16);

			ConfirmEditButton = new RanetToolBarButton(UriResources.Images.ConfirmEdit16, Localization.PivotGrid_ConfirmEditButton_Caption);
			ToolTipService.SetToolTip(ConfirmEditButton, Localization.PivotGrid_ConfirmEditButton_ToolTip);
			ConfirmEditButton.Click += new RoutedEventHandler(ConfirmEditButton_Click);

			CancelEditButton = new RanetToolBarButton();
			ToolTipService.SetToolTip(CancelEditButton, Localization.PivotGrid_CancelEditButton_ToolTip);
			CancelEditButton.Click += new RoutedEventHandler(CancelEditButton_Click);
			CancelEditButton.Content = UiHelper.CreateIcon(UriResources.Images.CancelEdit16);

			RestoreDefaultSizeButton = new RanetToolBarButton();
			ToolTipService.SetToolTip(RestoreDefaultSizeButton, Localization.PivotGrid_RestoreDefaultSize_ToolTip);
			RestoreDefaultSizeButton.Click += new RoutedEventHandler(RestoreDefaultSizeButton_Click);
			RestoreDefaultSizeButton.Content = UiHelper.CreateIcon(UriResources.Images.RestoreSize16);

			HideEmptyRowsButton = new RanetToggleButton();
			HideEmptyRowsButton.Click += new RoutedEventHandler(HideEmptyRowsButton_Click);
			HideEmptyRowsButton.Content = UiHelper.CreateIcon(UriResources.Images.HideEmptyRows16);
			ToolTipService.SetToolTip(HideEmptyRowsButton, Localization.PivotGrid_HideEmptyRowsButton_ToolTip);

			HideEmptyColumnsButton = new RanetToggleButton();
			HideEmptyColumnsButton.ClickMode = ClickMode.Press;
			HideEmptyColumnsButton.Click += new RoutedEventHandler(HideEmptyColumnsButton_Click);
			HideEmptyColumnsButton.Content = UiHelper.CreateIcon(UriResources.Images.HideEmptyColumns16);
			ToolTipService.SetToolTip(HideEmptyColumnsButton, Localization.PivotGrid_HideEmptyColumnsButton_ToolTip);

			GoToFocusedCellButton = new RanetToolBarButton();
			GoToFocusedCellButton.Click += new RoutedEventHandler(GoToFocusedCellButton_Click);
			GoToFocusedCellButton.Content = UiHelper.CreateIcon(UriResources.Images.ToFocused16);
			ToolTipService.SetToolTip(GoToFocusedCellButton, Localization.PivotGrid_GoToFocusedCellButton_ToolTip);

			ExportToExcelButton = new RanetToolBarButton();
			ToolTipService.SetToolTip(ExportToExcelButton, Localization.PivotGrid_ExportToExcelButton_ToolTip);
			ExportToExcelButton.Click += new RoutedEventHandler(ExportToExcelButton_Click);
			ExportToExcelButton.Content = UiHelper.CreateIcon(UriResources.Images.ExportToExcel16);

			RotateAxesButton = new RanetToggleButton();
			RotateAxesButton.ClickMode = ClickMode.Press;
			RotateAxesButton.Click += new RoutedEventHandler(RotateAxesButton_Click);
			RotateAxesButton.Content = UiHelper.CreateIcon(UriResources.Images.RotateAxes16);
			ToolTipService.SetToolTip(RotateAxesButton, Localization.PivotGrid_RotateAxesButton_ToolTip);

			HideHintsButton = new RanetToggleButton();
			HideHintsButton.ClickMode = ClickMode.Press;
			HideHintsButton.Click += new RoutedEventHandler(HideHintsButton_Click);
			HideHintsButton.Content = UiHelper.CreateIcon(UriResources.Images.HideHint16);
			ToolTipService.SetToolTip(HideHintsButton, Localization.PivotGrid_HideHintsButton_ToolTip);

			ConditionsDesignerButton = new RanetToggleButton();
			ConditionsDesignerButton.ClickMode = ClickMode.Press;
			ConditionsDesignerButton.Click += new RoutedEventHandler(ConditionsDesignerButton_Click);
			ConditionsDesignerButton.Content = UiHelper.CreateIcon(UriResources.Images.CellConditionsDesigner16);
			ConditionsDesignerButton.Visibility = Visibility.Collapsed;
			ToolTipService.SetToolTip(ConditionsDesignerButton, Localization.PivotGrid_CellsConditionsDesignerButton_ToolTip);

			ZoomControl = new ZoomingToolBarControl();
			ToolTipService.SetToolTip(ZoomControl, Localization.PivotGrid_ZoomingControl_ToolTip);
			ZoomControl.ValueChanged += new EventHandler(ZoomControl_ValueChanged);

			ToolBar.AddItem(RefreshButton);
			ToolBar.AddItem(m_NavigationButtons_Splitter);
			ToolBar.AddItem(ToBeginButton);
			ToolBar.AddItem(BackButton);
			ToolBar.AddItem(ForwardButton);
			ToolBar.AddItem(ToEndButton);
			NavigationToolBarSplitter = new RanetToolBarSplitter();
			ToolBar.AddItem(NavigationToolBarSplitter);
			ToolBar.AddItem(EditButton);
			ToolBar.AddItem(CopyToClipboardButton);
			ToolBar.AddItem(PasteFromClipboardButton);
			ToolBar.AddItem(UseChangesCasheButton);
			ToolBar.AddItem(ConfirmEditButton);
			ToolBar.AddItem(CancelEditButton);
			ToolBar.AddItem(new RanetToolBarSplitter());
			ToolBar.AddItem(RestoreDefaultSizeButton);
			ToolBar.AddItem(HideEmptyRowsButton);
			ToolBar.AddItem(HideEmptyColumnsButton);
			ToolBar.AddItem(GoToFocusedCellButton);
			ToolBar.AddItem(RotateAxesButton);
			ToolBar.AddItem(HideHintsButton);
			ToolBar.AddItem(ConditionsDesignerButton);
			ToolBar.AddItem(new RanetToolBarSplitter());
			ToolBar.AddItem(ExportToExcelButton);
			ToolBar.AddItem(ZoomControl);

			LayoutRoot.Children.Add(ToolBar);
			ToolBar.Margin = new Thickness(0, 0, 0, 3);

			// Сводная таблица
			//m_PivotGridPanel = new PivotGridPanel();
			//LayoutRoot.Children.Add(m_PivotGridPanel);
			//Grid.SetRow(m_PivotGridPanel, 1);

			m_PivotGrid = GetPivotGridControl();
			LayoutRoot.Children.Add(m_PivotGrid);
			Grid.SetRow(m_PivotGrid, 1);

			PivotGrid.ExecuteMemberAction += new MemberActionEventHandler(PivotGrid_ExecuteMemberAction);
			PivotGrid.CellValueChanged += new CellValueChangedEventHandler(CellsControl_CellValueChanged);
			PivotGrid.UndoCellChanges += new EventHandler(CellsControl_UndoCellChanges);
			PivotGrid.Cells_ContextMenuCreated += new EventHandler(CellsControl_ContextMenuCreated);
			PivotGrid.Columns_ContextMenuCreated += new EventHandler(ColumnsControl_ContextMenuCreated);
			PivotGrid.Rows_ContextMenuCreated += new EventHandler(RowsControl_ContextMenuCreated);
			PivotGrid.InitializeContextMenu += new EventHandler<CustomContextMenuEventArgs>(PivotGrid_InitializeContextMenu);

			UpdateToolbarButtons(null);
			UpdateEditToolBarButtons();

			grdIsWaiting = new Grid() { Background = new SolidColorBrush(Color.FromArgb(125, 0xFF, 0xFF, 0xFF)) };
			grdIsWaiting.Visibility = Visibility.Collapsed;
			BusyControl m_Waiting = new BusyControl();
			m_Waiting.Text = Localization.Loading;
			grdIsWaiting.Children.Add(m_Waiting);

			LayoutRoot.Children.Add(grdIsWaiting);
			Grid.SetColumnSpan(grdIsWaiting, LayoutRoot.ColumnDefinitions.Count > 0 ? LayoutRoot.ColumnDefinitions.Count : 1);
			Grid.SetRowSpan(grdIsWaiting, LayoutRoot.RowDefinitions.Count > 0 ? LayoutRoot.RowDefinitions.Count : 1);
			this.Content = LayoutRoot;

			PivotGrid.Cells_PerformControlAction += new EventHandler<ControlActionEventArgs<CellInfo>>(CellsControl_PerformControlAction);
			PivotGrid.Members_PerformControlAction += new EventHandler<ControlActionEventArgs<MemberControl>>(MembersArea_PerformControlAction);

			m_OlapDataLoader = GetOlapDataLoader();
			m_OlapDataLoader.DataLoaded += new EventHandler<DataLoaderEventArgs>(OlapDataLoader_DataLoaded);

			m_StorageManager = GetStorageManager();
			m_StorageManager.InvokeCompleted += new EventHandler<DataLoaderEventArgs>(StorageManager_ActionCompleted);

			// Метод Initialize необходимо вызывать для RotVisual элемента
			// всего приложения. Перенес его в ClientApp.
			//ScrollViewerMouseWheelSupport.Initialize(this);
			//m_ScrollableDataControl.ScrollView.AddMouseWheelSupport();

			this.KeyDown += new KeyEventHandler(UpdateablePivotGridControl_KeyDown);

			OlapTransactionManager.AfterCommandComplete += new EventHandler<TransactionCommandResultEventArgs>(AnalysisTransactionManager_AfterCommandComplete);
		}

		SortDescriptor GetAxisPropertySort(MemberControl member)
		{
			return PivotGrid.GetAxisPropertySort(member);
		}

		void PivotGrid_InitializeContextMenu(object sender, CustomContextMenuEventArgs e)
		{
			if (e != null && e.GridItem != null && e.Menu != null)
			{
				MemberControl member_control = e.GridItem as MemberControl;
				if (member_control != null)
				{
					foreach (UIElement element in e.Menu.Items)
					{
						ContextMenuItem menu_item = element as ContextMenuItem;
						if (menu_item != null)
						{
							if (menu_item.Tag is ControlActionType)
							{
								ControlActionType action = (ControlActionType)menu_item.Tag;
								switch (action)
								{
									case ControlActionType.SortingByProperty:
										// Иконка для пункта меню - Сортировка по свойству
										var property_sort = GetAxisPropertySort(member_control);
										switch (property_sort != null ? property_sort.Type : SortTypes.None)
										{
											case SortTypes.Ascending:
												menu_item.Icon = UriResources.Images.SortAZ16;
												break;
											case SortTypes.Descending:
												menu_item.Icon = UriResources.Images.SortZA16;
												break;
											default:
												menu_item.Icon = null;
												break;
										}
										break;
									case ControlActionType.SortingAxisByMeasure:
										// Иконка для пункта меню - Сортировка по значению
										var measure_sort = GetAxisByMeasureSort(member_control) as SortByMeasureDescriptor;
										switch (measure_sort != null ? measure_sort.Type : SortTypes.None)
										{
											case SortTypes.Ascending:
												menu_item.Icon = UriResources.Images.SortAZ16;
												break;
											case SortTypes.Descending:
												menu_item.Icon = UriResources.Images.SortZA16;
												break;
											default:
												menu_item.Icon = null;
												break;
										}
										break;
									case ControlActionType.SortingByValue:
										menu_item.Icon = null;
										if (member_control is ColumnMemberControl || member_control is RowMemberControl)
										{
											var value_sort = GetAxisSort(member_control is ColumnMemberControl ? AreaType.RowsArea : AreaType.ColumnsArea) as SortByValueDescriptor;
											if (value_sort != null && member_control.Member != null && value_sort.CompareByTuple(member_control.Member.GetAxisTuple()))
											{
												switch (value_sort.Type)
												{
													case SortTypes.Ascending:
														menu_item.Icon = UriResources.Images.SortAZ16;
														break;
													case SortTypes.Descending:
														menu_item.Icon = UriResources.Images.SortZA16;
														break;
													default:
														menu_item.Icon = null;
														break;
												}
											}
										}
										break;
									case ControlActionType.ClearAxisSorting:
										// Доступность пункта меню - Удалить сортировку
										var property_sort1 = GetAxisPropertySort(member_control);
										var measure_sort1 = GetAxisByMeasureSort(member_control);
										SortTypes property_sort_type = property_sort1 != null ? property_sort1.Type : SortTypes.None;
										SortTypes measure_sort_type = measure_sort1 != null ? measure_sort1.Type : SortTypes.None;

										menu_item.IsEnabled = property_sort_type != SortTypes.None || measure_sort_type != SortTypes.None;
										break;
								}
							}
						}
					}
				}
			}

			if (e.Menu != null)
			{
				foreach (UIElement element in e.Menu.Items)
				{
					var menu_item = element as ContextMenuItem;
					if (menu_item != null && menu_item.Tag is ControlActionType)
					{
						switch ((ControlActionType)(menu_item.Tag))
						{
							case ControlActionType.DataReorganizationType:
								if (menu_item.SubMenu != null)
								{
									foreach (UIElement child in menu_item.SubMenu.Items)
									{
										var checked_item = child as CheckedContectMenuItem;
										if (checked_item != null && checked_item.Tag is ControlActionType)
										{
											switch ((ControlActionType)(checked_item.Tag))
											{
												case ControlActionType.DataReorganizationType_None:
													checked_item.IsChecked = DataReorganizationType == DataReorganizationTypes.None;
													break;
												case ControlActionType.DataReorganizationType_HitchToParent:
													checked_item.IsChecked = DataReorganizationType == DataReorganizationTypes.LinkToParent;
													break;
												case ControlActionType.DataReorganizationType_MergeNeighbors:
													checked_item.IsChecked = DataReorganizationType == DataReorganizationTypes.MergeNeighbors;
													break;
											}
										}
									}
								}
								break;
						}
					}
				}
			}
		}

		void StorageManager_ActionCompleted(object sender, DataLoaderEventArgs e)
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
				if (args.ActionType == StorageActionTypes.Load)
				{
					List<String> list = XmlSerializationUtility.XmlStr2Obj<List<String>>(e.Result.Content);
					List<CellConditionsDescriptor> conditions = new List<CellConditionsDescriptor>();
					if (list != null)
					{
						foreach (var item in list)
						{
							CellConditionsDescriptor descr = CellConditionsDescriptor.Deserialize(item);
							if (descr != null)
								conditions.Add(descr);
						}
						m_CustomCellConditionsEditor.Initialize(conditions);
					}
				}
			}
		}

		ModalDialog m_ConditionsDesignerDialog = null;
		CustomCellConditionsEditor m_CustomCellConditionsEditor = null;
		void ConditionsDesignerButton_Click(object sender, RoutedEventArgs e)
		{
			if (m_ConditionsDesignerDialog == null)
			{
				m_ConditionsDesignerDialog = new ModalDialog() { Width = 700, Height = 600, DialogStyle = ModalDialogStyles.OKCancel };
				m_ConditionsDesignerDialog.Caption = Localization.CellsConditionsDesignerDialog_Caption;
				m_ConditionsDesignerDialog.DialogOk += new EventHandler<DialogResultArgs>(m_ConditionsDesignerDialog_DialogOk);
			}

			if (m_CustomCellConditionsEditor == null)
			{
				m_CustomCellConditionsEditor = new CustomCellConditionsEditor();
				m_CustomCellConditionsEditor.StorageManager = StorageManager;
				m_CustomCellConditionsEditor.LogManager = LogManager;
				m_ConditionsDesignerDialog.Content = m_CustomCellConditionsEditor;
				m_CustomCellConditionsEditor.SaveStyles += new EventHandler<CustomEventArgs<ObjectDescription>>(m_CustomCellConditionsEditor_SaveStyles);
				m_CustomCellConditionsEditor.LoadStyles += new EventHandler<CustomEventArgs<ObjectStorageFileDescription>>(m_CustomCellConditionsEditor_LoadStyles);
			}

			m_CustomCellConditionsEditor.Initialize(CustomCellsConditions != null ? CustomCellsConditions.ToList<CellConditionsDescriptor>() : new List<CellConditionsDescriptor>());
			ShowDialog(m_ConditionsDesignerDialog);
		}

		void m_CustomCellConditionsEditor_LoadStyles(object sender, CustomEventArgs<ObjectStorageFileDescription> e)
		{
			if (e.Args != null)
			{
				StorageActionArgs args = new StorageActionArgs();
				args.ActionType = StorageActionTypes.Load;
				args.ContentType = StorageContentTypes.CustomCellStyles;
				args.FileDescription = e.Args;
				if (StorageManager != null)
				{
					StorageManager.Invoke(XmlSerializationUtility.Obj2XmlStr(args, Constants.XmlNamespace), args);
				}
			}
		}

		void m_CustomCellConditionsEditor_SaveStyles(object sender, CustomEventArgs<ObjectDescription> e)
		{
			ObjectDescription descr = e.Args;
			if (descr != null)
			{
				StorageActionArgs args = new StorageActionArgs();
				args.ActionType = StorageActionTypes.Save;
				List<String> list = new List<string>();
				foreach (var cond in m_CustomCellConditionsEditor.CellsConditions)
				{
					list.Add(cond.Serialize());
				}
				args.Content = XmlSerializationUtility.Obj2XmlStr(list);
				args.ContentType = StorageContentTypes.CustomCellStyles;
				if (StorageManager != null)
				{
					if (String.IsNullOrEmpty(descr.Caption))
						descr.Caption = descr.Name;
					args.FileDescription = new ObjectStorageFileDescription(descr);
					StorageManager.Invoke(XmlSerializationUtility.Obj2XmlStr(args, Constants.XmlNamespace), args);

				}
			}
		}

		void m_ConditionsDesignerDialog_DialogOk(object sender, DialogResultArgs e)
		{
			if (m_CustomCellConditionsEditor != null)
			{
				m_CustomCellConditionsEditor.EndEdit();
				CustomCellsConditions = m_CustomCellConditionsEditor.CellsConditions;
				PivotGrid.Refresh();
			}
		}

		~UpdateablePivotGridControl()
		{
			OlapTransactionManager.AfterCommandComplete -= new EventHandler<TransactionCommandResultEventArgs>(AnalysisTransactionManager_AfterCommandComplete);
		}

		protected virtual void AnalysisTransactionManager_AfterCommandComplete(object sender, TransactionCommandResultEventArgs e)
		{
			if (e.Connection == this.Connection)
			{
				PivotGrid.LocalChanges.Clear();
				Refresh();
			}
		}

		void UpdateablePivotGridControl_KeyDown(object sender, KeyEventArgs e)
		{
			ServiceCommandType service_Command = ServiceCommandType.None;
			switch (e.Key)
			{
				case Key.Right:
					// Ctrl+Right - навигация на одну запись в истории вперед
					if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control &&
							(Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift)
					{
						if (ForwardButton.IsEnabled)
							service_Command = ServiceCommandType.Forward;
					}
					// Ctrl+Shift+Right - навигация на последнюю запись в истории
					if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control &&
							(Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
					{
						if (ToEndButton.IsEnabled)
							service_Command = ServiceCommandType.ToEnd;
					}
					break;
				case Key.Left:
					// Ctrl+Left - навигация на одну запись в истории назад
					if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control &&
							(Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift)
					{
						if (BackButton.IsEnabled)
							service_Command = ServiceCommandType.Back;
					}
					// Ctrl+Sift+Left - навигация на первую запись в истории
					if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control &&
							(Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
					{
						if (ToBeginButton.IsEnabled)
							service_Command = ServiceCommandType.ToBegin;
					}
					break;
			}

			if (service_Command != ServiceCommandType.None)
			{
				RunServiceCommand(service_Command);
				e.Handled = true;
			}
		}

		void ZoomControl_ValueChanged(object sender, EventArgs e)
		{
			PivotGrid.Scale = ZoomControl.Value / 100;
		}

		void HideHintsButton_Click(object sender, RoutedEventArgs e)
		{
			PivotGrid.Cells_UseHint = PivotGrid.Rows_UseHint = PivotGrid.Columns_UseHint = !HideHintsButton.IsChecked.Value;
		}

		protected virtual PivotQueryManager GetDataManager()
		{
			//return new PivotDataManager(new ConnectionInfo(Connection, Connection), Query, UpdateScript);
			return new PivotQueryManager(Query, UpdateScript);
		}

		protected virtual IDataLoader GetOlapDataLoader()
		{
			return new OlapDataLoader(URL);
		}

		protected virtual PivotGridControl GetPivotGridControl()
		{
			return new PivotGridControl();
		}

		void RotateAxesButton_Click(object sender, RoutedEventArgs e)
		{
			if (RotateAxesButton.IsChecked.Value)
			{
				RunServiceCommand(ServiceCommandType.RotateAxes);
			}
			else
			{
				RunServiceCommand(ServiceCommandType.NormalAxes);
			}
		}

		public override string URL
		{
			get
			{
				return base.URL;
			}
			set
			{
				base.URL = value;

				OlapDataLoader metadataLoader = OlapDataLoader as OlapDataLoader;
				if (metadataLoader != null)
				{
					metadataLoader.URL = value;
				}
			}
		}

		void GoToFocusedCellButton_Click(object sender, RoutedEventArgs e)
		{
			PivotGrid.GoToFocusedCell();
		}

		void PivotGrid_ExecuteMemberAction(object sender, MemberActionEventArgs args)
		{
			// Определяем нужно ли выполнить экшен по умолчанию
			if (args.Action == MemberActionType.Default)
			{
				var MemberAction = (ColumnTitleClickBehavior)(int)DefaultMemberAction;
				if (args.Axis == 0)
					if (this.ColumnTitleClickBehavior != ColumnTitleClickBehavior.SameAsRows)
					{
						MemberAction = this.ColumnTitleClickBehavior;
					}

				switch (MemberAction)
				{
					case ColumnTitleClickBehavior.None:
						return;
					case ColumnTitleClickBehavior.DrillDown:
						args.Action = MemberActionType.DrillDown;
						break;
					case ColumnTitleClickBehavior.ExpandCollapse:
						if (args.Member != null && args.Member.ChildCount > 0 && !args.Member.IsCalculated)
						{
							args.Action = MemberActionType.Expand;
							if (args.Member.DrilledDown)
								args.Action = MemberActionType.Collapse;
							break;
						}
						return;
					case ColumnTitleClickBehavior.SortByValue:
						if (args.Member.Children.Count > 0)
							return;

						args.Action = MemberActionType.SortByValue;
						break;

					case ColumnTitleClickBehavior.SortByProperty:
						if (args.Member != null && (args.Axis == 0 || args.Axis == 1))
						{
							SortDescriptor new_descr = new SortDescriptor();
							new_descr.SortBy = "Caption";
							new_descr.Type = SortTypes.Ascending;
							SortDescriptor descr = PivotGrid.GetAxisPropertySort(args.Axis, args.Member);
							if (descr != null)
							{
								if (!String.IsNullOrEmpty(descr.SortBy))
								{
									new_descr.SortBy = descr.SortBy;
								}
								switch (descr.Type)
								{
									case SortTypes.None:
										new_descr.Type = SortTypes.Ascending;
										break;
									case SortTypes.Ascending:
										new_descr.Type = SortTypes.Descending;
										break;
									case SortTypes.Descending:
										new_descr.Type = SortTypes.None;
										break;
								}
							}

							m_CellSetProvider.Sort(args.Axis, args.Member.HierarchyUniqueName, new_descr);

							try
							{
								IsWaiting = true;
								PivotGrid.Initialize(m_CellSetProvider);
							}
							finally
							{
								IsWaiting = false;
							}
						}
						return;
				}
			}

			if (args.Axis == 0 || args.Axis == 1)
			{
				ExportSizeInfo();
				PerformMemberAction(args);
			}
		}

		void PasteFromClipboardoButton_Click(object sender, RoutedEventArgs e)
		{
			//FAST PasteCellsFromClipboard(PivotPanelControl.CellsControl.FocusedCell);
		}

		void CopyToClipboardoButton_Click(object sender, RoutedEventArgs e)
		{
			CopyCellsToClipboard(null);
		}

		public bool ExportToExcelFile = true;

		void ExportToExcelButton_Click(object sender, RoutedEventArgs e)
		{
			OnExportToExcelClick();
			try
			{
				bool? ret = new bool?(true);
				if (ExportToExcelFile)
				{
					ret = SaveFileDialog.ShowDialog();
				}
				if (ret.HasValue && ret.Value == true)
				{
					RunServiceCommand(ServiceCommandType.ExportToExcel);
				}
			}
			catch (Exception ex)
			{
				LogManager.LogError(this, ex.ToString());
			}
		}

		void RestoreDefaultSizeButton_Click(object sender, RoutedEventArgs e)
		{
			ZoomControl.Value = 100;
			PivotGrid.RestoreDefaultSize();
			ExportSizeInfo();
		}

		RanetToolBarSplitter m_NavigationButtons_Splitter = new RanetToolBarSplitter();

		/// <summary>
		/// Выполняет откат изменений для указанной ячейки
		/// </summary>
		void CellsControl_UndoCellChanges(object sender, EventArgs e)
		{
			if (UseChangesCashe)
			{
				foreach (CellInfo cell in PivotGrid.Selection)
				{
					PivotGrid.LocalChanges.RemoveChange(cell);
				}
				UpdateEditToolBarButtons();
			}
		}

		void ToBeginButton_Click(object sender, RoutedEventArgs e)
		{
			RunServiceCommand(ServiceCommandType.ToBegin);
		}

		void ToEndButton_Click(object sender, RoutedEventArgs e)
		{
			RunServiceCommand(ServiceCommandType.ToEnd);
		}

		ModalDialog m_SortDialog;
		ModalDialog m_SortByValueDialog;
		SortPropertiesControl m_SortByValueSettingsControl;
		SortPropertiesControl m_SortSettingsControl;

		void MembersArea_PerformControlAction(object sender, ControlActionEventArgs<MemberControl> e)
		{
			ModalDialog dlg;
			MemberInfo info = e.UserData != null ? e.UserData.Member : null;
			switch (e.Action)
			{
				case ControlActionType.ShowMDX:
					ShowDataSourceInfo(GetDataSourceInfo(null));
					break;
				case ControlActionType.ShowProperties:
					dlg = new ModalDialog() { Width = 400, Height = 300, DialogStyle = ModalDialogStyles.OK };
					MemberPropertiesControl properties = new MemberPropertiesControl();
					properties.Initialize(info);
					dlg.Content = properties;
					dlg.Caption = Localization.MemberPropertiesDialog_Caption;
					Panel panel = GetRootPanel(this);
					if (panel != null)
					{
						panel.Children.Add(dlg.Dialog.PopUpControl);
					}
					ShowDialog(dlg);
					break;
				case ControlActionType.ShowAttributes:
					ShowMemberAttributes(info);
					break;
				case ControlActionType.SortingByProperty:
					ShowSortPropertiesDialog(PivotTableSortTypes.SortByProperty, e.UserData);
					break;
				case ControlActionType.SortingAxisByMeasure:
					ShowSortPropertiesDialog(PivotTableSortTypes.SortAxisByMeasure, e.UserData);
					break;
				case ControlActionType.SortingByValue:
					ShowSortPropertiesDialog(PivotTableSortTypes.SortByValue, e.UserData);
					break;
				case ControlActionType.ClearAxisSorting:
					if (m_CellSetProvider != null)
					{
						if (e.UserData is ColumnMemberControl)
						{
							if (PivotGrid.AxisIsRotated == false)
								m_CellSetProvider.ClearSort(0);
							else
								m_CellSetProvider.ClearSort(1);
						}
						if (e.UserData is RowMemberControl)
						{
							if (PivotGrid.AxisIsRotated == false)
								m_CellSetProvider.ClearSort(1);
							else
								m_CellSetProvider.ClearSort(0);
						}
					}
					bool refreshed = false;
					if (DataManager != null)
					{
						if (DataManager.Axis0_MeasuresSort != null || DataManager.Axis1_MeasuresSort != null)
						{
							SetAxisByMeasureSort(e.UserData, null);
							refreshed = true;
							RunServiceCommand(ServiceCommandType.Refresh);
						}
					}

					if (!refreshed)
					{
						try
						{
							IsWaiting = true;
							PivotGrid.Initialize(m_CellSetProvider);
						}
						finally
						{
							IsWaiting = false;
						}
					}
					break;
				case ControlActionType.DataReorganizationType_None:
					ChangeDataReorganizationType(DataReorganizationTypes.None);
					break;
				case ControlActionType.DataReorganizationType_HitchToParent:
					ChangeDataReorganizationType(DataReorganizationTypes.LinkToParent);
					break;
				case ControlActionType.DataReorganizationType_MergeNeighbors:
					ChangeDataReorganizationType(DataReorganizationTypes.MergeNeighbors);
					break;
			}
		}

		void ChangeDataReorganizationType(DataReorganizationTypes type)
		{
			if (DataReorganizationType != type)
			{
				DataReorganizationType = type;
				Initialize(m_CSDescr);
			}
		}

		void ShowSortPropertiesDialog(PivotTableSortTypes sortType, MemberControl member)
		{
			ModalDialog dlg;
			SortPropertiesControl sortControl;
			if (sortType == PivotTableSortTypes.SortByValue)
			{
				// Сортировку по значению делаем отдельным диалогом чтобы не думать о сохранении размеров
				if (m_SortByValueDialog == null)
				{
					m_SortByValueDialog = new ModalDialog() { Width = 500, Height = 400, MinHeight = 250, MinWidth = 180, DialogStyle = ModalDialogStyles.OKCancel };
					m_SortByValueDialog.Caption = Localization.SortingSettingsDialog_Caption;
					m_SortByValueDialog.DialogOk += new EventHandler<DialogResultArgs>(SortProperties_DialogOk);
					Panel panel1 = GetRootPanel(this);
					if (panel1 != null)
					{
						panel1.Children.Add(m_SortByValueDialog.Dialog.PopUpControl);
					}
				}

				if (m_SortByValueSettingsControl == null)
				{
					m_SortByValueSettingsControl = new SortPropertiesControl();
					m_SortByValueSettingsControl.CubeName = m_CSDescr != null ? m_CSDescr.CubeName : String.Empty;
					m_SortByValueSettingsControl.ConnectionID = m_CSDescr != null ? m_CSDescr.Connection.ConnectionID : String.Empty;
					m_SortByValueSettingsControl.GetOlapDataLoader += new EventHandler<GetIDataLoaderArgs>(CopyControl_GetMetadataLoader);
					m_SortByValueSettingsControl.LogManager = this.LogManager;

					m_SortByValueSettingsControl.LoadMeasures += new EventHandler<CustomEventArgs<EventArgs>>(m_SortSettingsControl_LoadMeasures);
					m_SortByValueDialog.Content = m_SortByValueSettingsControl;
				}
				sortControl = m_SortByValueSettingsControl;
				dlg = m_SortByValueDialog;
			}
			else
			{
				if (m_SortDialog == null)
				{
					m_SortDialog = new ModalDialog() { Width = 400, Height = 250, MinHeight = 215, MinWidth = 180, DialogStyle = ModalDialogStyles.OKCancel };
					m_SortDialog.Caption = Localization.SortingSettingsDialog_Caption;
					m_SortDialog.DialogOk += new EventHandler<DialogResultArgs>(SortProperties_DialogOk);
					Panel panel1 = GetRootPanel(this);
					if (panel1 != null)
					{
						panel1.Children.Add(m_SortDialog.Dialog.PopUpControl);
					}
				}

				if (m_SortSettingsControl == null)
				{
					m_SortSettingsControl = new SortPropertiesControl();
					m_SortSettingsControl.CubeName = m_CSDescr != null ? m_CSDescr.CubeName : String.Empty;
					m_SortSettingsControl.ConnectionID = m_CSDescr != null ? m_CSDescr.Connection.ConnectionID : String.Empty;
					m_SortSettingsControl.GetOlapDataLoader += new EventHandler<GetIDataLoaderArgs>(CopyControl_GetMetadataLoader);
					m_SortSettingsControl.LogManager = this.LogManager;

					m_SortSettingsControl.LoadMeasures += new EventHandler<CustomEventArgs<EventArgs>>(m_SortSettingsControl_LoadMeasures);
					m_SortDialog.Content = m_SortSettingsControl;
				}
				sortControl = m_SortSettingsControl;
				dlg = m_SortDialog;
			}

			if (dlg == null || sortControl == null)
				return;

			dlg.Tag = sortControl;
			sortControl.Tag = member;

			SortDescriptor sort = null;
			switch (sortType)
			{
				case PivotTableSortTypes.SortByProperty:
					if (m_CellSetProvider != null &&
									member != null &&
									member.Member != null)
					{
						sort = GetAxisPropertySort(member);
					}
					break;
				case PivotTableSortTypes.SortAxisByMeasure:
					sort = GetAxisByMeasureSort(member);
					break;
				case PivotTableSortTypes.SortByValue:
					// Получаем сортировку для противоположной оси. Если это сортировка по значению, то проверяем чтобы это была сортировка для данного элемента. Если нет, то создается новый дескриптор
					if (member is ColumnMemberControl || member is RowMemberControl)
					{
						var value_sort = GetAxisSort(member is ColumnMemberControl ? AreaType.RowsArea : AreaType.ColumnsArea) as SortByValueDescriptor;
						if (value_sort != null && member != null && member.Member != null && value_sort.CompareByTuple(member.Member.GetAxisTuple()))
						{
							sort = value_sort;
						}
					}

					if (sort == null)
					{
						sort = new SortByValueDescriptor();
						if (member != null && member.Member != null)
						{
							(sort as SortByValueDescriptor).Tuple = member.Member.GetAxisTuple();
						}
					}
					break;
			}

			sortControl.Initialize(sortType, sort);
			ShowDialog(dlg);
		}

		SortDescriptor GetAxisByMeasureSort(MemberControl member)
		{
			if (member is ColumnMemberControl)
				return GetAxisSort(AreaType.ColumnsArea);
			if (member is RowMemberControl)
				return GetAxisSort(AreaType.RowsArea);
			return null;
		}

		void SetAxisByMeasureSort(MemberControl member, SortDescriptor sort)
		{
			if (member is ColumnMemberControl)
				SetAxisSort(AreaType.ColumnsArea, sort);
			if (member is RowMemberControl)
				SetAxisSort(AreaType.RowsArea, sort);
		}

		SortDescriptor GetAxisSort(AreaType area)
		{
			if (DataManager != null)
			{
				if (area == AreaType.ColumnsArea)
				{
					return PivotGrid.AxisIsRotated == false ? DataManager.Axis0_MeasuresSort : DataManager.Axis1_MeasuresSort;
				}
				if (area == AreaType.RowsArea)
				{
					return PivotGrid.AxisIsRotated == false ? DataManager.Axis1_MeasuresSort : DataManager.Axis0_MeasuresSort;
				}
			}
			return null;
		}

		void SetAxisSort(AreaType area, SortDescriptor sort)
		{
			if (DataManager != null)
			{
				if (area == AreaType.ColumnsArea)
				{
					if (PivotGrid.AxisIsRotated == false)
						DataManager.Axis0_MeasuresSort = sort;
					else
						DataManager.Axis1_MeasuresSort = sort;
				}
				if (area == AreaType.RowsArea)
				{
					if (PivotGrid.AxisIsRotated == false)
						DataManager.Axis1_MeasuresSort = sort;
					else
						DataManager.Axis0_MeasuresSort = sort;
				}
			}
		}

		void m_SortSettingsControl_LoadMeasures(object sender, CustomEventArgs<EventArgs> e)
		{
			SortPropertiesControl sortControl = sender as SortPropertiesControl;
			if (sortControl != null && m_CellSetProvider != null &&
		m_CellSetProvider.CellSet_Description != null &&
	!String.IsNullOrEmpty(m_CellSetProvider.CellSet_Description.CubeName))
			{
				LogManager.LogInformation(this, this.Name + String.Format(" - Loading cube '{0}' measures", m_CellSetProvider.CellSet_Description.CubeName));
				MetadataQuery args = CommandHelper.CreateGetCubeMetadataArgs(Connection, m_CellSetProvider.CellSet_Description.CubeName, MetadataQueryType.GetMeasures);
				OlapDataLoader.LoadData(args, sortControl);
			}
			else
			{
				e.Cancel = true;
			}
		}

		void SortProperties_DialogOk(object sender, DialogResultArgs e)
		{
			// Сортировка по свойству применяется к той оси, на которой вызывалось меню
			// Сортировка по мере применяется к той оси, на которой вызывалось меню
			// Сортировка по значению применяется к противоположной оси

			// Если устанавливается сортировка по свойству:
			//  - На противоположной оси отменяется сортировка по значению

			// Если устанавливается сортировка по значению:
			//  - На противоположной оси отменяются все сортировки по свойству

			var dlg = sender as ModalDialog;
			if (dlg == null)
				return;
			var sortControl = dlg.Tag as SortPropertiesControl;
			if (sortControl == null)
				return;

			if (sortControl.SortType == PivotTableSortTypes.SortAxisByMeasure &&
							sortControl.SortDescriptor.Type != SortTypes.None &&
							String.IsNullOrEmpty(sortControl.SortDescriptor.SortBy))
			{
				MessageBox.Show(Localization.SortingPropertiesDialog_MeasureNotSelected_Message, Localization.Warning, MessageBoxButton.OK);
				e.Cancel = true;
				return;
			}

			MemberControl member = sortControl.Tag as MemberControl;
			if (member != null && member.Member != null)
			{
				int axisNum = -1;
				if (member is ColumnMemberControl)
					axisNum = 0;
				if (member is RowMemberControl)
					axisNum = 1;

				if (m_CellSetProvider != null && axisNum != -1)
				{
					// Сортировка по свойству
					if (sortControl.SortType == PivotTableSortTypes.SortByProperty)
					{
						// Если на противоположной оси нет сортировки по значению

						m_CellSetProvider.Sort(axisNum, member.Member.HierarchyUniqueName, sortControl.SortDescriptor);
						try
						{
							IsWaiting = true;
							// Отменяем сортировку по значению для противоположной оси
							SetAxisSort(axisNum == 0 ? AreaType.RowsArea : AreaType.ColumnsArea, null);
							PivotGrid.Initialize(m_CellSetProvider);
						}
						finally
						{
							IsWaiting = false;
						}
					}

					// Сортировка по значению
					if (sortControl.SortType == PivotTableSortTypes.SortAxisByMeasure)
					{
						if (DataManager != null)
						{
							// Новая сортировка для оси
							SortDescriptor descr = null;
							if (sortControl.SortDescriptor.Type != SortTypes.None &&
													 !String.IsNullOrEmpty(sortControl.SortDescriptor.SortBy))
								descr = sortControl.SortDescriptor;
							if (GetAxisByMeasureSort(member) != null || descr != null)
							{
								SetAxisByMeasureSort(member, descr);

								// Сортируем новым запросом
								RunServiceCommand(ServiceCommandType.Refresh);
							}
						}
					}

					if (sortControl.SortType == PivotTableSortTypes.SortByValue)
					{
						// Новая сортировка для противоположной оси
						SortDescriptor descr = null;
						if (sortControl.SortDescriptor.Type != SortTypes.None &&
												 !String.IsNullOrEmpty(sortControl.SortDescriptor.SortBy))
							descr = sortControl.SortDescriptor;
						// Проверяем сортировку противоположной оси
						if (GetAxisSort(axisNum == 0 ? AreaType.RowsArea : AreaType.ColumnsArea) != null || descr != null)
						{
							SetAxisSort(axisNum == 0 ? AreaType.RowsArea : AreaType.ColumnsArea, descr);

							// Сортируем новым запросом
							RunServiceCommand(ServiceCommandType.Refresh);
						}
					}
				}
			}
		}

		/// <summary>
		/// Кэш атрибутов для иерархии
		/// </summary>
		Dictionary<String, List<LevelPropertyInfo>> m_LevelProperties = new Dictionary<string, List<LevelPropertyInfo>>();

		void ShowMemberAttributes(MemberInfo member)
		{
			if (member != null)
			{
				if (!m_LevelProperties.ContainsKey(member.HierarchyUniqueName))
				{
					IsWaiting = true;
					LogManager.LogInformation(this, this.Name + " - Loading level attributes.");
					MetadataQuery args = CommandHelper.CreateLoadLevelPropertiesArgs(Connection, m_CSDescr.CubeName, String.Empty, member.HierarchyUniqueName, String.Empty);
					OlapDataLoader.LoadData(args, new MemberInfoWrapper<MetadataQuery>(member, args));
				}
				else
				{
					LoadMemberAttributes(member, m_LevelProperties[member.HierarchyUniqueName]);
				}
			}
		}

		ModalDialog m_DrillthroughDialog = null;
		void ShowDrillthroughResult(CellInfo cell, DataTableWrapper tableWrapper)
		{
			if (tableWrapper != null)
			{
				if (m_DrillthroughDialog == null)
				{
					m_DrillthroughDialog = new ModalDialog() { Width = 600, Height = 500, DialogStyle = ModalDialogStyles.OK };
					m_DrillthroughDialog.Caption = Localization.DrillthroughDialog_Caption;
				}
				//RanetDataGrid grid = new RanetDataGrid();
				//grid.Initialize(tableWrapper);

				DrillThroughControl grid = new DrillThroughControl();
				grid.Initialize(cell, tableWrapper);
				m_DrillthroughDialog.Content = grid;
				Panel panel = GetRootPanel(this);
				if (panel != null && !panel.Children.Contains(m_DrillthroughDialog.Dialog.PopUpControl))
				{
					panel.Children.Add(m_DrillthroughDialog.Dialog.PopUpControl);
				}
				ShowDialog(m_DrillthroughDialog);
			}
		}

		void ShowMemberAttributes(MemberData member)
		{
			if (member != null)
			{
				ModalDialog dlg = new ModalDialog() { Width = 400, Height = 300, DialogStyle = ModalDialogStyles.OK };
				MemberPropertiesControl properties = new MemberPropertiesControl();
				properties.Initialize(member);
				dlg.Content = properties;
				dlg.Caption = Localization.CustomPropertiesDialog_Caption;
				Panel panel = GetRootPanel(this);
				if (panel != null)
				{
					panel.Children.Add(dlg.Dialog.PopUpControl);
				}
				ShowDialog(dlg);
			}
		}

		/// <summary>
		/// Открывает диалог и обрабатывает блокировку подсказки для элементов сводной таблицы
		/// </summary>
		/// <param name="dlg"></param>
		void ShowDialog(ModalDialog dlg)
		{
			if (dlg != null)
			{
				// На время убираем контекстное меню сводной таблицы
				PivotGrid.UseContextMenu = false;
				// На время убираем всплывающую подсказку
				PivotGrid.TooltipManager.IsPaused = true;
				dlg.Show();
				dlg.DialogClosed -= new EventHandler<DialogResultArgs>(ModalDialog_DialogClosed);
				dlg.DialogClosed += new EventHandler<DialogResultArgs>(ModalDialog_DialogClosed);
			}
		}

		void ModalDialog_DialogClosed(object sender, DialogResultArgs e)
		{
			// Восстанавливаем работу подсказок
			PivotGrid.TooltipManager.IsPaused = false;
			// Восстанавливаем работу контекстного меню
			PivotGrid.UseContextMenu = true;
		}

		void LoadMemberAttributes(MemberInfo member, List<LevelPropertyInfo> properties)
		{
			IsWaiting = true;

			QueryProvider provider = new QueryProvider(m_CSDescr.CubeName, String.Empty, member.HierarchyUniqueName);
			String query = provider.GetMember(member.UniqueName, properties);
			LogManager.LogInformation(this, this.Name + " - Loading custom member properties.");
			MdxQueryArgs query_args = CommandHelper.CreateMdxQueryArgs(Connection, query);
			OlapDataLoader.LoadData(query_args, "CUSTOM_MEMBER_PROPERTIES");
		}


		Panel GetRootPanel(FrameworkElement element)
		{
			Panel panel = null;
			while (element != null && element.Parent != null)
			{
				if (element.Parent is Panel)
					panel = element.Parent as Panel;
				element = element.Parent as FrameworkElement;
			}
			return panel;
		}

		void CellsControl_PerformControlAction(object sender, ControlActionEventArgs<CellInfo> e)
		{
			switch (e.Action)
			{
				case ControlActionType.ShowMDX:
					UpdateEntry entry = new UpdateEntry(e.UserData);
					try
					{
						if (e.UserData != null)
							entry.OldValue = e.UserData.CellDescr.Value.Value.ToString();
					}
					catch { }

					UpdateEntry change = PivotGrid.LocalChanges.FindChange(e.UserData);
					if (change != null)
					{
						entry.NewValue = change.NewValue;
					}

					ShowDataSourceInfo(GetDataSourceInfo(entry));
					break;
				case ControlActionType.ShowProperties:
					ModalDialog dlg = new ModalDialog() { Width = 400, Height = 300, DialogStyle = ModalDialogStyles.OK };
					CellPropertiesControl properties = new CellPropertiesControl();
					properties.Initialize(e.UserData);
					dlg.Content = properties;
					dlg.Caption = Localization.CellPropertiesDialog_Caption;
					Panel panel = GetRootPanel(this);
					if (panel != null)
					{
						panel.Children.Add(dlg.Dialog.PopUpControl);
					}
					ShowDialog(dlg);
					break;
				case ControlActionType.ValueDelivery:
					ShowValueDeliveryDialog(e.UserData);
					break;
				case ControlActionType.ValueCopy:
					ShowValueCopyDialog(e.UserData);
					break;
				case ControlActionType.Copy:
					CopyCellsToClipboard(e.UserData);
					break;
				case ControlActionType.Paste:
					PasteCellsFromClipboard(e.UserData);
					break;
				case ControlActionType.DrillThrough:
					DrillThroughCell(e.UserData);
					break;
				case ControlActionType.DataReorganizationType_None:
					ChangeDataReorganizationType(DataReorganizationTypes.None);
					break;
				case ControlActionType.DataReorganizationType_HitchToParent:
					ChangeDataReorganizationType(DataReorganizationTypes.LinkToParent);
					break;
				case ControlActionType.DataReorganizationType_MergeNeighbors:
					ChangeDataReorganizationType(DataReorganizationTypes.MergeNeighbors);
					break;
			}
		}

		#region DrillThrough
		void DrillThroughCell(CellInfo cell)
		{
			if (DataManager != null)
			{
				String query = DataManager.BuildDrillThrough(cell);
				if (!String.IsNullOrEmpty(query))
				{
					MdxQueryArgs query_args = CommandHelper.CreateMdxQueryArgs(Connection, query);
					query_args.Type = QueryTypes.DrillThrough;
					LogManager.LogInformation(this, this.Name + " - DrillThrough cell");
					IsWaiting = true;
					OlapDataLoader.LoadData(query_args, new UserSchemaWrapper<String, CellInfo>("DRILLTHROUGH_CELL", cell));
				}
			}
		}

		#endregion
		#region Copy-Paste to Clipboard
		void PasteCellsFromClipboard(CellInfo cell)
		{
			// На всякий случай проверим
			if (!PivotGrid.CanEdit || !PivotGrid.EditMode)
				return;

			String text = Wing.Olap.Features.Clipboard.GetClipboardText();
			if (!String.IsNullOrEmpty(text))
			{
				// В буфере ячейки хранятся разделенные табуляцией и переходом на новую строку. 
				//  Например ячейки (* - пустая):
				//  3   *   *   *
				//  *   *   *   *
				//  5   *   6   *
				//  *   *   *   *
				// В буфере при копировании из Excel это выглядит так: "3\t\t\t\r\n\r\n5\t\t6"

				// Буфер, в который накапливаются значения из Clipboard
				List<List<String>> buff = new List<List<string>>();

				// Разделим содержимое буфера на строки
				text = text.Trim();
				String[] rows_list = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
				if (rows_list != null)
				{
					// Разделим содержимое каждой строки
					foreach (String str in rows_list)
					{
						List<String> row_Buff = new List<string>();
						String[] columns_list = str.Split(new string[] { "\t" }, StringSplitOptions.None);
						if (columns_list != null)
						{
							foreach (String val in columns_list)
							{
								row_Buff.Add(val);
							}
						}

						buff.Add(row_Buff);
					}
				}
				// Буфер с целью оптимизации не содержит пустые ячейки, если они вляются последними в строке.
				// Поэтому чтобы найти реальный размер копируемой области по ширине нужно взять максимальную длину из имеющихся
				int width = 0;
				foreach (List<String> row in buff)
				{
					width = Math.Max(width, row.Count);
				}

				// Обходим буфер и меняем значения ячеек
				int row_index = 0;
				List<UpdateEntry> changes = new List<UpdateEntry>();
				int start_ColumnIndex = 0;
				int start_RowIndex = 0;
				if (cell != null)
				{
					start_ColumnIndex = cell.CellDescr.Axis0_Coord;
					start_RowIndex = cell.CellDescr.Axis1_Coord;
				}
				foreach (List<String> row in buff)
				{
					for (int col_index = 0; col_index < width; col_index++)
					{
						String val = String.Empty;
						if (col_index < row.Count)
							val = row[col_index];

						// Пытаемся по координате получить ячейку
						CellInfo destination_cell = m_CellSetProvider.GetCellInfo(start_ColumnIndex + col_index, start_RowIndex + row_index);
						//FAST CellControl destination_cell = PivotPanelControl.CellsControl.GetCell(start_ColumnIndex + col_index, start_RowIndex + row_index);
						if (destination_cell != null && destination_cell.IsUpdateable)
						{
							if (String.IsNullOrEmpty(val))
							{
								// КC:
								// записывать НОЛЬ только в том случае если у нас было какое-то число, если числа не было (в ячейке было null) то оставлять null
								if (destination_cell.Value != null || PivotGrid.LocalChanges.FindChange(destination_cell) != null)
								{
									UpdateEntry entry = new UpdateEntry(destination_cell, "0");
									PivotGrid.ChangeCell(destination_cell, entry);
									changes.Add(entry);
									continue;
								}
							}
							else
							{
								try
								{
									// Пытаемся преобразовать строку в число
									double new_val = 0;
									new_val = Convert.ToDouble(val);

									if (destination_cell.Value != null)
									{
										double x = 0;
										x = Convert.ToDouble(destination_cell.Value.ToString());
										if (x == new_val)
											continue;
									}

									UpdateEntry entry = new UpdateEntry(destination_cell, new_val.ToString());
									PivotGrid.ChangeCell(destination_cell, entry);
									changes.Add(entry);
								}
								catch { }
							}
						}
					}
					row_index++;
				}

				if (changes.Count > 0)
					PerformCellChanges(changes);
			}
		}

		void CopyCellsToClipboard(CellInfo current)
		{
			IList<CellInfo> cells = PivotGrid.Selection;
			if (cells != null && cells.Count > 0)
			{
				// Ячейки для опреации копирования должны быть в прямоугольной области. Причем область должна быть без пустот
				// Для начала определим максимальные и минимальные индексы используемых строк и столбцов
				int min_row = -1;
				int max_row = -1;
				int min_col = -1;
				int max_col = -1;

				foreach (CellInfo cell in cells)
				{
					// Индексы для строки
					if (min_row == -1)
					{
						min_row = cell.CellDescr.Axis1_Coord;
					}
					else
					{
						min_row = Math.Min(min_row, cell.CellDescr.Axis1_Coord);
					}

					if (max_row == -1)
					{
						max_row = cell.CellDescr.Axis1_Coord;
					}
					else
					{
						max_row = Math.Max(max_row, cell.CellDescr.Axis1_Coord);
					}

					// Индексы для колонки
					if (min_col == -1)
					{
						min_col = cell.CellDescr.Axis0_Coord;
					}
					else
					{
						min_col = Math.Min(min_col, cell.CellDescr.Axis0_Coord);
					}
					if (max_col == -1)
					{
						max_col = cell.CellDescr.Axis0_Coord;
					}
					else
					{
						max_col = Math.Max(max_col, cell.CellDescr.Axis0_Coord);
					}
				}

				if (min_col > -1 && max_col > -1 && max_col >= min_col &&
						max_row >= min_row &&
						min_row >= -1 && max_row >= -1) // min_row и max_row могут быть равны -1 для случая когда в запросе только одна ось
				{
					// Теперь определяем массив (двумерный) с размерностью, которую мы определили
					// И пытаемся заполнить массив тем что есть. Если потом останутся в нем дырки (null) то область получается незамкнутая
					// Учитываем запросы с одной осью. Кол-во строк и столбцов не может быть меньше 1.
					int col_count = Math.Max(max_col - min_col + 1, 1);
					int row_count = Math.Max(max_row - min_row + 1, 1);

					List<List<String>> list = new List<List<string>>();
					for (int row_index = 0; row_index < row_count; row_index++)
					{
						List<String> row = new List<string>();
						for (int col_index = 0; col_index < col_count; col_index++)
						{
							// Зануляем все значения
							row.Add("_NULL_");
						}
						list.Add(row);
					}

					// Разбрасываем значения ячеек на свои места
					foreach (CellInfo cell in cells)
					{
						// Учитываем запросы с одной осью. Индекс не может быть отрицательным
						int row_indx = Math.Max(cell.CellDescr.Axis1_Coord - min_row, 0);
						int col_indx = Math.Max(cell.CellDescr.Axis0_Coord - min_col, 0);
						if (row_indx >= 0 && row_indx < row_count &&
								col_indx >= 0 && col_indx < col_count)
						{
							UpdateEntry change = PivotGrid.LocalChanges.FindChange(cell);
							if (change != null)
							{
								list[row_indx][col_indx] = change.NewValue;
							}
							else
							{
								if (cell.Value != null)
									list[row_indx][col_indx] = cell.Value.ToString();
								else
									list[row_indx][col_indx] = null;
							}
						}
					}

					bool has_dyrki = false;
					// Определяем есть ли дырки
					foreach (List<String> rows_list in list)
					{
						foreach (String str in rows_list)
						{
							if (str == "_NULL_")
							{
								has_dyrki = true;
								break;
							}
						}
					}

					// Если есть дырки - значит копировать не судьба
					if (has_dyrki)
					{
						MessageBox.Show(Localization.PivotGrid_CellsCopyAreaError, Localization.MessageBox_Warning, MessageBoxButton.OK);
						return;
					}

					StringBuilder sb = new StringBuilder();
					// Формируем результат. Значения размеляются \t 
					// Строки разделяются \r\n
					// Определяем есть ли дырки
					foreach (List<String> rows_list in list)
					{
						int i = 0;
						foreach (String str in rows_list)
						{
							if (i != 0)
								sb.Append("\t");
							sb.Append(str);
							i++;
						}
						sb.Append(Environment.NewLine);
					}

					Wing.Olap.Features.Clipboard.SetClipboardText(sb.ToString());
				}
			}
		}
		#endregion Copy-Paste to Clipboard

		#region Копирование данных
		protected virtual void ShowValueCopyDialog(CellInfo cell)
		{
			if (m_CSDescr != null && cell != null && cell.IsUpdateable)
			{
				// Если в кэше есть изменения, то нужно спросить об их сохранении
				if (UseChangesCashe && PivotGrid.LocalChanges.Count > 0)
				{
					MessageBox.Show(Localization.PivotGrid_SaveCachedChanges, Localization.MessageBox_Warning, MessageBoxButton.OK);
					return;
				}

				IDictionary<String, MemberWrap> slice = new Dictionary<String, MemberWrap>();
				IDictionary<String, MemberInfo> tuple = cell.GetTuple();
				foreach (String hierarchyUniqueName in tuple.Keys)
				{
					slice.Add(hierarchyUniqueName, new MemberWrap(tuple[hierarchyUniqueName]));
				}

				ModalDialog dlg = new ModalDialog();
				dlg.MinHeight = 300;
				dlg.MinWidth = 400;
				dlg.Height = 500;
				dlg.Width = 600;
				dlg.Caption = Localization.ValueCopyDialog_Caption;
				dlg.DialogOk += new EventHandler<DialogResultArgs>(ValueCopyDialog_OkButtonClick);

				ValueCopyControl CopyControl = new ValueCopyControl();
				CopyControl.CubeName = m_CSDescr.CubeName;
				CopyControl.ConnectionID = m_CSDescr.Connection.ConnectionID;
				CopyControl.LoadMetaData += new EventHandler(ValueCopyControl_LoadMetaData);
				CopyControl.GetOlapDataLoader += new EventHandler<GetIDataLoaderArgs>(CopyControl_GetMetadataLoader);
				CopyControl.LogManager = this.LogManager;
				CopyControl.Initialize(slice, cell.Value);
				dlg.Content = CopyControl;

				ShowDialog(dlg);
			}
		}

		void CopyControl_GetMetadataLoader(object sender, GetIDataLoaderArgs e)
		{
			e.Loader = GetOlapDataLoader();
			e.Handled = true;
		}

		CubeDefInfo m_CubeMetaData = null;

		void ValueCopyControl_LoadMetaData(object sender, EventArgs e)
		{
			ValueCopyControl copyControl = sender as ValueCopyControl;
			if (copyControl != null)
			{
				if (m_CubeMetaData == null)
				{
					copyControl.IsBusy = true;
					MetadataQuery args = CommandHelper.CreateGetCubeMetadataArgs(Connection, m_CSDescr.CubeName, MetadataQueryType.GetCubeMetadata_AllMembers);
					LogManager.LogInformation(this, this.Name + " - Loading cube metadata.");
					OlapDataLoader.LoadData(args, sender);
				}
				else
				{
					copyControl.IsBusy = false;
					copyControl.InitializeMetadata(m_CubeMetaData);
				}
			}
		}

		void ValueCopyDialog_OkButtonClick(object sender, DialogResultArgs e)
		{
			ModalDialog dlg = sender as ModalDialog;
			if (dlg != null)
			{
				ValueCopyControl copyControl = dlg.Content as ValueCopyControl;
				if (copyControl != null)
				{
					String query = copyControl.BuildCopyScript();
					if (!String.IsNullOrEmpty(query))
					{
						MdxQueryArgs args = CommandHelper.CreateMdxQueryArgs(Connection, query);
						args.Type = QueryTypes.Update;
						IsWaiting = true;
						LogManager.LogInformation(this, this.Name + " - Copy values.");
						OlapDataLoader.LoadData(args, "ValueCopyDialog_OkButton");
					}
					else
					{
						e.Cancel = true;
					}
				}
			}
		}
		#endregion Копирование данных

		#region Разноска данных
		protected virtual void ShowValueDeliveryDialog(CellInfo cell)
		{
			if (m_CSDescr != null && cell != null && cell.IsUpdateable)
			{
				// Если в кэше есть изменения, то нужно спросить об их сохранении
				if (UseChangesCashe && PivotGrid.LocalChanges.Count > 0)
				{
					MessageBox.Show(Localization.PivotGrid_SaveCachedChanges, Localization.MessageBox_Warning, MessageBoxButton.OK);
					return;
				}

				ModalDialog dlg = new ModalDialog();
				dlg.MinHeight = 300;
				dlg.MinWidth = 400;
				dlg.Height = 500;
				dlg.Width = 600;
				dlg.Caption = Localization.ValueDeliveryDialog_Caption;
				dlg.DialogOk += new EventHandler<DialogResultArgs>(ValueDeliveryDialog_OkButtonClick);

				ValueDeliveryControl DeliveryControl = new ValueDeliveryControl();
				DeliveryControl.CubeName = m_CSDescr.CubeName;
				DeliveryControl.ConnectionID = m_CSDescr.Connection.ConnectionID;
				DeliveryControl.LoadMembers += new EventHandler<QueryEventArgs>(DeliveryControl_LoadMembers);
				DeliveryControl.Initialize(cell);
				dlg.Content = DeliveryControl;

				ShowDialog(dlg);
			}
		}

		void ValueDeliveryDialog_OkButtonClick(object sender, DialogResultArgs e)
		{
			ModalDialog dlg = sender as ModalDialog;
			if (dlg != null)
			{
				ValueDeliveryControl DeliveryControl = dlg.Content as ValueDeliveryControl;
				if (DeliveryControl != null && DeliveryControl.Cell != null)
				{
					if (DeliveryControl.IsDelivered)
					{
						double new_Val = DeliveryControl.OriginalValue - DeliveryControl.DeliveredValue;
						String value = new_Val.ToString();
						// В качестве разделителя для числа обязательно должна использоватьеся точка (т.к. эта строка будет помещена в МDX)
						value = value.Replace(System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator, ".");

						List<UpdateEntry> entries = new List<UpdateEntry>();
						UpdateEntry entry = new UpdateEntry(DeliveryControl.Cell, value);
						DeliveryControl.GetDeliveredMembers();
						entries.Add(entry);

						IList<MemberItem> changes = DeliveryControl.GetDeliveredMembers();
						foreach (MemberItem item in changes)
						{
							value = item.NewValue.ToString();
							// В качестве разделителя для числа обязательно должна использоватьеся точка (т.к. эта строка будет помещена в МDX)
							value = value.Replace(System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator, ".");

							UpdateEntry item_entry = new UpdateEntry();
							foreach (var tuple_item in DeliveryControl.Cell.Tuple)
							{
								String uniqueName = String.Empty;
								if (tuple_item.Key != item.Member.HierarchyUniqueName)
								{
									uniqueName = tuple_item.Value;
								}
								else
								{
									uniqueName = item.Member.UniqueName;
								}
								item_entry.Tuple.Add(tuple_item.Key, uniqueName);
							}
							item_entry.NewValue = value;
							try
							{
								item_entry.OldValue = item.Cell.CellDescr.Value.Value.ToString();
							}
							catch
							{
							}

							entries.Add(item_entry);
						}
						SaveChanges(entries);
					}
				}
			}
		}

		void DeliveryControl_LoadMembers(object sender, QueryEventArgs e)
		{
			if (e != null && !String.IsNullOrEmpty(e.Query))
			{
				MdxQueryArgs args = CommandHelper.CreateMdxQueryArgs(Connection, e.Query);
				LogManager.LogInformation(this, this.Name + " - Delivery value.");
				OlapDataLoader.LoadData(args, sender);
			}
		}
		#endregion Разноска данных

		void ShowDataSourceInfo(DataSourceInfoArgs args)
		{
			ModalDialog dlg = new ModalDialog();
			dlg.Caption = Localization.DataSourceInfoDialog_Caption;
			dlg.MinHeight = 300;
			dlg.MinWidth = 400;
			dlg.Height = 400;
			dlg.Width = 500;

			DataSourceInfoControl DSInfo = new DataSourceInfoControl();
			dlg.Content = DSInfo;
			DSInfo.UpdateScriptVisibility = PivotGrid.CanEdit;
			DSInfo.Initialize(args);

			ShowDialog(dlg);
		}

		/// <summary>
		/// В случае необходимости выводит диалог с запросом на сохранение изменений
		/// </summary>
		//private PopUpQuestionDialog SaveChangesDlg
		//{
		//    get
		//    {
		//        PopUpQuestionDialog dlg = new PopUpQuestionDialog();
		//        dlg.Caption = "Warning...";
		//        dlg.ContentCtrl.Text = "Cache contains unsaved data! Save cached changes?";
		//        dlg.ContentCtrl.DialogType = DialogButtons.YesNo;
		//        return dlg;
		//    }
		//}


		/// <summary>
		/// Получает информацию о источнике данных
		/// </summary>
		/// <param name="userData"></param>
		public virtual DataSourceInfoArgs GetDataSourceInfo(UpdateEntry userData)
		{
			if (DataManager != null)
			{
				DataSourceInfoArgs info = DataManager.GetDataSourceInfo(userData);
				info.ConnectionString = Connection;
				return info;
			}
			return null;
		}

		void dlg_CloseDialog(DialogResult e)
		{
			if (e == DialogResult.Yes)
			{
				SaveChanges(PivotGrid.LocalChanges.GetCellChanges());
				return;
			}
			if (e == DialogResult.No)
			{
				CancelChanges();
				return;
			}
		}

		void SaveChanges(List<UpdateEntry> entries)
		{
			if (entries != null && entries.Count > 0)
			{
				RunUpdateCubeCommand(entries);
			}
			UpdateEditToolBarButtons();
		}

		void CancelChanges()
		{
			if (PivotGrid.LocalChanges.Count > 0)
			{
				PivotGrid.LocalChanges.Clear();
				UpdateEditToolBarButtons();

				RunServiceCommand(ServiceCommandType.Refresh);
			}
		}

		void CancelEditButton_Click(object sender, RoutedEventArgs e)
		{
			if (UseModifyDataConfirm)
			{
				if (MessageBox.Show(Localization.PivotGrid_CancelEditButton_Question, Localization.Confirmation, MessageBoxButton.OKCancel) != MessageBoxResult.OK)
					return;
			}
			CancelChanges();
		}

		void ConfirmEditButton_Click(object sender, RoutedEventArgs e)
		{
			if (UseModifyDataConfirm)
			{
				if (MessageBox.Show(Localization.PivotGrid_ConfirmEditButton_Question, Localization.Confirmation, MessageBoxButton.OKCancel) != MessageBoxResult.OK)
					return;
			}
			ExportSizeInfo();
			SaveChanges(PivotGrid.LocalChanges.GetCellChanges());
		}

		void UseChangesCasheButton_Checked(object sender, RoutedEventArgs e)
		{
			// Если в кэше есть изменения, то нужно спросить об их сохранении
			if (UseChangesCashe && PivotGrid.LocalChanges.Count > 0)
			{
				MessageBox.Show(Localization.PivotGrid_SaveCachedChanges, Localization.MessageBox_Warning, MessageBoxButton.OK);
				return;
			}

			if (UseChangesCashe != UseChangesCasheButton.IsChecked.Value)
			{
				UseChangesCashe = UseChangesCasheButton.IsChecked.Value;
			}
		}

		//void UseChangesCasheButton_SaveChanges_DialogClosed(object sender, Wing.Olap.Controls.Forms.DialogResultArgs e)
		//{
		//    dlg_CloseDialog(e.Result);
		//    if (UseChangesCashe != UseChangesCasheButton.IsChecked.Value)
		//    {
		//        UseChangesCashe = UseChangesCasheButton.IsChecked.Value;
		//    }
		//}

		void EditButton_Click(object sender, RoutedEventArgs e)
		{
			// Если в кэше есть изменения, то нужно спросить об их сохранении
			if (UseChangesCashe && PivotGrid.LocalChanges.Count > 0)
			{
				MessageBox.Show(Localization.PivotGrid_SaveCachedChanges, Localization.MessageBox_Warning, MessageBoxButton.OK);
				return;
			}

			if (EditMode != EditButton.IsChecked.Value)
			{
				PivotGrid.EditMode = EditButton.IsChecked.Value;
			}
			UpdateEditToolBarButtons(true);
		}

		//void EditButton_SaveChanges_DialogClosed(object sender, Wing.Olap.Controls.Forms.DialogResultArgs e)
		//{
		//    dlg_CloseDialog(e.Result);
		//    if (EditMode != EditButton.IsChecked.Value)
		//    {
		//        ScrollableDataControl.PivotGrid.CellsControl.EditMode = EditButton.IsChecked.Value;
		//    }
		//    UpdateEditToolBarButtons();
		//}

		void HideEmptyColumnsButton_Click(object sender, RoutedEventArgs e)
		{
			if (HideEmptyColumnsButton.IsChecked.Value)
				RunServiceCommand(ServiceCommandType.HideEmptyColumns);
			else
				RunServiceCommand(ServiceCommandType.ShowEmptyColumns);
		}

		void HideEmptyRowsButton_Click(object sender, RoutedEventArgs e)
		{
			if (HideEmptyRowsButton.IsChecked.Value)
				RunServiceCommand(ServiceCommandType.HideEmptyRows);
			else
				RunServiceCommand(ServiceCommandType.ShowEmptyRows);
		}

		protected virtual void RowsControl_ContextMenuCreated(object sender, EventArgs e)
		{
		}

		protected virtual void ColumnsControl_ContextMenuCreated(object sender, EventArgs e)
		{
		}

		protected virtual void CellsControl_ContextMenuCreated(object sender, EventArgs e)
		{
		}

		void CellsControl_CellValueChanged(object sender, CellValueChangedEventArgs e)
		{
			PerformCellChanges(e.Changes);
		}

		void PerformCellChanges(List<UpdateEntry> changes_list)
		{
			if (changes_list != null)
			{
				foreach (var e in changes_list)
				{
					PivotGrid.LocalChanges.Add(e);
				}

				if (!UseChangesCashe)
				{
					SaveChanges(changes_list);
					return;
				}

				UpdateEditToolBarButtons();
			}
		}

		void RunUpdateCubeCommand(IEnumerable<UpdateEntry> entries)
		{
			if (String.IsNullOrEmpty(UpdateScript))
			{
				LogManager.LogError(this, String.Format(Localization.ControlSettingsNotInitialized_Message, Localization.UpdateScript_PropertyDesc));
				return;
			}

			String cubeName = String.Empty;
			String connectionString = String.Empty;
			if (m_CSDescr != null)
			{
				cubeName = m_CSDescr.CubeName;
				connectionString = m_CSDescr.Connection.ConnectionString;
			}

			IsWaiting = true;

			MdxQueryArgs args = new MdxQueryArgs();
			args.Connection = Connection;
			args.Type = QueryTypes.Update;
			args.Queries = new List<string>(DataManager.BuildUpdateScripts(cubeName, entries));
			LogManager.LogInformation(this, this.Name + " - Update cube started.");
			OlapDataLoader.LoadData(args, entries);

			//UpdateCubeArgs args = CommandHelper.CreateUpdateCubeArgs(connectionString, cubeName, entries);
			//args.Script = DataManager != null ? DataManager.UpdateScript : String.Empty;
			//IsWaiting = true;
			//OlapDataLoader.LoadData(args, args);
		}

		protected void UpdateButtons()
		{
			UpdateToolbarButtons(DataManager != null ? DataManager.GetToolBarInfo() : null);
		}

		void UpdateToolbarButtons(PivotGridToolBarInfo info)
		{
			if (info == null)
			{
				BackButton.IsEnabled = false;
				ForwardButton.IsEnabled = false;
				ToBeginButton.IsEnabled = false;
				ToEndButton.IsEnabled = false;


				HideEmptyColumnsButton.IsChecked = false;
				HideEmptyRowsButton.IsChecked = false;
			}
			else
			{
				HideEmptyColumnsButton.IsChecked = info.HideEmptyColumns;
				HideEmptyRowsButton.IsChecked = info.HideEmptyRows;
				RotateAxesButton.IsChecked = info.RotateAxes;

				if (info.HistorySize > 0)
				{
					ToBeginButton.IsEnabled = BackButton.IsEnabled = info.CurrentHistoryIndex > 0;
					ToEndButton.IsEnabled = ForwardButton.IsEnabled = info.CurrentHistoryIndex + 1 < info.HistorySize;
				}
				else
				{
					BackButton.IsEnabled = false;
					ForwardButton.IsEnabled = false;
					ToBeginButton.IsEnabled = false;
					ToEndButton.IsEnabled = false;
				}
			}
		}

		protected virtual void PerformMemberAction(MemberActionEventArgs e)
		{
			if (e.Action == MemberActionType.Collapse ||
					e.Action == MemberActionType.Expand ||
					e.Action == MemberActionType.DrillDown
					|| e.Action == MemberActionType.SortByValue
				//                 || e.Action == MemberActionType.DrillUp
					)
			{
				List<MemberInfo> full_tuple = e.Member.GetAncestors(true);

				PerformMemberActionArgs args = CommandHelper.CreatePerformMemberActionArgs(
						e.Member, e.Axis, e.Action, full_tuple);

				if (DataManager != null)
				{
					String query = DataManager.PerformMemberAction(args);
					// query=generateQuery(e);
					if (!String.IsNullOrEmpty(query))
					{
						MdxQueryArgs query_args = CommandHelper.CreateMdxQueryArgs(Connection, query);
						ExecuteMemberAction(query_args, args);
					}
				}
			}
		}

		protected virtual void ExecuteMemberAction(MdxQueryArgs query_args, PerformMemberActionArgs args)
		{
			if (args != null && query_args != null)
			{
				IsWaiting = true;

				LogManager.LogInformation(this, this.Name + String.Format(" - {0} member {1}", args.Action.ToString(), args.Member != null ? args.Member.UniqueName : "<null>"));
				OlapDataLoader.LoadData(query_args, args);
			}
		}

		public PivotQueryManager DataManager
		{
			get { return PivotGrid.QueryManager; }
			set { PivotGrid.QueryManager = value; }
		}

		public DrillDownMode m_DrillDownMode = DrillDownMode.BySingleDimensionHideSelf;
		public DrillDownMode DrillDownMode
		{
			get { return m_DrillDownMode; }
			set { m_DrillDownMode = value; }
		}

		IDataLoader m_OlapDataLoader = null;
		public IDataLoader OlapDataLoader
		{
			set
			{
				if (m_OlapDataLoader != null)
				{
					m_OlapDataLoader.DataLoaded -= new EventHandler<DataLoaderEventArgs>(OlapDataLoader_DataLoaded);
				}

				m_OlapDataLoader = value;
				if (m_OlapDataLoader != null)
				{
					m_OlapDataLoader.DataLoaded += new EventHandler<DataLoaderEventArgs>(OlapDataLoader_DataLoaded);
				}
			}
			get
			{
				return m_OlapDataLoader;
			}
		}

		IStorageManager m_StorageManager = null;
		public IStorageManager StorageManager
		{
			get
			{
				return m_StorageManager;
			}
		}

		protected virtual IStorageManager GetStorageManager()
		{
			return new StorageManager(URL);
		}

		void OlapDataLoader_DataLoaded(object sender, DataLoaderEventArgs e)
		{
			bool stopWaiting = true;

			try
			{
				// Exception
				if (e.Error != null)
				{
					LogManager.LogError(this, e.Error.ToString());
					return;
				}

				// Exception or Message from Olap-Service
				if (e.Result.ContentType == InvokeContentType.Error)
				{
					LogManager.LogError(this, e.Result.Content);
					return;
				}

				PivotGrid.Focus();

				if (e.UserState != null && e.UserState.ToString() == "ValueCopyDialog_OkButton")
				//ValueCopyControl copy = e.UserState as ValueCopyControl;
				//if (copy != null)
				{
					stopWaiting = false;
					RunServiceCommand(ServiceCommandType.Refresh);
					return;
				}


				ValueDeliveryControl dilivery = e.UserState as ValueDeliveryControl;
				if (dilivery != null)
				{
					if (!string.IsNullOrEmpty(e.Result.Content))
					{
						CellSetData cs_descr = CellSetData.Deserialize(e.Result.Content);
						dilivery.InitializeMembersList(new CellSetDataProvider(cs_descr));
					}
					return;
				}

				PivotInitializeArgs args = e.UserState as PivotInitializeArgs;
				if (args != null)
				{
					if (!string.IsNullOrEmpty(e.Result.Content))
					{
						CellSetData cs_descr = CellSetData.Deserialize(e.Result.Content);
						Initialize(cs_descr);
					}
					else
					{
						Initialize(null);
					}
					return;
				}

				PerformMemberActionArgs action_args = e.UserState as PerformMemberActionArgs;
				if (action_args != null)
				{
					MemberActionCompleted(e);
					return;
				}

				if (e.UserState != null && e.UserState is ServiceCommandType)
				{
					ServiceCommandType actionType = (ServiceCommandType)(ServiceCommandType.Parse(typeof(ServiceCommandType), e.UserState.ToString(), true));
					// Save to File
					if (actionType == ServiceCommandType.ExportToExcel)
					{
						if (ExportToExcelFile)
						{
							SaveToFile(e.Result.Content);
							return;
						}
					}
					else
					{
						if (!string.IsNullOrEmpty(e.Result.Content))
						{
							CellSetData cs_descr = CellSetData.Deserialize(e.Result.Content);
							Initialize(cs_descr);
						}
					}

					UpdateButtons();
					ServiceCommandCompleted(actionType);
					return;
				}

				//PivotGridToolBarInfo toolbar_args = e.UserState as PivotGridToolBarInfo;
				//if (toolbar_args != null)
				//{
				//    if (!string.IsNullOrEmpty(e.Result.Content))
				//    {
				//        toolbar_args = XmlSerializationUtility.XmlStr2Obj<PivotGridToolBarInfo>(e.Result.Content);
				//        UpdateToolbarButtons(toolbar_args);
				//    }
				//    //this.Focus();
				//    return;
				//}

				//UpdateCubeArgs update_Args = e.UserState as UpdateCubeArgs;
				//if (update_Args != null)
				List<UpdateEntry> entries = e.UserState as List<UpdateEntry>;
				if (entries != null)
				{
					// Результат - коллекция строк. Если на какой-то ячейке произошла ошибка, то в соотв. строке будет ее текст
					List<String> results = XmlSerializationUtility.XmlStr2Obj<List<String>>(e.Result.Content);
					if (results != null && results.Count == entries.Count)
					{
						StringBuilder sb = new StringBuilder();
						var successful = new List<UpdateEntry>();
						for (int i = 0; i < results.Count; i++)
						{
							if (String.IsNullOrEmpty(results[i]))
							{
								// No error
								entries[i].Error = String.Empty;
								// Update is successful. 
								successful.Add(entries[i]);
								// Update is successful. Remove this change from local cache
								PivotGrid.LocalChanges.RemoveChange(entries[i]);
							}
							else
							{
								// Error message
								entries[i].Error = results[i];
								sb.AppendLine(results[i]);
								PivotGrid.LocalChanges.Add(entries[i]);
							}
						}
						// Add cell changes to Transaction cache
						if (successful.Count > 0)
						{
							OlapTransactionManager.AddPendingChanges(Connection, successful);
						}

						if (!String.IsNullOrEmpty(sb.ToString()))
						{
							LogManager.LogError(this, sb.ToString());
						}
					}

					UpdateEditToolBarButtons();
					UpdateCubeCompleted();
					//this.Focus();
					return;
				}

				ValueCopyControl copyControl = e.UserState as ValueCopyControl;
				if (copyControl != null)
				{
					CubeDefInfo cs_descr = XmlSerializationUtility.XmlStr2Obj<CubeDefInfo>(e.Result.Content);
					m_CubeMetaData = cs_descr;
					copyControl.IsBusy = false;
					copyControl.InitializeMetadata(cs_descr);
				}

				MemberInfoWrapper<MetadataQuery> metadata_args = e.UserState as MemberInfoWrapper<MetadataQuery>;
				if (metadata_args != null)
				{
					switch (metadata_args.UserData.QueryType)
					{
						case MetadataQueryType.GetLevelProperties:
							List<LevelPropertyInfo> properties = XmlSerializationUtility.XmlStr2Obj<List<LevelPropertyInfo>>(e.Result.Content);
							m_LevelProperties[metadata_args.Member.HierarchyUniqueName] = properties;
							LoadMemberAttributes(metadata_args.Member, properties);
							break;
					}
				}

				UserSchemaWrapper<String, CellInfo> user_wrapper = e.UserState as UserSchemaWrapper<String, CellInfo>;
				if (user_wrapper != null && user_wrapper.Schema == "DRILLTHROUGH_CELL")
				{
					if (!String.IsNullOrEmpty(e.Result.Content))
					{
						DataTableWrapper tableWrapper = XmlSerializationUtility.XmlStr2Obj<DataTableWrapper>(e.Result.Content);
						ShowDrillthroughResult(user_wrapper.UserData, tableWrapper);
					}
				}

				SortPropertiesControl sortControl = e.UserState as SortPropertiesControl;
				if (sortControl != null)
				{
					List<MeasureInfo> list = XmlSerializationUtility.XmlStr2Obj<List<MeasureInfo>>(e.Result.Content);
					sortControl.InitializeMeasuresList(list);
				}

				//MemberInfoWrapper<MemberChoiceQuery> member_args = e.UserState as MemberInfoWrapper<MemberChoiceQuery>;
				//if (member_args != null)
				if (e.UserState != null && e.UserState.ToString() == "CUSTOM_MEMBER_PROPERTIES")
				{
					//switch (member_args.UserData.QueryType)
					{
						//case MemberChoiceQueryType.GetMember:

						MemberData member = null;
						if (!String.IsNullOrEmpty(e.Result.Content))
						{
							CellSetData cellSet = CellSetData.Deserialize(e.Result.Content);
							if (cellSet != null && cellSet.Axes.Count > 0 && cellSet.Axes[0].Members.Count > 0)
								member = cellSet.Axes[0].Members[0];
						}

						if (member != null)
						{
							ShowMemberAttributes(member);
						}
						else
						{
							MessageBox.Show(Localization.PivotGrid_CustomProperties_NotFound, Localization.Warning, MessageBoxButton.OK);
						}
						//break;
					}
				}
			}
			finally
			{
				if (stopWaiting)
					IsWaiting = false;
			}
		}

		private SaveFileDialog m_SaveFileDialog = null;
		private SaveFileDialog SaveFileDialog
		{
			get
			{
				if (m_SaveFileDialog == null)
				{
					m_SaveFileDialog = new SaveFileDialog();
					m_SaveFileDialog.Filter = Localization.SaveDialog_Filter_XlsFiles;
				}
				return m_SaveFileDialog;
			}
		}


		void SaveToFile(String str)
		{
			try
			{
				//bool? ret = SaveFileDialog.ShowDialog();
				//if (ret.HasValue && ret.Value == true)
				//{
				using (Stream stream = SaveFileDialog.OpenFile())
				{
					byte[] info = (new UTF8Encoding(true)).GetBytes(str);
					stream.Write(info, 0, info.Length);
				}
				//}
			}
			catch (Exception ex)
			{
				LogManager.LogError(this, ex.ToString());
			}
		}

		protected virtual void ServiceCommandCompleted(ServiceCommandType commandType)
		{

		}

		protected virtual void UpdateCubeCompleted()
		{
			RunServiceCommand(ServiceCommandType.Refresh);
		}

		protected virtual void MemberActionCompleted(DataLoaderEventArgs e)
		{
			if (!String.IsNullOrEmpty(e.Result.Content))
			{
				//CellSetData cs_descr = XmlSerializationUtility.XmlStr2Obj<CellSetData>(e.Result);
				CellSetData cs_descr = CellSetData.Deserialize(e.Result.Content);
				Initialize(cs_descr);
			}
			//this.Focus();
			UpdateButtons();
		}

		void ResetSettings()
		{
			m_CSDescr = null;
			m_CubeMetaData = null;
			m_CellSetProvider = null;
			UpdateToolbarButtons(null);
			UpdateEditToolBarButtons();
		}

		public void Initialize()
		{
			// Отжимаем кнопку "Повернуть оси"
			RotateAxesButton.Click -= new RoutedEventHandler(RotateAxesButton_Click);
			RotateAxesButton.IsChecked = new bool?(false);
			PivotGrid.AxisIsRotated = false;
			RotateAxesButton.Click += new RoutedEventHandler(RotateAxesButton_Click);

			// Устанавливаем значение кнопки Редактирование в соответствии с возможностью редактирования
			EditButton.IsChecked = IsUpdateable;

			ResetSettings();

			DataManager = GetDataManager();
			DataManager.DrillDownMode = this.DrillDownMode;

			// Clear Pivot Grid
			Initialize(null);

			// Если запрос пустой, то данные с сервера не читаем
			if (!String.IsNullOrEmpty(Query))
			{
				PivotInitializeArgs args = CommandHelper.CreatePivotInitializeArgs(Connection, Query, UpdateScript);
				if (DataManager != null)
				{
					IsWaiting = true;
					LogManager.LogInformation(this, this.Name + " - Initialization started.");
					MdxQueryArgs query_args = CommandHelper.CreateMdxQueryArgs(Connection, DataManager.RefreshQuery());
					// qContext = new Wing.Olap.Mdx.MdxQueryContext(Query);
					OlapDataLoader.LoadData(query_args, args);
				}

			}
		}

		private CellSetData m_CSDescr = null;
		CellSetDataProvider m_CellSetProvider = null;
		protected CellSetDataProvider CellSetProvider
		{
			get { return m_CellSetProvider; }
		}

		protected virtual void Initialize(CellSetData cs_descr)
		{
			try
			{
				Dictionary<String, SortDescriptor> axis1_sortInfo = null;
				Dictionary<String, SortDescriptor> axis0_sortInfo = null;
				if (m_CellSetProvider != null)
				{
					axis0_sortInfo = m_CellSetProvider.ColumnsSortInfo;
					axis1_sortInfo = m_CellSetProvider.RowsSortInfo;
				}

				ResetSettings();

				IsWaiting = true;
				DateTime start = DateTime.Now;
				//System.Diagnostics.Debug.WriteLine("UpdateablePivotGrid initializing start: " + start.ToString());

				m_CSDescr = cs_descr;
				if (cs_descr != null)
				{
					m_CellSetProvider = new CellSetDataProvider(cs_descr, DataReorganizationType);
					if (axis0_sortInfo != null)
					{
						foreach (var hierarchyUniqueName in axis0_sortInfo.Keys)
						{
							m_CellSetProvider.Sort(0, hierarchyUniqueName, axis0_sortInfo[hierarchyUniqueName]);
						}
					}

					if (axis1_sortInfo != null)
					{
						foreach (var hierarchyUniqueName in axis1_sortInfo.Keys)
						{
							m_CellSetProvider.Sort(1, hierarchyUniqueName, axis1_sortInfo[hierarchyUniqueName]);
						}
					}
					ImportSizeInfo();
				}

				PivotGrid.Initialize(m_CellSetProvider);
				UpdateButtons();

				DateTime stop = DateTime.Now;
				//System.Diagnostics.Debug.WriteLine("UpdateablePivotGrid initializing stop: " + stop.ToString());
				System.Diagnostics.Debug.WriteLine("UpdateablePivotGrid initializing time: " + (stop - start).ToString());
			}
			finally
			{
				IsWaiting = false;
			}
		}

		public void Refresh()
		{
			RunServiceCommand(ServiceCommandType.Refresh);
		}

		private void RefreshButton_Click(object sender, RoutedEventArgs e)
		{
			Refresh();
		}

		private void BackButton_Click(object sender, RoutedEventArgs e)
		{
			RunServiceCommand(ServiceCommandType.Back);
		}

		private void RunServiceCommand(ServiceCommandType actionType)
		{
			if (actionType == ServiceCommandType.Refresh)
				ExportSizeInfo();

			if (DataManager != null)
			{
				switch (actionType)
				{
					case ServiceCommandType.GetDataSourceInfo:
						break;
					default:
						String query = DataManager.PerformServiceCommand(actionType);
						PivotGrid.AxisIsRotated = DataManager.CurrentHistoryItem.RotateAxes;
						if (RotateAxesButton.IsChecked != PivotGrid.AxisIsRotated)
						{
							RotateAxesButton.IsChecked = PivotGrid.AxisIsRotated;
						}
						if (!String.IsNullOrEmpty(query))
						{
							MdxQueryArgs query_args = CommandHelper.CreateMdxQueryArgs(Connection, query);
							// Export to Excel execute on server
							if (actionType == ServiceCommandType.ExportToExcel)
							{
								query_args.ActionType = OlapActionTypes.ExportToExcel;
							}

							if (actionType == ServiceCommandType.RotateAxes || actionType == ServiceCommandType.NormalAxes)
							{
								if (m_CellSetProvider != null)
									m_CellSetProvider.RotateSortInfo();
							}
							ExecuteServiceCommand(query_args, actionType);
						}
						break;
				}
			}
			//OlapDataLoader.LoadData(args, args);
		}

		protected virtual void ExecuteServiceCommand(MdxQueryArgs query_args, ServiceCommandType actionType)
		{
			if (query_args != null)
			{
				IsWaiting = true;
				LogManager.LogInformation(this, this.Name + " - Service command: " + actionType.ToString());
				OlapDataLoader.LoadData(query_args, actionType);
			}
		}

		public CellControl FocusedCell
		{
			get
			{
				return PivotGrid.FocusedCell;
			}
		}

		//void RunService_SaveChanges_DialogClosed(object sender, Wing.Olap.Controls.Forms.DialogResultArgs e)
		//{
		//    dlg_CloseDialog(e.Result);

		//    PopUpQuestionDialog dlg = sender as PopUpQuestionDialog;
		//    ServiceCommandType actionType = ServiceCommandType.None;
		//    if (dlg != null && dlg.Tag is ServiceCommandType)
		//    {
		//        actionType = (ServiceCommandType)(dlg.Tag);
		//    }

		//    ServiceCommandArgs args = new ServiceCommandArgs(actionType);
		//    Loader.PerformServiceCommand(XmlSerializationUtility.Obj2XmlStr(args, Constants.XmlNamespace), args);
		//}

		#region Настройки шрифтов, размеров по умолчанию
		public double DefaultFontSize
		{
			get { return PivotGrid.DefaultFontSize; }
			set { PivotGrid.DefaultFontSize = value; }
		}

		public double DefaultMemberWidth
		{
			get { return PivotGrid.DEFAULT_WIDTH; }
			set { PivotGrid.DEFAULT_WIDTH = value; }
		}

		public double DefaultMemberHeight
		{
			get { return PivotGrid.DEFAULT_HEIGHT; }
			set { PivotGrid.DEFAULT_HEIGHT = value; }
		}

		public double MinMemberWidth
		{
			get { return PivotGrid.MIN_WIDTH; }
			set { PivotGrid.MIN_WIDTH = value; }
		}

		public double MinMemberHeight
		{
			get { return PivotGrid.MIN_HEIGHT; }
			set { PivotGrid.MIN_HEIGHT = value; }
		}

		public double RowsDrillDownMemberOffset
		{
			get { return PivotGrid.DRILLDOWN_SPACE_WIDTH; }
			set { PivotGrid.DRILLDOWN_SPACE_WIDTH = value; }
		}
		#endregion Настройки шрифтов, размеров по умолчанию

		#region Управление возможностью редактирования ячеек
		public bool IsUpdateable
		{
			get
			{
				return PivotGrid.IsUpdateable;
			}
			set
			{
				PivotGrid.IsUpdateable = value;
				// Устанавливаем значение кнопки Редактирование в соответствии с возможностью редактирования
				EditButton.IsChecked = IsUpdateable;
				UpdateEditToolBarButtons();
			}
		}

		bool m_UseChangesCashe = false;
		public bool UseChangesCashe
		{
			get
			{
				return m_UseChangesCashe;
			}
			set
			{
				m_UseChangesCashe = value;
				UseChangesCasheButton.IsChecked = new bool?(value);
				UpdateEditToolBarButtons();
			}
		}

		public bool EditMode
		{
			get
			{
				return PivotGrid.EditMode;
			}
		}

		void UpdateEditToolBarButtons()
		{
			UpdateEditToolBarButtons(false);
		}

		void UpdateEditToolBarButtons(bool skipEditModeButton)
		{
			// Кнопки управления редактированием делаем видимыми только для таблицы, которая поддерживает редактирование
			EditButton.Visibility = PivotGrid.CanEdit ? Visibility.Visible : Visibility.Collapsed;
			//CopyToClipboardButton;
			PasteFromClipboardButton.Visibility = PivotGrid.CanEdit ? Visibility.Visible : Visibility.Collapsed;
			UseChangesCasheButton.Visibility = PivotGrid.CanEdit ? Visibility.Visible : Visibility.Collapsed;
			ConfirmEditButton.Visibility = PivotGrid.CanEdit ? Visibility.Visible : Visibility.Collapsed;
			CancelEditButton.Visibility = PivotGrid.CanEdit ? Visibility.Visible : Visibility.Collapsed;

			if (PivotGrid.CanEdit)
			{
				if (!skipEditModeButton)
				{
					// Кнопка "Редактирование"
					if (EditButton.IsChecked.HasValue && EditButton.IsChecked.Value != PivotGrid.EditMode)
					{
						EditButton.IsChecked = new bool?(PivotGrid.EditMode);
					}
				}

				//if (IsUpdateable)
				//{
				// Делаем кнопки видимыми
				// EditButton.Visibility = UseChangesCasheButton.Visibility = ConfirmEditButton.Visibility = CancelEditButton.Visibility = Visibility.Visible;
				EditButton.IsEnabled = true;

				// Если кнопка btnEdit не нажата, то делаем недоступными кнопки btnUseChangesCache, btnSave, btnCancel
				UseChangesCasheButton.IsEnabled = EditButton.IsChecked.Value;
				PasteFromClipboardButton.IsEnabled = EditButton.IsChecked.Value;

				if (EditButton.IsChecked.Value)
				{
					ConfirmEditButton.IsEnabled = CancelEditButton.IsEnabled =
							PivotGrid.LocalChanges.Count > 0/* || m_AllocationArgs.Count > 0*/;
					/*btnModifications.Enabled = true;*/
				}
				else
				{
					ConfirmEditButton.IsEnabled = CancelEditButton.IsEnabled = EditButton.IsChecked.Value;
					//btnModifications.Enabled = false;
				}
				//}
				//else
				//{
				//    // Делаем кнопки невидимыми
				//    // EditButton.Visibility = UseChangesCasheButton.Visibility = ConfirmEditButton.Visibility = CancelEditButton.Visibility = Visibility.Collapsed;
				//    EditButton.IsEnabled = UseChangesCasheButton.IsEnabled = PasteFromClipboardButton.IsEnabled = ConfirmEditButton.IsEnabled = CancelEditButton.IsEnabled = false;
				//}

				//EditButton.IsChecked = new bool?(IsUpdateable);
				//UseChangesCasheButton.IsChecked = new bool?(UseChangesCashe);
			}
		}

		#endregion Управление возможностью редактирования ячеек

		#region Свойства для настройки на OLAP
		/// <summary>
		/// Описание соединения с БД для идентификации соединения на сервере (строка соединения либо ID)
		/// </summary>
		public String Connection
		{
			get
			{
				return PivotGrid.Connection;
			}
			set
			{
				PivotGrid.Connection = value;
			}
		}
		#endregion Свойства для настройки на OLAP

		public String Query { get; set; }
		public String UpdateScript
		{
			get { return PivotGrid.UpdateScript; }
			set
			{
				PivotGrid.UpdateScript = value;
				UpdateEditToolBarButtons();
			}
		}

		public MemberVisualizationTypes MemberVisualizationType
		{
			get { return PivotGrid.MemberVisualizationType; }
			set { PivotGrid.MemberVisualizationType = value; }
		}

		private void ForwardButton_Click(object sender, RoutedEventArgs e)
		{
			RunServiceCommand(ServiceCommandType.Forward);
		}

		/// <summary>
		/// Определяет возможность использования Expand, Collapse, DrillDown
		/// </summary>
		public bool RowsIsInteractive
		{
			get
			{
				return PivotGrid.Axis1_IsInteractive;
			}
			set
			{
				PivotGrid.Axis1_IsInteractive = value;
			}
		}

		/// <summary>
		/// Определяет возможность использования Expand, Collapse, DrillDown
		/// </summary>
		public bool ColumnsIsInteractive
		{
			get
			{
				return PivotGrid.Axis0_IsInteractive;
			}
			set
			{
				PivotGrid.Axis0_IsInteractive = value;
			}
		}

		public void ImportSizeInfo()
		{
			try
			{
				IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();
				if (isoStore != null)
				{
					if (isoStore.FileExists(IsoStorageFile))
					{
						IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(IsoStorageFile, System.IO.FileMode.Open, isoStore);
						StreamReader reader = new StreamReader(isoStream);
						String Text = reader.ReadToEnd();
						reader.Close();

						if (!String.IsNullOrEmpty(Text))
						{
							PivotGridSizeInfo sizeInfo = XmlSerializationUtility.XmlStr2Obj<PivotGridSizeInfo>(Text);
							if (sizeInfo != null)
							{
								PivotGrid.SetSizeInfo(sizeInfo);
							}
						}
					}
				}
			}
			catch
			{
			}
		}

		public void ExportSizeInfo()
		{
			PivotGridSizeInfo sizeInfo = PivotGrid.GetSizeInfo();
			if (sizeInfo != null)
			{
				try
				{
					IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();
					if (isoStore != null)
					{
						IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(IsoStorageFile, System.IO.FileMode.Create, isoStore);
						StreamWriter writer = new StreamWriter(isoStream);
						writer.Write(XmlSerializationUtility.Obj2XmlStr(sizeInfo));
						writer.Close();
					}
				}
				catch { }
			}
		}

		String m_IsoStorageFile = String.Empty;
		private String IsoStorageFile
		{
			get
			{
				if (String.IsNullOrEmpty(m_IsoStorageFile))
				{
					m_IsoStorageFile = this.Name + "_SI.xml";
					if (!String.IsNullOrEmpty(IsolatedStoragePrefix))
					{
						String[] items = IsolatedStoragePrefix.Split(new String[] { "." }, StringSplitOptions.RemoveEmptyEntries);
						if (items != null && items.Length > 0)
							m_IsoStorageFile = items[items.Length - 1] + "_" + m_IsoStorageFile;
					}
				}
				return m_IsoStorageFile;
			}
		}

		String m_IsolatedStoragePrefix = String.Empty;
		public String IsolatedStoragePrefix
		{
			get
			{
				return m_IsolatedStoragePrefix;
			}
			set
			{
				m_IsolatedStoragePrefix = value;
				m_IsoStorageFile = String.Empty;
			}
		}

		#region Настройки для подсказок
		/// <summary>
		/// Использование подсказки в области строк
		/// </summary>
		public bool UseRowsAreaHint
		{
			get { return PivotGrid.Rows_UseHint; }
			set { PivotGrid.Rows_UseHint = value; }
		}

		/// <summary>
		/// Использование подсказки в области колонок
		/// </summary>
		public bool UseColumnsAreaHint
		{
			get { return PivotGrid.Columns_UseHint; }
			set { PivotGrid.Columns_UseHint = value; }
		}

		/// <summary>
		/// Использование подсказки в области ячеек
		/// </summary>
		public bool UseCellsAreaHint
		{
			get { return PivotGrid.Cells_UseHint; }
			set { PivotGrid.Cells_UseHint = value; }
		}
		#endregion Настройки для подсказок

		bool m_UseNavigationButtons = true;
		/// <summary>
		/// Управляет отображением кнопок навигации в сводной таблице
		/// </summary>
		public bool UseNavigationButtons
		{
			get
			{
				return m_UseNavigationButtons;
			}
			set
			{
				if (value)
				{
					m_NavigationButtons_Splitter.Visibility = Visibility.Visible;
					ToBeginButton.Visibility = Visibility.Visible;
					BackButton.Visibility = Visibility.Visible;
					ForwardButton.Visibility = Visibility.Visible;
					ToEndButton.Visibility = Visibility.Visible;
				}
				else
				{
					m_NavigationButtons_Splitter.Visibility = Visibility.Collapsed;
					ToBeginButton.Visibility = Visibility.Collapsed;
					BackButton.Visibility = Visibility.Collapsed;
					ForwardButton.Visibility = Visibility.Collapsed;
					ToEndButton.Visibility = Visibility.Collapsed;
				}
				m_UseNavigationButtons = value;
			}
		}

		public IList<CellConditionsDescriptor> CustomCellsConditions
		{
			get
			{
				return PivotGrid.CustomCellsConditions;
			}
			set
			{
				PivotGrid.CustomCellsConditions = value;
			}
		}

		public bool DrillThroughCells
		{
			get { return PivotGrid.DrillThroughCells; }
			set { PivotGrid.DrillThroughCells = value; }
		}

		bool m_UseCellConditionsDesigner = false;
		/// <summary>
		/// Определяет необходимость использования дизайнера стилей для ячеек
		/// </summary>
		public bool UseCellConditionsDesigner
		{
			get { return m_UseCellConditionsDesigner; }
			set
			{
				m_UseCellConditionsDesigner = value;
				ConditionsDesignerButton.Visibility = value == true ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		public MemberClickBehaviorTypes DefaultMemberAction = MemberClickBehaviorTypes.DrillDown;
		public ColumnTitleClickBehavior ColumnTitleClickBehavior = ColumnTitleClickBehavior.SameAsRows;

		public bool AutoWidthColumns
		{
			get { return PivotGrid.AutoWidthColumns; }
			set { PivotGrid.AutoWidthColumns = value; }
		}

		public ViewModeTypes MembersViewMode
		{
			get
			{
				return PivotGrid.ColumnsViewMode;
			}
			set
			{
				PivotGrid.ColumnsViewMode = value;
				PivotGrid.RowsViewMode = value;
			}
		}

		public DataReorganizationTypes DataReorganizationType = DataReorganizationTypes.MergeNeighbors;

	}

	public enum ViewModeTypes
	{
		Tree,
		Grid
	}

	public enum MemberClickBehaviorTypes
	{
		None = 0,
		DrillDown = 1,
		ExpandCollapse = 2,
		SortByProperty = 3
	}
	public enum ColumnTitleClickBehavior
	{
		None = 0,
		DrillDown = 1,
		ExpandCollapse = 2,
		SortByProperty = 3,
		SameAsRows = 4,
		SortByValue = 5
	}
	public enum DrillDownMode
	{
		ByCurrentTupleShowSelf = 0,
		BySingleDimensionShowSelf = 1,
		ByCurrentTupleHideSelf = 2,
		BySingleDimensionHideSelf = 3
	}
}


