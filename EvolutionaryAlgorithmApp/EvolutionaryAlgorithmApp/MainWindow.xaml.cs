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
using System.ComponentModel;
using Troschuetz.Random.Distributions.Continuous;
using Troschuetz.Random.Generators;
using Troschuetz.Random;

namespace EvolutionaryAlgorithmApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 




    public partial class MainWindow : Window
    {
        private readonly BackgroundWorker worker = new BackgroundWorker();
        private Parameters _parameters;
        Random r = new Random();

        private int Pop_Size;
        private double F1LeftConstraint;
        private double F1RightConstraint;
        private double F2LeftConstraint;
        private double F2RightConstraint;
        private double Sleeper;



        public MainWindow()
        {

            // maja byc fajne punkty na pareto front takie ze mozna na nie kliknac i zeby byly identyfikowalne na dziedzinie
            // np rozne kolory na wykresie dziedziny, żeby było wiadomo jakie punkty są odwzorowane
            // 28 maja oddanie programu !!!!!!!!!! potwierdzone info 
 
            // 2 populacje musza migac 
            // wykres iteracji --> suma, min f1 , min f2

            // w sprawozdaniu odleglosc pareto frontu od rozwiazania teoretycznego

            // suma po wszystkich punktach w populacji (od i do licznosci pareto frontu) |f(f1) - f2|

            // zrobic zapisa pareto frontu do pliku

            InitializeComponent();


            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;

            // Binding
            _parameters = new Parameters();
            this.DataContext = _parameters;

            ReinitializeVariables();

        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) { if (!worker.IsBusy) Start.IsEnabled = true; }


        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var trandom = new TRandom();
            for (int i = 0; i < Pop_Size; i++)
            {
                Thread.Sleep((int)(Sleeper * 1000));
                _parameters.ListOfPoints.Add(new ObservablePoint(trandom.NextDouble(F1LeftConstraint,F1RightConstraint), trandom.NextDouble(F2LeftConstraint, F2RightConstraint)));
                wykres.EditSeriesCollection(_parameters.ListOfPoints);
            }

        }


        private void ReinitializeVariables()
        {
            //przez to ze strona jest zbindowana do parameters musimy reinicjowac, bo dane z gui sa przesylane tylko do parameters
            Pop_Size = _parameters.Popsize;
            F1LeftConstraint = _parameters.F1LeftConstraint;
            F1RightConstraint = _parameters.F1RightConstraint;
            F2LeftConstraint = _parameters.F2LeftConstraint;
            F2RightConstraint = _parameters.F2RightConstraint;
            Sleeper = _parameters.SleepTime;
            _parameters.ListOfPoints = new LiveCharts.ChartValues<ObservablePoint>();
        }

        // Serce programu wszystko bedzie odbwac sie wlasnie tutaj
        private void EvolutionaryCore()
        {
            // ta metoda bedzie wywolywana po nacisnieciu przycisku start - przerywana jest dopiero
            // w momencie gdy nacisnie sie przycisk stop
            // iteracje maja timer po to aby mozna bylo zauwazyc na interfejsie ksztaltowanie punktow
            // dzieki wykorzystaniu async await workerow interfejs graficzny nie bedzie blokowany

            // teraz ta petla ma byc przerwana kiedy nacisniemy stop
            while (true)
            {
                // czas na obserwacje popsize'a i wykresu wartosci funkcji
                Thread.Sleep(_parameters.SleepTime);

                // wszystkie operacje wykonujemy na tablicy znajdujacej sie w Parameters   public double[][][] Population; // [2][popsize][popsize]


                // operacja selekcji

                // 2 warianty i tutaj trzeba zrobic ze na zasadzie losowej jest wybierana metoda selekcji

                // selekcja turniejowa --> losujemy 2 punkty z populacji i wygrywa lepszy (o mniejszej wartosci), 
                // dzielimy popsize na 2 rowne zbiory i jeden zbior jest porownywany wzgledem f1 a drugi wzgledem f2

                // nie wiem co dalej... 

                // selekcja ruletkowa --> obliczamy fitness, jaki to jest procent z calosci dla danego osobnika, obliczamy dystrybuante,
                // generujemy liczby losowe i szeregujemy okreslajac ktore elementy maja przetrwac

                // mozna tutaj juz wrzucic te osobniki na wykres dziedziny


                // operacja mutacji

                // losujemy ktore punkty zostana poddane mutacji (sprawdzamy wszystkie pod wzgledem prawdopodobienstwa) (prawdopodobienstwo mutacji dla kazdego osobnika)
                // jezeli wylosowano osobnika to losujemy kat oraz dlugosc wektora
                // sprawdzamy czy zmutowany osobnik znajduje sie w dziedzinie
                // jezeli nie wykorzystujemy funkcje kary aby zwiekszyc wartosc osobnika

                // mozna tutaj juz wrzucic te osobniki na wykres dziedziny


                // operacja krzyzowania
                // to jest chyba najtrudniejsza operacja, duzo pierdolenia z przeksztalceniami
                // ogolnie to staramy sie tak skrzyzowac aby np calkowicie odbic jeden punkt 
                // do dyskusji jak to robimy

                // mozna tutaj juz wrzucic te osobniki na wykres dziedziny



                // obliczenie minimum

                // finalne utworzenie wykresu dziedziny oraz pareto frontu a takze wykresu wartosci poszczegolnych funkcji

                // jezeli przez 5 iteracji nie ma poprawy minimum zatrzymujemy algorytm

                // musimy obliczyc jeszcze to spierdolone odchylenie
                // suma po wszystkich punktach w populacji (od i do licznosci pareto frontu) |f(f1) - f2|
            }
        }

        private void Selection()
        {

        }

        private void Mutation()
        {

        }

        private void Crossover()
        {

        }


        private void Start_Click(object sender, RoutedEventArgs e)
        {
            Start.IsEnabled = false;
            ReinitializeVariables();
            worker.RunWorkerAsync();
            
        }





        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            // zatrzymujemy w dowolnym momencie (po danej skonczonej iteracji) dzialanie algorytmu
        }

        private void DValues_Click(object sender, RoutedEventArgs e)
        {
            // tutaj po prostu ustawiamy wszystkie wartosci w parameters i potem sa one wyswietlane na ekranie
        }

        private void SaveToFile_Click(object sender, RoutedEventArgs e)
        {
            // pareto front zostaje zapisany jako obraz png do pliku 
            
            // tworzymy dokument txt o nastepujacej budowie
            // x1, x2, f1, f2, odleglosc (chyba od minimum)
            
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            // tutaj po prostu czyscimy wszystkie wartosci z parameters na puste 
        }
    }
}
