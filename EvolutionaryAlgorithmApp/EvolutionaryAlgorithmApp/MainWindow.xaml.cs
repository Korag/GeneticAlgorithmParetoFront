using EvolutionaryAlgorithmApp.UserControls;
using LiveCharts.Defaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace EvolutionaryAlgorithmApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public static class ExtensionMethods
    {
        private static Action EmptyDelegate = delegate () { };


        public static void Refresh(this CartesianChartUserCtrl uiElement)

        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
    }
    public partial class MainWindow : Window
    {
        Parameters parameters;
        Random r = new Random();
        public MainWindow()
        {
            // maja byc fajne punkty na pareto front takie ze mozna na nie kliknac i zeby byly identyfikowalne na dziedzinie
            // np rozne kolory na wykresie dziedziny, żeby było wiadomo jakie punkty są odwzorowane
            // 28 maja oddanie programu !!!!!!!!!! potwierdzone info 
            
            parameters = new Parameters();
            parameters.ListOfPoints.Add(new ObservablePoint(r.NextDouble() * 10, r.NextDouble() * 10));
           
            // 2 populacje musza migac 
            // wykres iteracji --> suma, min f1 , min f2

            // w sprawozdaniu odleglosc pareto frontu od rozwiazania teoretycznego

            // suma po wszystkich punktach w populacji (od i do licznosci pareto frontu) |f(f1) - f2|

            InitializeComponent();
            wykres.EditSeriesCollection(parameters.ListOfPoints);

        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            blabla();

        }

        private void blabla()
        {
            int i = 0;
            while (i < 10)
            {
                Thread.Sleep(500);
                parameters.ListOfPoints.Add(new ObservablePoint(r.NextDouble() * 10, r.NextDouble() * 10));
                wykres.EditSeriesCollection(parameters.ListOfPoints);
                wykres.Refresh();
                i++;
            }
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
