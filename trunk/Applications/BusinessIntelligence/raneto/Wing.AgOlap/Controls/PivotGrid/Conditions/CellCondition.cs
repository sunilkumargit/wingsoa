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
using System.Windows.Media.Imaging;

namespace Ranet.AgOlap.Controls.PivotGrid.Conditions
{
    /// <summary>
    /// Тип условия
    /// </summary>
    //[Serializable]
    public enum CellConditionType
    {
        // Summary:
        //     The style is not applied to any cell.
        None = 0,
        //
        // Summary:
        //     The style is applied to cells (or corresponding rows) whose values match
        //     the DevExpress.XtraEditors.StyleFormatConditionBase.Value1 property value.
        Equal = 1,
        //
        // Summary:
        //     The style is applied to cells (or corresponding rows) whose values do not
        //     match the DevExpress.XtraEditors.StyleFormatConditionBase.Value1 property
        //     value.
        NotEqual = 2,
        //
        // Summary:
        //     The style is applied to cells (or corresponding rows) whose values fall into
        //     the range specified by the DevExpress.XtraEditors.StyleFormatConditionBase.Value1
        //     and DevExpress.XtraEditors.StyleFormatConditionBase.Value2 properties.  
        // x < Value < y
        Between = 3,
        //
        // Summary:
        //     The style is applied to cells (or corresponding rows) whose values fall outside
        //     of the range specified by the DevExpress.XtraEditors.StyleFormatConditionBase.Value1
        //     and DevExpress.XtraEditors.StyleFormatConditionBase.Value2 properties.  
        NotBetween = 4,
        //
        // Summary:
        //     The style is applied to cells (or corresponding rows) whose values are less
        //     than that specified by the DevExpress.XtraEditors.StyleFormatConditionBase.Value1
        //     property.
        Less = 5,
        //
        // Summary:
        //     The style is applied to cells (or corresponding rows) whose values are greater
        //     than that specified by the DevExpress.XtraEditors.StyleFormatConditionBase.Value1
        //     property.
        Greater = 6,
        //
        // Summary:
        //     The style is applied to cells (or corresponding rows) whose values are greater
        //     or equal to the DevExpress.XtraEditors.StyleFormatConditionBase.Value1 property
        //     value.
        GreaterOrEqual = 7,
        //
        // Summary:
        //     The style is applied to cells (or corresponding rows) whose values are less
        //     or equal to the DevExpress.XtraEditors.StyleFormatConditionBase.Value1 property
        //     value.
        LessOrEqual = 8
    }

    public class CellCondition
    {
        public CellCondition()
        {
        }

        public CellCondition(CellConditionType type, double val1, double val2)
        {
            m_ConditionType = type;
            m_Value1 = val1;
            m_Value2 = val2;
        }
        public CellCondition(CellConditionType type, double val1, double val2, CellAppearanceObject appearance)
            : this(type, val1, val2)
        {

            m_Appearance = appearance;
        }

        private CellAppearanceObject m_Appearance;
        /// <summary>
        /// Настройки отображения
        /// </summary>
        public CellAppearanceObject Appearance
        {
            get {
                if (m_Appearance == null)
                {
                    m_Appearance = new CellAppearanceObject();
                }
                return m_Appearance;
            }
            set
            {
                m_Appearance = value;
                RaisePropertyChanged();
            }
        }

        private CellConditionType m_ConditionType = CellConditionType.None;
        /// <summary>
        /// Условие
        /// </summary>
        public CellConditionType ConditionType
        {
            get 
            {
                return m_ConditionType;
            }
            set 
            {
                m_ConditionType = value;
                RaisePropertyChanged();
            }
        }

        private double m_Value1 = 0;
        /// <summary>
        /// Значение 1
        /// </summary>
        public double Value1
        {
            get {
                return m_Value1;
            }
            set { 
                m_Value1 = value;
                RaisePropertyChanged();
            }
        }

        private double m_Value2 = 0;
        /// <summary>
        /// Значение 2
        /// </summary>
        public double Value2
        {
            get
            {
                return m_Value2;
            }
            set
            {
                m_Value2 = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Событие - произошло изменение свойств
        /// </summary>
        public event EventHandler PropertyChanged;

        protected void RaisePropertyChanged()
        {
            EventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        void m_Appearance_PropertyChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged();
        }

        public override string ToString()
        {
            String Text = String.Empty;
            String format1 = "{0} {1}";
            String format2 = "{0} {1} and {2}";
            switch (ConditionType)
            {

                case CellConditionType.Equal:
                    Text = String.Format(format1, Localization.ValueFilter_Equal, Value1.ToString());
                    break;
                case CellConditionType.NotEqual:
                    Text = String.Format(format1, Localization.ValueFilter_NotEqual, Value1.ToString());
                    break;
                case CellConditionType.Less:
                    Text = String.Format(format1, Localization.ValueFilter_Less, Value1.ToString());
                    break;
                case CellConditionType.LessOrEqual:
                    Text = String.Format(format1, Localization.ValueFilter_LessOrEqual, Value1.ToString());
                    break;
                case CellConditionType.Greater:
                    Text = String.Format(format1, Localization.ValueFilter_Greater, Value1.ToString());
                    break;
                case CellConditionType.GreaterOrEqual:
                    Text = String.Format(format1, Localization.ValueFilter_GreaterOrEqual, Value1.ToString());
                    break;
                case CellConditionType.Between:
                    Text = String.Format(format2, Localization.ValueFilter_Between, Value1.ToString(), Value2.ToString());
                    break;
                case CellConditionType.NotBetween:
                    Text = String.Format(format2, Localization.ValueFilter_NotBetween, Value1.ToString(), Value2.ToString());
                    break;
                case CellConditionType.None:
                    Text = Localization.ComboBoxItem_None;
                    break;
                default:
                    Text = ConditionType.ToString();
                    break;
            }
            return Text;
        }

        public BitmapImage ToImage()
        {
            BitmapImage Icon = null;
            switch (ConditionType)
            {

                case CellConditionType.Equal:
                    Icon = UriResources.Images.Equal16;
                    break;
                case CellConditionType.NotEqual:
                    Icon = UriResources.Images.NotEqual16;
                    break;
                case CellConditionType.Less:
                    Icon = UriResources.Images.Less16;
                    break;
                case CellConditionType.LessOrEqual:
                    Icon = UriResources.Images.LessOrEqual16;
                    break;
                case CellConditionType.Greater:
                    Icon = UriResources.Images.Greater16;
                    break;
                case CellConditionType.GreaterOrEqual:
                    Icon = UriResources.Images.GreaterOrEqual16;
                    break;
                case CellConditionType.Between:
                    Icon = UriResources.Images.Between16;
                    break;
                case CellConditionType.NotBetween:
                    Icon = UriResources.Images.NotBetween16;
                    break;
            }
            return Icon;
        }

    }
}
