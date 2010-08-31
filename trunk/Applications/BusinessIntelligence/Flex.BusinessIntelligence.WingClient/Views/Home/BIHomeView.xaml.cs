using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Flex.BusinessIntelligence.WingClient.Views.Home
{
    public partial class BIHomeView : UserControl, IBIHomeView
    {
        public BIHomeView()
        {
            InitializeComponent();
            Button_Click(null, null);
        }

        List<Row1> _row1 = new List<Row1>();
        List<Row2> _row2 = new List<Row2>();


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_row1.Count == 0)
            {
                _row1.Add(new Row1() { Vendedor = "Eliel", UF = "SP", Valor = 110 });
                _row1.Add(new Row1() { Vendedor = "Eliel", UF = "RJ", Valor = 85 });
                _row1.Add(new Row1() { Vendedor = "Eliel", UF = "MG", Valor = 76 });
                _row1.Add(new Row1() { Vendedor = "Eliel", UF = "ES", Valor = 50 });
                _row1.Add(new Row1() { Vendedor = "Eliel", UF = "RS", Valor = 100 });

                _row1.Add(new Row1() { Vendedor = "Marcelo", UF = "SP", Valor = 100 });
                _row1.Add(new Row1() { Vendedor = "Marcelo", UF = "RJ", Valor = 80 });
                _row1.Add(new Row1() { Vendedor = "Marcelo", UF = "MG", Valor = 85 });
                _row1.Add(new Row1() { Vendedor = "Marcelo", UF = "ES", Valor = 40 });
                _row1.Add(new Row1() { Vendedor = "Marcelo", UF = "RS", Valor = 90 });

                _row2.Add(new Row2() { Vendedor = "Eliel", SP = 110, RJ = 85, MG = 76, ES = 50, RS = 100 });
                _row2.Add(new Row2() { Vendedor = "Marcelo", SP = 100, RJ = 80, MG = 85, ES = 40, RS = 90 });
            }
            if (dataGrid1.ItemsSource == _row1)
                dataGrid1.ItemsSource = _row2;
            else
                dataGrid1.ItemsSource = _row1;
        }

    }


    public class Row1
    {
        public String Vendedor { get; set; }
        public String UF { get; set; }
        public Decimal Valor { get; set; }
    }

    public class Row2
    {
        public String Vendedor { get; set; }
        public Decimal SP { get; set; }
        public Decimal RJ { get; set; }
        public Decimal MG { get; set; }
        public Decimal ES { get; set; }
        public Decimal RS { get; set; }
    }

}
