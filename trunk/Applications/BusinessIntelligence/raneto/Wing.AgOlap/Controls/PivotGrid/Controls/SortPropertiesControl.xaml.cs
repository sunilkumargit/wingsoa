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
using Ranet.Olap.Core.Providers;
using Ranet.AgOlap.Controls.General.ItemControls;
using Ranet.Olap.Core.Metadata;
using Ranet.AgOlap.Controls.General.Tree;
using Ranet.AgOlap.Controls.General;
using Ranet.AgOlap.Controls.ValueCopy;

namespace Ranet.AgOlap.Controls.PivotGrid.Controls
{
    public enum PivotTableSortTypes
    { 
        SortByProperty,
        SortAxisByMeasure,
        SortByValue
    }

    public partial class SortPropertiesControl : UserControl
    {
        public String CubeName
        {
            get { return tupleControl.CubeName; }
            set { tupleControl.CubeName = value; }
        }
        public String ConnectionID
        {
            get { return tupleControl.ConnectionID; }
            set { tupleControl.ConnectionID = value; }
        }
        public ILogService LogManager
        {
            get { return tupleControl.LogManager; }
            set { tupleControl.LogManager = value; }
        }

        public event EventHandler<GetIDataLoaderArgs> GetOlapDataLoader;
        void Raise_GetOlapDataLoader(GetIDataLoaderArgs args)
        {
            EventHandler<GetIDataLoaderArgs> handler = this.GetOlapDataLoader;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        public SortPropertiesControl()
        {
            InitializeComponent();

            tupleControl.GetOlapDataLoader += new EventHandler<GetIDataLoaderArgs>(tupleControl_GetOlapDataLoader);

            lblSortType.Text = Localization.SortingDirection_Label;
            lblNone.Text = Localization.Sorting_None_Label;
            lblAscending.Text = Localization.Sorting_Ascending_Label;
            lblDescending.Text = Localization.Sorting_Descending_Label;
            lblSortBy.Text = Localization.SortingBy_Label;
            lblProperty.Text = Localization.Property_Label;
            lblMeasure.Text = Localization.Measure_Label;

            comboProperty.Items.Add(new MemberPropertyItemControl(new MemberPropertyInfo("Caption", "Caption")));
            comboProperty.Items.Add(new MemberPropertyItemControl(new MemberPropertyInfo("Key0", "Key0")));

            comboProperty.SelectedIndex = 0;

            comboMeasure.Items.Add(new NoneItemControl());
            comboMeasure.SelectedIndex = 0;

            comboMeasure.IsEnabledChanged += new DependencyPropertyChangedEventHandler(comboMeasure_IsEnabledChanged);
            rbNone.Checked += new RoutedEventHandler(rbNone_Checked);
            rbNone.Unchecked += new RoutedEventHandler(rbNone_Unchecked);
        }

        void tupleControl_GetOlapDataLoader(object sender, GetIDataLoaderArgs e)
        {
            Raise_GetOlapDataLoader(e);
        }

        void comboMeasure_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (comboMeasure.IsEnabled && SortType == PivotTableSortTypes.SortAxisByMeasure)
            {
                if (!m_MeasuresIsLoaded)
                {
                    EventHandler<CustomEventArgs<EventArgs>> handler = LoadMeasures;
                    if (handler != null)
                    {
                        comboMeasure.Items.Clear();
                        WaitTreeControl ctrl = new WaitTreeControl() { Text = Localization.Loading };
                        comboMeasure.Items.Add(ctrl);
                        comboMeasure.SelectedIndex = 0;
                        comboMeasure.IsEnabled = false;

                        CustomEventArgs<EventArgs> args = new CustomEventArgs<EventArgs>(EventArgs.Empty);
                        handler(this, args);

                        if (args.Cancel)
                        {
                            comboMeasure.IsEnabled = true;
                            comboMeasure.Items.Clear();
                            comboMeasure.Items.Add(new NoneItemControl());
                            comboMeasure.SelectedIndex = 0;
                        }
                    }

                }
            }
        }

        void rbNone_Unchecked(object sender, RoutedEventArgs e)
        {
            switch (SortType)
            {
                case PivotTableSortTypes.SortAxisByMeasure:
                    comboMeasure.IsEnabled = true;
                    break;
                case PivotTableSortTypes.SortByProperty:
                    comboProperty.IsEnabled = true;
                    break;
                case PivotTableSortTypes.SortByValue:
                    tupleControl.IsEnabled = true;
                    break;
            }
        }

        void rbNone_Checked(object sender, RoutedEventArgs e)
        {
            comboProperty.IsEnabled = false;
            comboMeasure.IsEnabled = false;
            tupleControl.IsEnabled = false;
        }

        bool m_MeasuresIsLoaded = false;
        public event EventHandler<CustomEventArgs<EventArgs>> LoadMeasures;


        public void InitializeMeasuresList(List<MeasureInfo> measures)
        {
            comboMeasure.Items.Clear();
            comboMeasure.Items.Add(new NoneItemControl());

            if (measures != null)
            {
                m_MeasuresIsLoaded = true;
                foreach (var item in measures)
                { 
                    comboMeasure.Items.Add(new MeasureItemControl(item));
                }
            }

            comboMeasure.IsEnabled = true;
            SelectMeasure(m_SortDescriptor != null ? m_SortDescriptor.SortBy : String.Empty);
        }

        PivotTableSortTypes m_SortType = PivotTableSortTypes.SortByProperty;
        public PivotTableSortTypes SortType
        {
            get { return m_SortType; }
            set
            {
                m_SortType = value;
                lblMeasure.Visibility = m_SortType == PivotTableSortTypes.SortAxisByMeasure ? Visibility.Visible : Visibility.Collapsed;
                comboMeasure.Visibility = m_SortType == PivotTableSortTypes.SortAxisByMeasure ? Visibility.Visible : Visibility.Collapsed;

                lblProperty.Visibility = m_SortType == PivotTableSortTypes.SortByProperty ? Visibility.Visible : Visibility.Collapsed;
                comboProperty.Visibility = m_SortType == PivotTableSortTypes.SortByProperty ? Visibility.Visible : Visibility.Collapsed;

                lblContext.Visibility = m_SortType == PivotTableSortTypes.SortByValue ? Visibility.Visible : Visibility.Collapsed;
                tupleControl.Visibility = m_SortType == PivotTableSortTypes.SortByValue ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        SortDescriptor m_SortDescriptor;
        public void Initialize(PivotTableSortTypes sortType, SortDescriptor sortDescriptor)
        {
            SortType = sortType;
            m_SortDescriptor = sortDescriptor;

            // Инициализируем как сортируем
            if (sortDescriptor == null)
            {
                rbNone.IsChecked = true;
            }
            else
            {
                switch (sortDescriptor.Type)
                {
                    case SortTypes.Ascending:
                        rbAscending.IsChecked = true;
                        break;
                    case SortTypes.Descending:
                        rbDescending.IsChecked = true;
                        break;
                    default:
                        rbNone.IsChecked = true;
                        break;
                }
            }

            // Инициализируем по чем сортируем
            switch (sortType)
            {
                case PivotTableSortTypes.SortByProperty:
                    SelectProperty(sortDescriptor != null ? sortDescriptor.SortBy : String.Empty);
                    break;
                case PivotTableSortTypes.SortAxisByMeasure:
                    SelectMeasure(sortDescriptor != null ? sortDescriptor.SortBy : String.Empty);
                    break;
                case PivotTableSortTypes.SortByValue:
                    InitializeTuple(sortDescriptor as SortByValueDescriptor);
                    break;
            }
        }

        public SortDescriptor SortDescriptor
        {
            get {
                SortDescriptor sortDescriptor = null;

                switch (SortType)
                {
                    case PivotTableSortTypes.SortByProperty:
                        sortDescriptor = new SortDescriptor();
                        if (!(rbNone.IsChecked.Value))
                        {
                            MemberPropertyItemControl property = comboProperty.SelectedItem as MemberPropertyItemControl;
                            if (property != null && property.Info != null)
                                sortDescriptor.SortBy = property.Info.UniqueName;
                        }
                        break;
                    case PivotTableSortTypes.SortAxisByMeasure:
                        sortDescriptor = new SortByMeasureDescriptor();
                        if (!(rbNone.IsChecked.Value))
                        {
                            MeasureItemControl measure = comboMeasure.SelectedItem as MeasureItemControl;
                            if (measure != null && measure.Info != null)
                                sortDescriptor.SortBy = measure.Info.UniqueName;
                        }
                        break;
                    case PivotTableSortTypes.SortByValue:
                        sortDescriptor = new SortByValueDescriptor();
                        if (!(rbNone.IsChecked.Value))
                        {
                            Dictionary<String, String> tuple = new Dictionary<string, string>();
                            foreach (var item in tupleControl.Tuple)
                            {
                                if (!String.IsNullOrEmpty(item.MemberUniqueName) &&
                                    !String.IsNullOrEmpty(item.HierarchyUniqueName))
                                {
                                    if (item.IsCustom)
                                    {
                                        (sortDescriptor as SortByValueDescriptor).MeasureUniqueName = item.MemberUniqueName;
                                    }
                                    else
                                    {
                                        if (!tuple.ContainsKey(item.HierarchyUniqueName))
                                            tuple.Add(item.HierarchyUniqueName, item.MemberUniqueName);
                                    }
                                }
                            }

                            (sortDescriptor as SortByValueDescriptor).Tuple = tuple;
                        }
                        break;
                }

                if (sortDescriptor != null)
                {
                    if (rbAscending.IsChecked.Value)
                        sortDescriptor.Type = SortTypes.Ascending;
                    if (rbDescending.IsChecked.Value)
                        sortDescriptor.Type = SortTypes.Descending;
                }
                return sortDescriptor;
            }
        }

        private void lblNone_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!rbNone.IsChecked.Value)
            {
                rbNone.IsChecked = new bool?(true);
            }
        }

        private void lblAscending_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!rbAscending.IsChecked.Value)
            {
                rbAscending.IsChecked = new bool?(true);
            }
        }

        private void lblDescending_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!rbDescending.IsChecked.Value)
            {
                rbDescending.IsChecked = new bool?(true);
            }
        }

        void InitializeTuple(SortByValueDescriptor sortDescriptor)
        {
            var tuple = new List<TupleItem>();
            if (sortDescriptor != null)
            {
                tuple = new List<TupleItem>();
                foreach (var item in sortDescriptor.Tuple)
                {
                    var tuple_item = new TupleItem() { HierarchyUniqueName = item.Key, MemberUniqueName = item.Value };
                    tuple.Add(tuple_item);
                }
                
                // Если меры в тапле нет, то добавляем ее искусственно
                if (!sortDescriptor.Tuple.ContainsKey("[Measures]"))
                {
                    var tuple_item = new TupleItem() { HierarchyUniqueName = "[Measures]", MemberUniqueName = sortDescriptor.MeasureUniqueName,  IsReadOnly = false, IsCustom = true };
                    tuple.Add(tuple_item);
                }
            }

            tupleControl.Initialize(tuple);
        }

        public void SelectMeasure(String uniqueName)
        {
            if (String.IsNullOrEmpty(uniqueName))
            {
                if (comboMeasure.Items.Count > 0)
                    comboMeasure.SelectedIndex = 0;
            }
            else
            {
                for (int i = 0; i < comboMeasure.Items.Count; i++)
                {
                    var ctrl = comboMeasure.Items[i] as MeasureItemControl;
                    if (ctrl != null && ctrl.Info != null && ctrl.Info.UniqueName == uniqueName)
                    {
                        comboMeasure.SelectedIndex = i;
                        return;
                    }
                }
            }
        }

        public void SelectProperty(String name)
        {
            if (String.IsNullOrEmpty(name))
            {
                if (comboProperty.Items.Count > 0)
                    comboProperty.SelectedIndex = 0;
            }
            else
            {
                for (int i = 0; i < comboProperty.Items.Count; i++)
                {
                    var ctrl = comboProperty.Items[i] as MemberPropertyItemControl;
                    if (ctrl != null && ctrl.Info != null && ctrl.Info.Name == name)
                    {
                        comboProperty.SelectedIndex = i;
                        return;
                    }
                }
            }
        }
    }
}
