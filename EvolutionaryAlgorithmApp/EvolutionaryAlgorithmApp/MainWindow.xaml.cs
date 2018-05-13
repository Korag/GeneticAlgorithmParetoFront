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

    // niedzialajacy bullshit
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
 
            // 2 populacje musza migac 
            // wykres iteracji --> suma, min f1 , min f2

            // w sprawozdaniu odleglosc pareto frontu od rozwiazania teoretycznego

            // suma po wszystkich punktach w populacji (od i do licznosci pareto frontu) |f(f1) - f2|

            // zrobic zapisa pareto frontu do pliku

            InitializeComponent();

            // Binding
            parameters = new Parameters();
            this.DataContext = parameters;
            DefaultValue();



            // testy 
            parameters.ListOfPoints.Add(new ObservablePoint(r.NextDouble() * 10, r.NextDouble() * 10));

            wykres.EditSeriesCollection(parameters.ListOfPoints);

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
                Thread.Sleep((int)parameters.SleepTime);

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
            // zatrzymujemy w dowolnym momencie (po danej skonczonej iteracji) dzialanie algorytmu
        }

        // tutaj po prostu ustawiamy wszystkie wartosci w parameters i potem sa one wyswietlane na ekranie
        private void DValues_Click(object sender, RoutedEventArgs e)
        {
            DefaultValue();
        }

        private void DefaultValue()
        {
            parameters.F1Formula = "Evol";
            parameters.F2Formula = "Morons";
            parameters.F1LeftConstraint = 1;
            parameters.F1RightConstraint = 1;
            parameters.F2LeftConstraint = 1;
            parameters.F2RightConstraint = 1;
            parameters.Popsize = 1;
            parameters.PlausOfMutation = 1;
            parameters.PlausOfCrossing = 1;
            parameters.Minimum = "Wojrog";
            parameters.SleepTime = 1;
            parameters.IterationLimit = 1;
            parameters.IterationNumber = 0;
        }

        private void SaveToFile_Click(object sender, RoutedEventArgs e)
        {
            // pareto front zostaje zapisany jako obraz png do pliku 
            
            // tworzymy dokument txt o nastepujacej budowie
            // x1, x2, f1, f2, odleglosc (chyba od minimum)
            
        }

        // tutaj po prostu czyscimy wszystkie wartosci z parameters na puste 
        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            this.parameters = null;
            parameters = new Parameters();
            this.DataContext = parameters;
        }
    }
}
