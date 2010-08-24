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
using Ranet.Olap.Core.Data;
using Ranet.Olap.Core;
using Ranet.AgOlap.Commands;
using System.Collections.Generic;
using Ranet.AgOlap.Controls.ValueDelivery;
using Ranet.AgOlap.Controls.PivotGrid.Data;
using System.Globalization;
using Ranet.Olap.Core.Providers.ClientServer;

namespace Ranet.AgOlap.Controls
{
    public class DateEventArgs : EventArgs
    {
        public readonly DateTime Date = DateTime.MinValue;

        public DateEventArgs(DateTime date)
        {
            Date = date;
        }
    }

    public class DateChoiceCtrl : AgControlBase, IChoiceControl
    {
        System.Windows.Controls.Calendar m_Calendar;
        BusyControl m_Waiting;

        public DateChoiceCtrl()
        {
            Grid LayoutRoot = new Grid();
            m_Calendar = new System.Windows.Controls.Calendar();
            m_Calendar.SelectionMode = CalendarSelectionMode.SingleDate;
            m_Calendar.DisplayDate = DateTime.Today;
            m_Calendar.DisplayDateChanged += new EventHandler<CalendarDateChangedEventArgs>(m_Calendar_DisplayDateChanged);
            m_Calendar.SelectedDatesChanged += new EventHandler<SelectionChangedEventArgs>(m_Calendar_SelectedDatesChanged);
            
            LayoutRoot.Children.Add(m_Calendar);

            m_Waiting = new BusyControl();
            m_Waiting.Text = Localization.Loading;
            LayoutRoot.Children.Add(m_Waiting);
            Grid.SetRow(m_Waiting, 1);

            IsBusy = false;

            this.Content = LayoutRoot;

            resDoubleClickTimer = new Storyboard();
            resDoubleClickTimer.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 250));
            resDoubleClickTimer.Completed += new EventHandler(Storyboard_Completed);
            LayoutRoot.Resources.Add("resDoubleClickTimer", resDoubleClickTimer);

            m_Calendar.MouseLeftButtonDown += new MouseButtonEventHandler(Calendar_MouseLeftButtonDown);
        }

        Storyboard resDoubleClickTimer;

        public event EventHandler MouseDoubleClick;
        protected void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            EventHandler handler = MouseDoubleClick;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        int m_ClickCount = 0;
        MouseButtonEventArgs m_LastArgs;

        void Calendar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            m_ClickCount++;
            m_LastArgs = e;
            resDoubleClickTimer.Begin();
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            if (m_ClickCount > 1)
            {
                this.OnMouseDoubleClick(m_LastArgs);
            }
            m_ClickCount = 0;
        }

        public DateTime? SelectedDate
        {
            get { return m_Calendar.SelectedDate; }
        }

        public event EventHandler<DateEventArgs> SelectedItemChanged;
        void Raise_SelectedItemChanged(DateTime date)
        {
            EventHandler<DateEventArgs> handler = SelectedItemChanged;
            if (handler != null)
            {
                handler(this, new DateEventArgs(date));
            }
        }

        /// <summary>
        /// Событие генерируется после окончания выбора
        /// </summary>
        public event EventHandler ApplySelection;

        /// <summary>
        /// Генерирует событие "Выбор окончен"
        /// </summary>
        private void Raise_ApplySelection()
        {
            EventHandler handler = ApplySelection;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        bool m_IsReadyToSelection = false;
        public bool IsReadyToSelection
        {
            get
            {
                return m_IsReadyToSelection;
            }
        }

        void m_Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (e.AddedItems != null && e.AddedItems.Count > 0)
            //{
            //    if (e.AddedItems[0] is DateTime)
            //    {
            //        DateTime date = (DateTime)e.AddedItems[0];
            //        m_IsReadyToSelection = !m_Calendar.BlackoutDates.Contains(date);
            //        Raise_SelectedItemChanged(date);
            //    }
            //}
            if (m_Calendar.SelectedDate.HasValue)
            {
                m_IsReadyToSelection = !m_Calendar.BlackoutDates.Contains(m_Calendar.SelectedDate.Value);
                Raise_SelectedItemChanged(m_Calendar.SelectedDate.Value);
            }
            else
            {
                m_IsReadyToSelection = false;
            }
        }

        void m_Calendar_DisplayDateChanged(object sender, CalendarDateChangedEventArgs e)
        {
            UpdateDates();
            m_IsReadyToSelection = !m_Calendar.BlackoutDates.Contains(m_Calendar.DisplayDate);
            Raise_SelectedItemChanged(m_Calendar.DisplayDate);
        }

        public bool m_IsBusy = false;
        public bool IsBusy
        {
            get
            {
                return m_IsBusy;
            }
            set
            {
                if (value)
                {
                    m_Calendar.IsEnabled = false;
                    m_Waiting.Visibility = Visibility.Visible;
                }
                else
                {
                    m_Calendar.IsEnabled = true;
                    m_Waiting.Visibility = Visibility.Collapsed;
                }
                m_IsBusy = value;
            }
        }

        #region Свойства для настройки на OLAP
        /// <summary>
        /// 
        /// </summary>
        private String m_Connection = String.Empty;
        /// <summary>
        /// Описание соединения с БД для идентификации соединения на сервере (строка соединения либо ID)
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
            }
        }

        /// <summary>
        /// Имя OLAP куба
        /// </summary>
        String m_CubeName = String.Empty;
        /// <summary>
        /// Имя OLAP куба
        /// </summary>
        public String CubeName
        {
            get
            {
                return this.m_CubeName;
            }
            set
            {
                m_CubeName = value;
            }
        }

        /// <summary>
        /// Уникальное имя уровня, хранящего дни
        /// </summary>
        String m_DayLevelUniqueName = String.Empty;
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
            }
        }
        #endregion Свойства для настройки на OLAP

        public void Initialize()
        {
            m_LoadedMembers.Clear();

            m_Calendar.DisplayDate = DateTime.Today;

            LoadDates();
        }

        void LoadDates()
        {
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
                    if(position.Members.Count > 0)
                    {
                        if(!m_LoadedMembers.ContainsKey(cs_descr.Axes[0].Members[position.Members[0].Id].UniqueName))
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
            m_Calendar.BlackoutDates.Clear();

            DateTime tmp;
            DateTime averageDate = new DateTime(m_Calendar.DisplayDate.Year, m_Calendar.DisplayDate.Month, 1);
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
                            m_Calendar.BlackoutDates.Add(new CalendarDateRange(blackout_Start, blackout_Stop));
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
                m_Calendar.BlackoutDates.Add(new CalendarDateRange(blackout_Start, blackout_Stop));
            }

            DateTime stop = DateTime.Now;

            System.Diagnostics.Debug.WriteLine((stop - start).ToString() + " " + x.ToString());
        }

        public String DateToUniqueNameTemplate { get; set; }

        String GetDayUniqueName(DateTime date)
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
