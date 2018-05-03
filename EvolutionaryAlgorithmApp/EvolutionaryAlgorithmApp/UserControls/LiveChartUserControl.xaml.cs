using LiveCharts;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace NeuralNetworkApp.View.UserControls
{
    /// <summary>
    /// Interaction logic for LiveChartUserControl.xaml
    /// </summary>
    public partial class LiveChartUserControl : UserControl
    {
        //to jest dla wag
        private List<double> FirstWeightList = new List<double>();
        private List<double> SecondWeightList = new List<double>();
        private List<double> ThirdWeightList = new List<double>();
        private List<double> IterationsList = new List<double>();

        public static readonly DependencyProperty ChartName =
        DependencyProperty.Register("ChartTitle", typeof(String),
        typeof(LiveChartUserControl), new FrameworkPropertyMetadata(string.Empty));




        public String ChartTitle
        {
            get { return GetValue(ChartName).ToString(); }
            set { SetValue(ChartName, value); }

        }


        public LiveChartUserControl()
        {
            InitializeComponent();
            YFormatter = value => value.ToString();

            //modifying the series collection will animate and update the chart


            //modifying any series values will also animate and update the chart
            
        }

        public void ResetValues()
        {
            
            FirstWeightList = new List<double>();
            SecondWeightList = new List<double>();
            ThirdWeightList = new List<double>();
            IterationsList = new List<double>();
        }

        public void AddToHistory(double[] Weight, int Iteration)
        {
            FirstWeightList.Add(Weight[0]);
            SecondWeightList.Add(Weight[1]);
            ThirdWeightList.Add(Weight[2]);
            IterationsList.Add(Iteration + 1);
        }

        private List<string> ConvertListFromDoubleToString(List<double> ArrayToConvert)
        {
            List<string> ArrayToReturn = new List<string>();
            for (int i = 0; i < ArrayToConvert.Count; i++)
            {
                ArrayToReturn.Add(ArrayToConvert[i].ToString());
            }
            return ArrayToReturn;

        }

        public void MakeChart()
        {
            
            SeriesCollection = MakeChartSeries();

            Labels = ConvertListFromDoubleToString(IterationsList).ToArray();
            DataContext = this;
        }


        private SeriesCollection MakeChartSeries()
        {

            SeriesCollection seriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "W1",
                    Values = FirstWeightList.AsChartValues(),
                    Stroke = Brushes.Green,
                },
                new LineSeries
                {
                    Title = "W2",
                    Values = SecondWeightList.AsChartValues(),
                },
                new LineSeries
                {
                    Title = "W3",
                    Values = ThirdWeightList.AsChartValues(),

                }
            };
            
            return seriesCollection;
        }



        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }
    }
}

