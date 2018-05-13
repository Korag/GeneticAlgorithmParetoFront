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
        public string Name { get; set; }
        public ChartValues<ObservablePoint> ListOfPoints { get; set; } = new ChartValues<ObservablePoint>();


        public string F1Formula { get; set; }
        public string F2Formula { get; set; }
        public double? F1LeftConstraint { get; set; }
        public double? F1RightConstraint { get; set; }
        public double? F2LeftConstraint { get; set; }
        public double? F2RightConstraint { get; set; }
        public int? Popsize { get; set; }
        public double? PlausOfMutation { get; set; }
        public double? PlausOfCrossing { get; set; }
        public string Minimum { get; set; }
        public int? SleepTime{ get; set; }
        public int? IterationLimit{ get; set; }
        public int IterationNumber { get; set; }

        //  iteracje, X, Y
        public double[][][] Population; // [2][popsize][popsize]

    }
}
