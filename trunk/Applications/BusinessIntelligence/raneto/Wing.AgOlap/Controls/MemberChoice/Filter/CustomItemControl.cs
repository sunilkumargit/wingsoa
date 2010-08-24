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
using Ranet.AgOlap.Controls.General.ItemControls;

namespace Ranet.AgOlap.Controls.MemberChoice.Filter
{
    public enum CustomControlTypes 
    {
        None,
        Delete,
        Clear,
        AddOperation,
        AddOperand
    }

    public class CustomItemEventArgs : EventArgs
    {
        public readonly CustomControlTypes Type = CustomControlTypes.None;

        public CustomItemEventArgs(CustomControlTypes type)
        {
            Type = type;
        }
    }

    public class CustomItemControl : ItemControlBase
    {
        public CustomItemControl(CustomControlTypes type)
        {
            Type = type;
            Text = type.ToString();

            switch (type)
            { 
                case CustomControlTypes.Delete:
                    Icon = UriResources.Images.RemoveGroup16;
                    Text = Localization.Delete;
                    break;
                case CustomControlTypes.Clear:
                    Icon = UriResources.Images.Clear16;
                    Text = Localization.Clear;
                    break;
                case CustomControlTypes.AddOperand:
                    Icon = UriResources.Images.AddOperand16;
                    Text = Localization.Filter_AddOperand;
                    break;
                case CustomControlTypes.AddOperation:
                    Icon = UriResources.Images.AddOperation16;
                    Text = Localization.Filter_AddOperation;
                    break;

            }
            
        }

        public CustomControlTypes Type = CustomControlTypes.None;
    }
}