using LiveCharts;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using LiveCharts.Wpf.Charts.Base;
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
    /// Interaction logic for MainChartUserControl.xaml
    /// </summary>
    public partial class MainChartUserControl : UserControl
    {
        public static readonly DependencyProperty ChartName =
        DependencyProperty.Register("MainChartTitle", typeof(String),
        typeof(MainChartUserControl), new FrameworkPropertyMetadata(string.Empty));

        public String MainChartTitle
        {
            get { return GetValue(ChartName).ToString(); }
            set { SetValue(ChartName, value); }

        }

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }

        private List<double> XValues = new List<double>();
        private List<double> YValueList;
            
        public MainChartUserControl()
        {
            InitializeComponent();
            FillXValues();
            YFormatter = value => value.ToString();
        }


        private void FillXValues()
        {
            for (double i = -1; i < 1; i += 0.1)
            {
                XValues.Add(Math.Round(i, 1));
            }
        }

        private List<double> FillYValues(double[] Weight)
        {
            YValueList = new List<double>();
            double YValue = 0;
            if (Weight[1] != 0)
            {
                for (int i = 0; i < XValues.Count; i++)
                {
                    YValue = -(Weight[0] * XValues[i] - Weight[2]) / (Weight[1]);
                    YValueList.Add(YValue);
                }
            }
            else
            {
                FillListWithZeros(YValueList);
            }
            return YValueList;
        }

        private void FillListWithZeros(List<double> TempList)
        {
            for (int i = 0; i < XValues.Count; i++)
            {

                TempList.Add(0);
            }
        }

        private SeriesCollection FillMainChartSeriesWithYValues(int NumberOfWeights, List<double[]> Weights)
        {
            List<double> YValues = null;
            SeriesCollection seriesCollection = new SeriesCollection();
            int FunctionNumber = 0;
            while (FunctionNumber < NumberOfWeights)
            {
                YValues = FillYValues(Weights[FunctionNumber]);


                seriesCollection.Add
                (
                    new LineSeries
                    {
                        Title = $"Function {FunctionNumber + 1}",
                        Values = YValues.AsChartValues(),
                        Stroke = new SolidColorBrush(Chart.Colors[FunctionNumber])

                    }
                );

                FunctionNumber++;
            }

            return seriesCollection;
        }

        public void DrawChart(int NumberOfWeights, List<double[]> Weights)
        {
            SeriesCollection = FillMainChartSeriesWithYValues(NumberOfWeights, Weights);

            Labels = ConvertFromDoubleToString();
            DataContext = this;
        }

        private string[] ConvertFromDoubleToString()
        {
            string[] TempArray = new string[XValues.Count];

            for (int i = 0; i < XValues.Count; i++)
            {
                TempArray[i] = XValues[i].ToString();
            }
            return TempArray;
        }
        
    }
}
