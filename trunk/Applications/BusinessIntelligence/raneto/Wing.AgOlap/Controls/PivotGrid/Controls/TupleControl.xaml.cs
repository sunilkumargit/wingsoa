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
using Ranet.AgOlap.Controls.ValueCopy;
using Ranet.AgOlap.Controls.General;
using Ranet.AgOlap.Controls.MemberChoice.Info;
using System.ComponentModel;

namespace Ranet.AgOlap.Controls.PivotGrid.Controls
{
    public partial class TupleControl : UserControl
    {
        public String CubeName = String.Empty;
        public String ConnectionID = String.Empty;
        public ILogService LogManager = null;

        public event EventHandler<GetIDataLoaderArgs> GetOlapDataLoader;
        void Raise_GetOlapDataLoader(GetIDataLoaderArgs args)
        {
            EventHandler<GetIDataLoaderArgs> handler = this.GetOlapDataLoader;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        public TupleControl()
        {
            InitializeComponent();

            coordinatesGrid.AlternatingRowBackground = new SolidColorBrush(Colors.White);
            coordinatesGrid.RowBackground = new SolidColorBrush(Colors.White);
            coordinatesGrid.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            coordinatesGrid.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            coordinatesGrid.RowHeight = 22;
        }

        public void Initialize(List<TupleItem> list)
        {
            coordinatesGrid.ItemsSource = list;
        }

        public List<TupleItem> Tuple
        {
            get { return coordinatesGrid.ItemsSource as List<TupleItem>; }
        }

        private void ChangeMember_Button_Click(object sender, RoutedEventArgs e)
        {
            BeginMemberEdit(coordinatesGrid.SelectedItem as TupleItem);
        }

        private void DeleteMember_Button_Click(object sender, RoutedEventArgs e)
        {
            var item = coordinatesGrid.SelectedItem as TupleItem;
            if (item != null)
            {
                item.MemberUniqueName = String.Empty;
            }
        }

        bool BeginMemberEdit(TupleItem item)
        {
            if (item != null)
            {
                IChoiceDialog dlg = null;
                if (!String.IsNullOrEmpty(item.HierarchyUniqueName) && m_ChoiceDialogs.ContainsKey(item.HierarchyUniqueName))
                {
                    dlg = m_ChoiceDialogs[item.HierarchyUniqueName];
                }
                
                if (dlg == null)
                {
                    if (item.HierarchyUniqueName == "[Measures]")
                    {
                        // Выбор меры
                        MeasureChoiceDialog dialog = new MeasureChoiceDialog();
                        dialog.ChoiceControl.CubeName = CubeName;
                        dialog.ChoiceControl.Connection = ConnectionID;
                        dialog.ChoiceControl.LogManager = LogManager;
                        GetIDataLoaderArgs args = new GetIDataLoaderArgs();
                        Raise_GetOlapDataLoader(args);
                        if (args.Handled)
                            dialog.ChoiceControl.OlapDataLoader = args.Loader;
                        dialog.Tag = item;
                        dialog.DialogOk += new EventHandler(Dialog_DialogOk);
                        dialog.DialogCancel += new EventHandler(Dialog_DialogCancel);
                        dialog.Show();
                        dlg = dialog;
                    }
                    else
                    {
                        // Выбор элемента измерения
                        MemberChoiceDialog dialog = new MemberChoiceDialog();
                        dialog.ChoiceControl.CubeName = CubeName;
                        dialog.ChoiceControl.Connection = ConnectionID;
                        dialog.ChoiceControl.HierarchyUniqueName = item.HierarchyUniqueName;
                        dialog.ChoiceControl.LogManager = LogManager;

                        GetIDataLoaderArgs args = new GetIDataLoaderArgs();
                        Raise_GetOlapDataLoader(args);
                        if (args.Handled)
                            dialog.ChoiceControl.OlapDataLoader = args.Loader;

                        dialog.Tag = item;
                        dialog.DialogOk += new EventHandler(Dialog_DialogOk);
                        dialog.DialogCancel += new EventHandler(Dialog_DialogCancel);
                        dialog.Show();
                        dlg = dialog;
                    }
                    if(!String.IsNullOrEmpty(item.HierarchyUniqueName))
                        m_ChoiceDialogs[item.HierarchyUniqueName] = dlg;
                }

                if (dlg != null)
                {
                    dlg.Show();
                    return true;
                }
            }
            return false;
        }

        void Dialog_DialogCancel(object sender, EventArgs e)
        {
            coordinatesGrid.CancelEdit();
        }

        void Dialog_DialogOk(object sender, EventArgs e)
        {
            MemberChoiceDialog dlg = sender as MemberChoiceDialog;
            if (dlg != null)
            {
                var item = dlg.Tag as TupleItem;
                if (item != null)
                {
                    List<MemberChoiceSettings> selectedInfo = dlg.ChoiceControl.SelectedInfo;
                    if (selectedInfo != null && selectedInfo.Count == 1)
                    {
                        item.MemberUniqueName = dlg.ChoiceControl.SelectedInfo[0].UniqueName;
                    }
                }
                return;
            }

            MeasureChoiceDialog measure_dlg = sender as MeasureChoiceDialog;
            if (measure_dlg != null)
            {
                var item = measure_dlg.Tag as TupleItem;
                if (item != null)
                {
                    if (measure_dlg.ChoiceControl.SelectedInfo != null)
                    {
                        item.MemberUniqueName = measure_dlg.ChoiceControl.SelectedInfo.UniqueName;
                    }
                }
            }
        }

        /// <summary>
        /// Кэш диалогов для выбора элементов
        /// </summary>
        Dictionary<String, IChoiceDialog> m_ChoiceDialogs = new Dictionary<String, IChoiceDialog>();

    }

    public class TupleItem : INotifyPropertyChanged
    {
        /// <summary>
        /// Флаг определяет, что данный элемент создан пользователем и в тапле его реально нет
        /// </summary>
        public bool IsCustom = false;

        String m_HierarchyUniqueName = String.Empty;
        public String HierarchyUniqueName
        {
            get { return m_HierarchyUniqueName; }
            set
            {
                m_HierarchyUniqueName = value;
                Raise_PropertyChanged("HierarchyUniqueName");
            }
        }

        String m_MemberUniqueName = String.Empty;
        public String MemberUniqueName
        {
            get { return m_MemberUniqueName; }
            set
            {
                m_MemberUniqueName = value;
                Raise_PropertyChanged("MemberUniqueName");
            }
        }

        public bool IsReadOnly = true;

        /// <summary>
        /// Видимость контролов для редактирования координаты (Изменение источника, приемника, конки удаления). Используется ТОЛЬКО для управления отображением в гриде
        /// </summary>
        public Visibility ModifyControlsVisibility
        {
            get
            {
                if (IsReadOnly)
                    return Visibility.Collapsed;
                return Visibility.Visible;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void Raise_PropertyChanged(String propName)
        {
            if (!String.IsNullOrEmpty(propName))
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(propName));
                }
            }
        }
    }
}
