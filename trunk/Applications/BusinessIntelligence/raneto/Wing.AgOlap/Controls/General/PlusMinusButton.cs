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
using System.Windows.Controls.Primitives;

namespace Ranet.AgOlap.Controls.General
{
    public class PlusMinusButton : ToggleButton
    {
        public PlusMinusButton()
        { 
            base.DefaultStyleKey = typeof(PlusMinusButton);
            base.Checked += new RoutedEventHandler(btnPlusMinus_Checked);
            base.Unchecked += new RoutedEventHandler(btnPlusMinus_Unchecked);

            this.SizeChanged += new SizeChangedEventHandler(PlusMinusButton_SizeChanged);
        }

        Border m_brdrPlus = null;
        Border m_brdrMinus = null;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_brdrPlus = base.GetTemplateChild("plus") as Border;
            m_brdrMinus = base.GetTemplateChild("minus") as Border;
        }

        void PlusMinusButton_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (m_brdrPlus != null)
            {
                double marg = Math.Round(Math.Max(1, this.ActualWidth / 9));
                m_brdrPlus.Width = marg;
                m_brdrPlus.Margin = new Thickness(0, marg, 0, marg);
            }
            if (m_brdrMinus != null)
            {
                double marg = Math.Round(Math.Max(1, this.ActualHeight / 9));
                m_brdrMinus.Height = marg;
                m_brdrMinus.Margin = new Thickness(marg, 0, marg, 0);
            }

            //border.BorderThickness = new Thickness(Math.Round(Math.Max(1, this.ActualHeight / 15)));
            //LayoutRoot.Margin = new Thickness(Math.Round(Math.Max(1, (2 * this.ActualHeight / 15))));

            ////if (this.ActualHeight % 2 != 0)
            //{
            //    LayoutRoot.ColumnDefinitions[1].Width = new GridLength(Math.Round(Math.Max(1, this.ActualHeight / 15)));
            //    LayoutRoot.RowDefinitions[1].Height = new GridLength(Math.Round(Math.Max(1, this.ActualHeight / 15)));
            //    //line1.StrokeThickness = 0.5 * this.ActualHeight / 13;
            //    //line2.StrokeThickness = 0.5 * this.ActualHeight / 13;
            //    border1.BorderThickness = new Thickness(Math.Round(Math.Max(1, this.ActualHeight / 15)));
            //    border2.BorderThickness = new Thickness(Math.Round(Math.Max(1, this.ActualHeight / 15)));
            //}
        }

        public event EventHandler CheckedChanged;
        void Raise_CheckedChanged()
        {
            EventHandler handler = CheckedChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        void btnPlusMinus_Unchecked(object sender, RoutedEventArgs e)
        {
            Raise_CheckedChanged();
        }

        void btnPlusMinus_Checked(object sender, RoutedEventArgs e)
        {
            Raise_CheckedChanged();
        }

        public bool IsExpanded
        {
            get
            {
                if (IsChecked.HasValue)
                    return IsChecked.Value;
                return false;
            }
            set
            {
                IsChecked = new bool?(value);
            }
        }
    }
}

