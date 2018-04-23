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

namespace EvolutionaryAlgorithmApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        { 
            // maja byc fajne punkty na pareto front takie ze mozna na nie kliknac i zeby byly identyfikowalne na dziedzinie
            // np rozne kolory na wykresie dziedziny, żeby było wiadomo jakie punkty są odwzorowane
            // 28 maja oddanie programu
            InitializeComponent();
        }
    }
}
