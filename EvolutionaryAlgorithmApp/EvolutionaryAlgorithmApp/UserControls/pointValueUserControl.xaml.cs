using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NeuralNetworkApp.View.UserControls
{
    /// <summary>
    /// Interaction logic for pointValueUserControl.xaml
    /// </summary>
    public partial class pointValueUserControl : UserControl
    {
        public static readonly DependencyProperty TextProperty1 =
        DependencyProperty.Register("TextValue", typeof(String),
        typeof(pointValueUserControl), new FrameworkPropertyMetadata(string.Empty));

        public String TextValue
        {
            get { return GetValue(TextProperty1).ToString(); }
            set { SetValue(TextProperty1, value); }
        }

        public static readonly DependencyProperty TextProperty2 =
        DependencyProperty.Register("FirstValueText", typeof(String),
        typeof(pointValueUserControl), new FrameworkPropertyMetadata("0"));
        public String FirstValueText
        {
            get { return GetValue(TextProperty2).ToString(); }
            set { SetValue(TextProperty2, value); }

        }

        public static readonly DependencyProperty TextProperty3 =
        DependencyProperty.Register("SecondValueText", typeof(String),
        typeof(pointValueUserControl), new FrameworkPropertyMetadata("0"));
        public String SecondValueText
        {
            get { return GetValue(TextProperty3).ToString(); }
            set { SetValue(TextProperty3, value); }
        }
        public static readonly DependencyProperty TextProperty4 =
       DependencyProperty.Register("ThirdValueText", typeof(String),
       typeof(pointValueUserControl), new FrameworkPropertyMetadata("-1"));
        public String ThirdValueText
        {
            get { return GetValue(TextProperty4).ToString(); }
            set { SetValue(TextProperty4, value); }
        }


        public static readonly RoutedEvent TextChangedEvent =
       EventManager.RegisterRoutedEvent("TextChanged", RoutingStrategy.Bubble,
       typeof(RoutedEventHandler), typeof(pointValueUserControl));

        public event RoutedEventHandler TextChanged
        {
            add { AddHandler(TextChangedEvent, value); }
            remove { RemoveHandler(TextChangedEvent, value); }
        }
        private void OnTextChanged(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(TextChangedEvent));
        }


        public pointValueUserControl()
        {
            InitializeComponent();
        }
       

    }
}
