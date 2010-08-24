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
using Ranet.AgOlap.Controls.General;
using Ranet.AgOlap.Commands;
using Ranet.Olap.Core;
using Ranet.Olap.Core.Data;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using System.Text;
using Ranet.Olap.Core.Providers.ClientServer;

namespace Ranet.AgOlap.Controls
{
    public class DatePickerEx : DatePicker
    {
        public DatePickerEx()
        {
            m_Waiting = new BusyControl();
            m_Waiting.Text = Localization.Loading;

            this.Loaded += new RoutedEventHandler(DatePickerEx_Loaded);
        }

        void DatePickerEx_Loaded(object sender, RoutedEventArgs e)
        {
        }
        public event EventHandler DisplayDateChanged;

        BusyControl m_Waiting;
        Calendar m_Calendar = null;
        public Calendar PopUpCalendar
        {
            get { return m_Calendar; }
        }

        DatePickerTextBox m_TextBox = null;
        public DatePickerTextBox TextBox
        {
            get { return m_TextBox; }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Popup popup = base.GetTemplateChild("Popup") as Popup;
            m_Calendar = null;
            
            if (popup != null && popup.Child != null)
            { 
                Canvas canvas = popup.Child as Canvas;
                if (canvas != null)
                {
                    foreach (UIElement item in canvas.Children)
                    {
                        m_Calendar = item as Calendar;
                        if (m_Calendar != null)
                            break;
                    }
                }
            }

            m_TextBox = base.GetTemplateChild("TextBox") as DatePickerTextBox;
            //m_TextBox.FontSize = 11;
            //m_TextBox.Padding = new Thickness(2, 0, 2, 0);
            m_TextBox.VerticalAlignment = VerticalAlignment.Center;
            m_TextBox.Text = " ";

            if (m_TextBox != null)
                m_TextBox.IsReadOnly = true;

            var readOnlyVisualElement = base.GetTemplateChild("ReadOnlyVisualElement") as Border;
            if (readOnlyVisualElement != null)
            {
                readOnlyVisualElement.Background = new SolidColorBrush(Colors.Transparent);
            }

            if (m_Calendar != null)
            {
                m_Calendar.DisplayDateChanged += new EventHandler<CalendarDateChangedEventArgs>(calendar_DisplayDateChanged);
            }
        }

        void calendar_DisplayDateChanged(object sender, CalendarDateChangedEventArgs e)
        {
            EventHandler handler = DisplayDateChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }


    public class DatePickerCtrl : AgControlBase
    {
        Grid LayoutRoot;
        DatePickerEx m_Picker;
        protected DatePickerEx Picker
        {
            get { return m_Picker; }
        }

        public DatePickerCtrl()
        {
            this.Height = 22;
            LayoutRoot = new Grid();

            m_Picker = new DatePickerEx();
            m_Picker.CalendarOpened += new RoutedEventHandler(Picker_CalendarOpened);
            m_Picker.CalendarClosed += new RoutedEventHandler(Picker_CalendarClosed);
            m_Picker.SelectedDateChanged += new EventHandler<SelectionChangedEventArgs>(Picker_SelectedDateChanged);
            m_Picker.DisplayDateChanged += new EventHandler(Picker_DisplayDateChanged);

            m_Picker.Text = String.Empty;
            m_Picker.DisplayDate = DateTime.Today;

            LayoutRoot.Children.Add(m_Picker);
            
            this.Content = LayoutRoot;
        }

        void Picker_DisplayDateChanged(object sender, EventArgs e)
        {
            UpdateDates();
        }

        void Picker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        void Picker_CalendarClosed(object sender, RoutedEventArgs e)
        {
            if (Picker.SelectedDate.HasValue)
            {
                Picker.Text = Picker.SelectedDate.Value.ToShortDateString();
                // Если дата изменилась с момента открытия
                if (Picker.SelectedDate.Value != m_Opened_SelectedDate)
                {
                    ApplySelection();
                }
            }
            else
            {
                Picker.Text = String.Empty;
            }
        }

        protected virtual void ApplySelection()
        {}

        DateTime m_Opened_SelectedDate = DateTime.MinValue;
        void Picker_CalendarOpened(object sender, RoutedEventArgs e)
        {
            if (Picker.SelectedDate != null && Picker.SelectedDate.HasValue)
            {
                m_Opened_SelectedDate = Picker.SelectedDate.Value;
                Picker.DisplayDate = Picker.SelectedDate.Value;
            }
            else
            {
                Picker.DisplayDate = DateTime.Today;
            }
            
            if (NeedReload)
            {
                Initialize();
                NeedReload = false;
            }
        }

        protected bool NeedReload = true;

        #region Свойства для настройки на OLAP
        private String m_Connection = String.Empty;
        /// <summary>
        /// Описание соединения с БД
        /// </summary>
        public String Connection
        {
            get
            {
                return m_Connection;
            }
            set
            {
                m_Connection = value;
                NeedReload = true;
            }
        }

        private string m_CubeName;
        /// <summary>
        /// Имя OLAP куба
        /// </summary>
        public String CubeName
        {
            get
            {
                return m_CubeName;
            }
            set
            {
                m_CubeName = value;
                NeedReload = true;
            }
        }

        private string m_DayLevelUniqueName = String.Empty;
        /// <summary>
        /// Уникальное имя уровня, хранящего дни
        /// </summary>
        public String DayLevelUniqueName
        {
            get
            {
                return m_DayLevelUniqueName;
            }
            set
            {
                m_DayLevelUniqueName = value;
                NeedReload = true;
            }
        }

        private string m_DateToUniqueNameTemplate = String.Empty;
        /// <summary>
        /// Шаблон для преобразования даты в уникальное имя
        /// </summary>
        public String DateToUniqueNameTemplate
        {
            get
            {
                return m_DateToUniqueNameTemplate;
            }
            set
            {
                m_DateToUniqueNameTemplate = value;
                NeedReload = true;
            }
        }
        #endregion Свойства для настройки на OLAP

        bool m_IsBusy = false;
        public bool IsBusy
        {
            get { return m_IsBusy; }
            set {
                m_IsBusy = value;
                if (Picker.PopUpCalendar != null)
                {
                    if (value)
                        Picker.PopUpCalendar.IsEnabled = false;
                    else
                        Picker.PopUpCalendar.IsEnabled = true;
                }
            }
        }

        public void Initialize()
        {
            m_LoadedMembers.Clear();

            LoadDates();
        }

        void LoadDates()
        {
            if (String.IsNullOrEmpty(Connection))
            {
                // Сообщение в лог
                StringBuilder builder = new StringBuilder();
                if (String.IsNullOrEmpty(Connection))
                    builder.Append(Localization.Connection_PropertyDesc);
                LogManager.LogError(this, String.Format(Localization.ControlSettingsNotInitialized_Message, builder.ToString()));
                return;
            }

            if (!String.IsNullOrEmpty(DayLevelUniqueName))
            {
                String query = "Select {" + DayLevelUniqueName +
                    ".Members} on 0, {} on 1 from " + OlapHelper.ConvertToQueryStyle(CubeName);

                IsBusy = true;

                MdxQueryArgs args = CommandHelper.CreateMdxQueryArgs(Connection, query);
                OlapDataLoader.LoadData(args, null);
            }
        }

        #region Загрузчики
        IDataLoader m_OlapDataLoader = null;
        public IDataLoader OlapDataLoader
        {
            set
            {
                if (m_OlapDataLoader != null)
                {
                    m_OlapDataLoader.DataLoaded -= new EventHandler<DataLoaderEventArgs>(Loader_DataLoaded);
                }
                m_OlapDataLoader = value;
                m_OlapDataLoader.DataLoaded += new EventHandler<DataLoaderEventArgs>(Loader_DataLoaded);
            }
            get
            {
                if (m_OlapDataLoader == null)
                {
                    m_OlapDataLoader = GetDataLoader();
                    m_OlapDataLoader.DataLoaded += new EventHandler<DataLoaderEventArgs>(Loader_DataLoaded);
                }
                return m_OlapDataLoader;
            }
        }

        protected virtual IDataLoader GetDataLoader()
        {
            return new OlapDataLoader(URL);
        }
        #endregion Загрузчики

        void Loader_DataLoaded(object sender, DataLoaderEventArgs e)
        {
            IsBusy = false;

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

            //CellSetData cs_descr = XmlSerializationUtility.XmlStr2Obj<CellSetData>(e.Result);
            CellSetData cs_descr = CellSetData.Deserialize(e.Result.Content);
            OnDatesLoaded(cs_descr);

            UpdateDates();
        }

        Dictionary<String, MemberData> m_LoadedMembers = new Dictionary<String, MemberData>();

        void OnDatesLoaded(CellSetData cs_descr)
        {
            m_LoadedMembers.Clear();
            if (cs_descr.Axes.Count > 0)
            {
                foreach (PositionData position in cs_descr.Axes[0].Positions)
                {
                    if (position.Members.Count > 0)
                    {
                        if (!m_LoadedMembers.ContainsKey(cs_descr.Axes[0].Members[position.Members[0].Id].UniqueName))
                            m_LoadedMembers.Add(cs_descr.Axes[0].Members[position.Members[0].Id].UniqueName, cs_descr.Axes[0].Members[position.Members[0].Id]);
                    }
                }
            }
        }

        /// <summary>
        /// Кэш сгенеренных уникальных имен для дат
        /// </summary>
        Dictionary<DateTime, String> m_Dates = new Dictionary<DateTime, string>();

        void UpdateDates()
        {
            Picker.BlackoutDates.Clear();

            DateTime tmp;
            DateTime averageDate = new DateTime(Picker.DisplayDate.Year, Picker.DisplayDate.Month, 1);
            tmp = averageDate.AddMonths(-1);
            DateTime firstDate = new DateTime(tmp.Year, tmp.Month, 15);
            tmp = averageDate.AddMonths(1);
            DateTime lastDate = new DateTime(tmp.Year, tmp.Month, 15);

            DateTime start = DateTime.Now;

            DateTime date = firstDate;
            int x = 0;

            DateTime blackout_Start = DateTime.MinValue;
            DateTime blackout_Stop = DateTime.MinValue;
            while (date <= lastDate)
            {
                String un = String.Empty;
                if (m_Dates.ContainsKey(date))
                {
                    un = m_Dates[date];
                }

                else
                {
                    un = GetDayUniqueName(date);
                    x++;
                    if (!String.IsNullOrEmpty(un))
                    {
                        m_Dates[date] = un;
                    }
                }

                if (!String.IsNullOrEmpty(DateToUniqueNameTemplate))
                {
                    if (!m_LoadedMembers.ContainsKey(un))
                    {
                        if (blackout_Start == DateTime.MinValue || blackout_Stop == DateTime.MinValue)
                        {
                            blackout_Start = date;
                            blackout_Stop = date;
                        }
                        else
                        {
                            blackout_Stop = date;
                        }
                    }
                    else
                    {
                        if (blackout_Start != DateTime.MinValue &&
                            blackout_Stop != DateTime.MinValue)
                        {
                            Picker.BlackoutDates.Add(new CalendarDateRange(blackout_Start, blackout_Stop));
                        }

                        blackout_Start = DateTime.MinValue;
                        blackout_Stop = DateTime.MinValue;
                    }
                }
                date = date.AddDays(1);
            }

            if (blackout_Start != DateTime.MinValue &&
                blackout_Stop != DateTime.MinValue)
            {
                try
                {
                    Picker.BlackoutDates.Add(new CalendarDateRange(blackout_Start, blackout_Stop));
                }
                catch { }
            }

            DateTime stop = DateTime.Now;

            System.Diagnostics.Debug.WriteLine((stop - start).ToString() + " " + x.ToString());
        }


        protected String GetDayUniqueName(DateTime date)
        {
            String result = String.Empty;
            if (!String.IsNullOrEmpty(DateToUniqueNameTemplate))
            {
                String year = date.Year.ToString("0000");
                String year_short = year.Substring(2, 2);

                int q = ((int)Math.Ceiling(date.Month / 3.0));
                String quarter = q.ToString("00");
                String quarter_short = q.ToString().Length == 1 ? quarter.Substring(1, 1) : quarter;

                String month = date.Month.ToString("00");
                String month_short = date.Month.ToString().Length == 1 ? month.Substring(1, 1) : month;

                String day = date.Day.ToString("00");
                String day_short = date.Day.ToString().Length == 1 ? day.Substring(1, 1) : day;

                result = DateToUniqueNameTemplate;
                result = result.Replace("<YYYY>", year);
                result = result.Replace("<YY>", year_short);
                result = result.Replace("<QQ>", quarter);
                result = result.Replace("<Q>", quarter_short);
                result = result.Replace("<MM>", month);
                result = result.Replace("<M>", month_short);
                result = result.Replace("<DD>", day);
                result = result.Replace("<D>", day_short);
            }
            return result;
        }
    }
}
