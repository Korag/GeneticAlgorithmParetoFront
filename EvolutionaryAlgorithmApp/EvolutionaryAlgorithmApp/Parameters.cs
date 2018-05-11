using LiveCharts;
using LiveCharts.Defaults;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EvolutionaryAlgorithmApp
{
    public class Parameters : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string Name { get; set; } = "dsfsdf";

        public ChartValues<ObservablePoint> ListOfPoints { get; set; } = new ChartValues<ObservablePoint>();


    }
}
