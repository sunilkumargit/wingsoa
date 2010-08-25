/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Wing.UILibrary.Olap
 
    Wing.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Wing.UILibrary.Olap.  If not, see
  	<http://www.gnu.org/licenses/> 
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Wing.UILibrary.Olap under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
*/
using System.Windows.Controls;

namespace Wing.AgOlap.Controls.List
{
    public class RanetListBox : ListBox
    {
        public RanetListBox()
        {
            DefaultStyleKey = typeof(RanetListBox);
        }

        ScrollViewer Scroller = null;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Scroller = base.GetTemplateChild("ScrollViewer") as ScrollViewer;
            if (Scroller != null)
            {
                Wing.AgOlap.Features.ScrollViewerMouseWheelSupport.AddMouseWheelSupport(Scroller, this);
            }
        }


        ~RanetListBox()
        {
            Wing.AgOlap.Features.ScrollViewerMouseWheelSupport.RemoveMouseWheelSupport(this);
        }
    }
}
