using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPFStudy.Bind
{
    /// <summary>
    /// BindingEvent.xaml 的交互逻辑
    /// </summary>
    public partial class BindingEvent : Window
    {
        public BindingEvent()
        {
            InitializeComponent();
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            text1Box1.Text = slider.Value.ToString();
            text1Box2.Text = slider.Value.ToString();
            text1Box3.Text = slider.Value.ToString();
        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if((double.TryParse(text1Box1.Text,out double result)))
            {
                slider.Value = result;
            }
        }
    }
}
