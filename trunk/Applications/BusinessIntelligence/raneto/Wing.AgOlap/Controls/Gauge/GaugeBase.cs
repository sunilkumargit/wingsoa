using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Ranet.AgOlap.Controls.Gauge
{

    public class GaugeBase : ItemsControl
    {

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (base.GetValue(StyleManager.ThemeProperty) != null)
            {
                foreach (DependencyObject obj2 in base.Items)
                {
                    Gauge.SetTheme<RangedControl>(obj2, base.GetValue(StyleManager.ThemeProperty));
                }
            }
        }

        public RangeBase ScaleIndicator
        {
            get; set;
        }

    }
}
