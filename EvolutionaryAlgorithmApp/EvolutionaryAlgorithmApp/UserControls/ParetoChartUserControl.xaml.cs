using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace EvolutionaryAlgorithmApp.UserControls
{
    /// <summary>
    /// Interaction logic for CartesianChartUserCtrl.xaml
    /// </summary>
    public partial class ParetoChartUserControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Parameters _parameters = ConnectionHelper.ParametersObject;
        //private double? F1LeftConstraint
        //{
        //    get { return _Parameters.F1LeftConstraint; }
        //    set { _Parameters.F1LeftConstraint = value; }
        //}
        //private double? F1RightConstraint
        //{
        //    get { return _Parameters.F1RightConstraint; }
        //    set { _Parameters.F1RightConstraint = value; }
        //}
        //private double? F2LeftConstraint
        //{
        //    get { return _Parameters.F2LeftConstraint; }
        //    set { _Parameters.F2LeftConstraint = value; }
        //}
        //private double? F2RightConstraint
        //{
        //    get { return _Parameters.F2RightConstraint; }
        //    set { _Parameters.F2RightConstraint = value; }
        //}

        //public new string Name2
        //{
        //    get { return _Parameters.Name; }
        //    set { _Parameters.Name = value; }
        //}

        //public ChartValues<ObservablePoint> PointSeries
        //{
        //    get { return _Parameters.ListOfPoints; }
        //    set { _Parameters.ListOfPoints = value; }
        //}

        private double? F1LeftConstraint;
        private double? F1RightConstraint;
        private double? F2LeftConstraint;
        private double? F2RightConstraint;




        public ChartValues<ObservablePoint> ValuesA { get; set; } = null;
        public ChartValues<ObservablePoint> ValuesB { get; set; } = null;
        //public ChartValues<ObservablePoint> ValuesC { get; set; }

        public ParetoChartUserControl()
        {
            InitializeComponent();
            ReinitializeVariables();
            var r = new Random();
            ValuesA = new ChartValues<ObservablePoint>();
            ValuesB = new ChartValues<ObservablePoint>();

            SeriesCollection = new SeriesCollection();


            YFormatter = value => value.ToString("C");

            //modifying the series collection will animate and update the chart
            SeriesCollection.Add(new LineSeries
            {
                Title = "Series 4",
                Values = new ChartValues<double>(),
                LineSmoothness = 0, //0: straight lines, 1: really smooth lines
            });

            //modifying any series values will also animate and update the chart
            SeriesCollection[0].Values.Add(5d);

            DataContext = this;

        }

        private void ReinitializeVariables()
        {
            //przez to ze strona jest zbindowana do parameters musimy reinicjowac, bo dane z gui sa przesylane tylko do parameters
            F1LeftConstraint = _parameters.F1LeftConstraint;
            F1RightConstraint = _parameters.F1RightConstraint;
            F2LeftConstraint = _parameters.F2LeftConstraint;
            F2RightConstraint = _parameters.F2RightConstraint;

        }

        public void MakeParetoFunctions()
        {
            ReinitializeVariables();
            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Bottom Pareto",
                    Values = CreateBottomPareto(),
                    LineSmoothness = 0
                },
                new LineSeries
                {
                    Title = "Top Pareto",
                    Values = CreateTopPareto(),
                    LineSmoothness = 0
                },
            };
        }


        

        public void EditSeriesCollection(ChartValues<ObservablePoint> NewCollection)
        {
            ValuesA = NewCollection;
        }

        private ChartValues<double> CreateBottomPareto()
        {
            ChartValues<double> tempArray = new ChartValues<double>();
            for (double i = (double)F1LeftConstraint, j  = (double) F2LeftConstraint; i < F1RightConstraint; i+=0.2, j += 0.2)
            {

                tempArray.Add((1 + j) / i);
            }

            return tempArray;
        }

        private ChartValues<double> CreateTopPareto()
        {
            ChartValues<double> tempArray = new ChartValues<double>();
            double x = (double)F1LeftConstraint;
            for (double i = (double)F1LeftConstraint, j = (double)F2LeftConstraint; i < F1RightConstraint; i += 0.2, j += 0.2)
            {
                if (i< F1RightConstraint - F1LeftConstraint)
                {                   
                    tempArray.Add(0);
                }
                else
                {
                    
                    tempArray.Add((1 + j) / x);
                    x += 0.2;
                }
                
            }

            return tempArray;
        }

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }


    }
}
