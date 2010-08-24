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

namespace Ranet.AgOlap.Controls.Combo
{
    public class ColorItem
    {
        Color m_ColorValue = Colors.Transparent;
        public Color ColorValue
        {
            get { return m_ColorValue; }
            set {
                m_Brush = null;
                m_ColorValue = value; 
            }
        }

        Brush m_Brush = null;
        public Brush ColorBrush
        {
            get
            {
                if (m_Brush == null)
                {
                    m_Brush = new SolidColorBrush(ColorValue);
                }
                return m_Brush;
            }
        }

        String m_ColorName = String.Empty;
        public String ColorName
        {
            get { return m_ColorName; }
            set { m_ColorName = value; }
        }

        public ColorItem(Color colorValue, String colorName)
        {
            ColorValue = colorValue;
            ColorName = colorName;
        }
    }

    public partial class ColorPicker : UserControl
    {
        List<ColorItem> m_List = null;

        public ColorPicker()
        {
            InitializeComponent();

            m_List = new List<ColorItem>();
            m_List.Add(new ColorItem(Colors.Transparent, "<None>"));
            m_List.Add(new ColorItem(Colors.Black, "Black"));
            m_List.Add(new ColorItem(Colors.Blue, "Blue"));
            m_List.Add(new ColorItem(Colors.Brown, "Brown"));
            m_List.Add(new ColorItem(Colors.Cyan, "Cyan"));
            m_List.Add(new ColorItem(Colors.DarkGray, "DarkGray"));
            m_List.Add(new ColorItem(Colors.Gray, "Gray"));
            m_List.Add(new ColorItem(Colors.Green, "Green"));
            m_List.Add(new ColorItem(Colors.Magenta, "Magenta"));
            m_List.Add(new ColorItem(Colors.Orange, "Orange"));
            m_List.Add(new ColorItem(Colors.Purple, "Purple"));
            m_List.Add(new ColorItem(Colors.Red, "Red"));
            m_List.Add(new ColorItem(Colors.White, "White"));
            m_List.Add(new ColorItem(Colors.Yellow, "Yellow"));
            
            //LightBlue #F08080   
            //LightCyan  #E0FFFF   
            //LightGoldenRodYellow  #FAFAD2   
            //LightGrey  #D3D3D3   
            //LightGreen  #90EE90   
            //LightPink  #FFB6C1   
            //LightSalmon  #FFA07A   
            //LightSeaGreen  #20B2AA   
            //LightSkyBlue  #87CEFA   
            //LightSlateGray  #778899   
            //LightSteelBlue  #B0C4DE   
            //LightYellow  #FFFFE0 

            m_List.Add(new ColorItem(Color.FromArgb(0xFF, 0xF0, 0x80, 0x80), "LightBlue"));
            m_List.Add(new ColorItem(Color.FromArgb(0xFF, 0xE0, 0xFF, 0xFF), "LightCyan"));
            m_List.Add(new ColorItem(Color.FromArgb(0xFF, 0xFA, 0xFA, 0xD2), "LightGoldenRodYellow"));
            m_List.Add(new ColorItem(Color.FromArgb(0xFF, 0xD3, 0xD3, 0xD3), "LightGrey"));
            m_List.Add(new ColorItem(Color.FromArgb(0xFF, 0x90, 0xEE, 0x90), "LightGreen"));
            m_List.Add(new ColorItem(Color.FromArgb(0xFF, 0xFF, 0xB6, 0xC1), "LightPink"));
            m_List.Add(new ColorItem(Color.FromArgb(0xFF, 0xFF, 0xA0, 0x7A), "LightSalmon"));
            m_List.Add(new ColorItem(Color.FromArgb(0xFF, 0x20, 0xB2, 0xAA), "LightSeaGreen"));
            m_List.Add(new ColorItem(Color.FromArgb(0xFF, 0x87, 0xCE, 0xFA), "LightSkyBlue"));
            m_List.Add(new ColorItem(Color.FromArgb(0xFF, 0x77, 0x88, 0x99), "LightSlateGray"));
            m_List.Add(new ColorItem(Color.FromArgb(0xFF, 0xB0, 0xC4, 0xDE), "LightSteelBlue"));
            m_List.Add(new ColorItem(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xE0), "LightYellow"));
            
            ColorsComboBox.ItemsSource = m_List;
        }

        public ColorItem CurrentObject
        {
            get { 
                ColorItem item = ColorsComboBox.SelectedItem as ColorItem;
                if(item != null)
                {
                    return item;
                }
                return null;
            }
        }

        public void SelectItem(Color color)
        {
            foreach (ColorItem item in m_List)
            {
                if (item.ColorValue == color)
                {
                    ColorsComboBox.SelectedItem = item;
                    return;
                }
            }

            ColorItem custom = new ColorItem(color, String.Format("({0},{1},{2},{3})", color.A.ToString(), color.R.ToString(), color.G.ToString(), color.B.ToString()));
            m_List.Add(custom);
            ColorsComboBox.SelectedItem = custom;
        }
    }
}
