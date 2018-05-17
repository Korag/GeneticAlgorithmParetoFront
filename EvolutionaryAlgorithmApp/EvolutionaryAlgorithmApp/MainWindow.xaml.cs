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
        TRandom trandom = new TRandom();

        private int Pop_Size;
        private double F1LeftConstraint;
        private double F1RightConstraint;
        private double F2LeftConstraint;
        private double F2RightConstraint;
        private double Sleeper;
        private double[][] Population;
        private double[][] PopulationAfterSelection;
        private double[][] PopulationAfterCrossing;
        private double MinF1;
        private double MinF2;
        private string Minimum;



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
            _parameters = ConnectionHelper.ParametersObject;
            this.DataContext = _parameters;
            DefaultValue();
            
            ReinitializeVariables();

        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) { if (!worker.IsBusy) Start.IsEnabled = true; }


        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i < Pop_Size; i++)
            {
                Thread.Sleep((int)(Sleeper * 100));
                _parameters.ListOfPoints.Add(new ObservablePoint(trandom.NextDouble(F1LeftConstraint, F1RightConstraint), 
                                                                trandom.NextDouble(F2LeftConstraint, F2RightConstraint)
                                                                ));
                wykres.EditSeriesCollection(_parameters.ListOfPoints);
            }

            int x = 0;

            while (x<5)
            {
                // czas na obserwacje popsize'a i wykresu wartosci funkcji

                // wszystkie operacje wykonujemy na tablicy znajdujacej sie w Parameters   public double[][][] Population; // [2][popsize][popsize]

                FillRandomValues(ref Population);

                // operacja selekcji

                // 2 warianty i tutaj trzeba zrobic ze na zasadzie losowej jest wybierana metoda selekcji

                // selekcja turniejowa --> losujemy 2 punkty z populacji i wygrywa lepszy (o mniejszej wartosci), 
                // dzielimy popsize na 2 rowne zbiory i jeden zbior jest porownywany wzgledem f1 a drugi wzgledem f2

                Selection(Population, ref PopulationAfterSelection);



                // selekcja ruletkowa --> obliczamy fitness, jaki to jest procent z calosci dla danego osobnika, obliczamy dystrybuante,
                // generujemy liczby losowe i szeregujemy okreslajac ktore elementy maja przetrwac

                // mozna tutaj juz wrzucic te osobniki na wykres dziedziny


                // operacja mutacji

                Mutation(ref PopulationAfterCrossing);
                // losujemy ktore punkty zostana poddane mutacji (sprawdzamy wszystkie pod wzgledem prawdopodobienstwa) (prawdopodobienstwo mutacji dla kazdego osobnika)
                // jezeli wylosowano osobnika to losujemy kat oraz dlugosc wektora
                // sprawdzamy czy zmutowany osobnik znajduje sie w dziedzinie
                // jezeli nie wykorzystujemy funkcje kary aby zwiekszyc wartosc osobnika

                // mozna tutaj juz wrzucic te osobniki na wykres dziedziny


                // operacja krzyzowania
                // to jest chyba najtrudniejsza operacja, duzo pierdolenia z przeksztalceniami
                // ogolnie to staramy sie tak skrzyzowac aby np calkowicie odbic jeden punkt 
                // do dyskusji jak to robimy


                PopulationAfterCrossing = Crossing(PopulationAfterSelection);

                // mozna tutaj juz wrzucic te osobniki na wykres dziedziny

                _parameters.RewriteThePoints(PopulationAfterCrossing);
                wykres.EditSeriesCollection(_parameters.ListOfPoints);

                // obliczenie minimum
                SearchForMinValue(PopulationAfterCrossing, ref MinF1, ref MinF2);

                Minimum = $"{{{MinF1}.{MinF2}}}";


                Array.Clear(Population, 0, Population.Length);
                Array.Copy(PopulationAfterCrossing, Population, PopulationAfterCrossing.Length);

                // finalne utworzenie wykresu dziedziny oraz pareto frontu a takze wykresu wartosci poszczegolnych funkcji

                // jezeli przez 5 iteracji nie ma poprawy minimum zatrzymujemy algorytm

                // musimy obliczyc jeszcze to spierdolone odchylenie
                // suma po wszystkich punktach w populacji (od i do licznosci pareto frontu) |f(f1) - f2|
                Thread.Sleep(200);
                x++;
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
            Population = _parameters.Population;
            PopulationAfterSelection = _parameters.PopulationAfterSelection;
            PopulationAfterCrossing = _parameters.PopulationAfterCrossing;
            MinF1 = _parameters.MinF1;
            MinF2 = _parameters.MinF2;
            Minimum = _parameters.Minimum;
            _parameters.ListOfPoints = new LiveCharts.ChartValues<ObservablePoint>();
        }

        // Serce programu wszystko bedzie odbwac sie wlasnie tutaj
        private void EvolutionaryCore()
        {
            // ta metoda bedzie wywolywana po nacisnieciu przycisku start - przerywana jest dopiero
            // w momencie gdy nacisnie sie przycisk stop
            // iteracje maja timer po to aby mozna bylo zauwazyc na interfejsie ksztaltowanie punktow
            // dzieki wykorzystaniu async await workerow interfejs graficzny nie bedzie blokowany
            InitializePopulation(ref Population, Pop_Size);
            InitializePopulation(ref PopulationAfterSelection, Pop_Size / 2);
            InitializePopulation(ref PopulationAfterCrossing, Pop_Size / 4);

          

            // teraz ta petla ma byc przerwana kiedy nacisniemy stop
            while (true)
            {
                // czas na obserwacje popsize'a i wykresu wartosci funkcji

                // wszystkie operacje wykonujemy na tablicy znajdujacej sie w Parameters   public double[][][] Population; // [2][popsize][popsize]

                FillRandomValues(ref Population);

                // operacja selekcji

                // 2 warianty i tutaj trzeba zrobic ze na zasadzie losowej jest wybierana metoda selekcji

                // selekcja turniejowa --> losujemy 2 punkty z populacji i wygrywa lepszy (o mniejszej wartosci), 
                // dzielimy popsize na 2 rowne zbiory i jeden zbior jest porownywany wzgledem f1 a drugi wzgledem f2

                Selection(Population, ref PopulationAfterSelection);



                // selekcja ruletkowa --> obliczamy fitness, jaki to jest procent z calosci dla danego osobnika, obliczamy dystrybuante,
                // generujemy liczby losowe i szeregujemy okreslajac ktore elementy maja przetrwac

                // mozna tutaj juz wrzucic te osobniki na wykres dziedziny


                // operacja mutacji

                Mutation(ref PopulationAfterCrossing);
                // losujemy ktore punkty zostana poddane mutacji (sprawdzamy wszystkie pod wzgledem prawdopodobienstwa) (prawdopodobienstwo mutacji dla kazdego osobnika)
                // jezeli wylosowano osobnika to losujemy kat oraz dlugosc wektora
                // sprawdzamy czy zmutowany osobnik znajduje sie w dziedzinie
                // jezeli nie wykorzystujemy funkcje kary aby zwiekszyc wartosc osobnika

                // mozna tutaj juz wrzucic te osobniki na wykres dziedziny


                // operacja krzyzowania
                // to jest chyba najtrudniejsza operacja, duzo pierdolenia z przeksztalceniami
                // ogolnie to staramy sie tak skrzyzowac aby np calkowicie odbic jeden punkt 
                // do dyskusji jak to robimy

                PopulationAfterCrossing = Crossing(PopulationAfterSelection);

                // mozna tutaj juz wrzucic te osobniki na wykres dziedziny
                


                // obliczenie minimum
                SearchForMinValue(PopulationAfterCrossing, ref MinF1, ref MinF2);

                Minimum = $"{{{MinF1}.{MinF2}}}";


                Array.Clear(Population, 0, Population.Length);
                Array.Copy(PopulationAfterCrossing, Population, PopulationAfterCrossing.Length);

                // finalne utworzenie wykresu dziedziny oraz pareto frontu a takze wykresu wartosci poszczegolnych funkcji

                // jezeli przez 5 iteracji nie ma poprawy minimum zatrzymujemy algorytm

                // musimy obliczyc jeszcze to spierdolone odchylenie
                // suma po wszystkich punktach w populacji (od i do licznosci pareto frontu) |f(f1) - f2|
            }
        }

        private void SearchForMinValue(double[][] PopulationAfterCrossing, ref double MinF1, ref double MinF2)
        {
            MinF1 = PopulationAfterCrossing[0][0];
            MinF2 = PopulationAfterCrossing[0][1];

            for (int i = 0; i < PopulationAfterCrossing.Length; i++)
            {
                if (PopulationAfterCrossing[i][0] < MinF1)
                {
                    MinF1 = PopulationAfterCrossing[i][0];
                }
                if (PopulationAfterCrossing[i][1] < MinF2)
                {
                    MinF2 = PopulationAfterCrossing[i][1];
                }
            }
        }

        private void FillRandomValues(ref double[][] Population)
        {
            for (int i = 0; i < Population.Length; i++)
            {

                    for (int j = 0; j < Population[i].Length; j++)
                    {
                        if (Population[i][0] == 0 && Population[i][1] == 0 && j == 0)
                        {
                            Population[i][j] = trandom.NextDouble(F1LeftConstraint, F1RightConstraint);
                        }
                        else if (Population[i][0] == 0 && Population[i][1] == 0 && j == 1)
                        {
                            Population[i][j] = trandom.NextDouble(F2LeftConstraint, F2RightConstraint);
                        }
                    }




                //try
                //{
                //    for (int j = 0; j < Population[i].Length; j++)
                //    {
                //        if (Population[i][0] == 0 && Population[i][1] == 0 && j == 0)
                //        {
                //            Population[i][j] = trandom.NextDouble(F1LeftConstraint, F1RightConstraint);
                //        }
                //        else if (Population[i][0] == 0 && Population[i][1] == 0 && j == 1)
                //        {
                //            Population[i][j] = trandom.NextDouble(F2LeftConstraint, F2RightConstraint);
                //        }
                //    }
                //}
                //catch (Exception)
                //{

                //    Population[i] = new double[2];
                //    Population[i][0] = trandom.NextDouble(F1LeftConstraint, F1RightConstraint);
                //    Population[i][1] = trandom.NextDouble(F1LeftConstraint, F1RightConstraint);

                //}
            }
        }

        private void InitializePopulation(ref double[][] Population, int popsize)
        {
            Population = new double[popsize][];
            for (int i = 0; i < Population.Length; i++)
            {
                Population[i] = new double[2];
            }
        }

        private void Selection(double[][] Population, ref double[][] PopulationAfterSelection)
        {
            HashSet<int> numbers1 = new HashSet<int>();
            HashSet<int> numbers2 = new HashSet<int>();

            for (int i = 0; i < Pop_Size/4; i++)
            {

                int trandom1 = (int)(trandom.NextUInt(0, (uint)Pop_Size / 2));
                int trandom2 = (int)(trandom.NextUInt(0, (uint)Pop_Size / 2));

                while (!numbers1.Contains(trandom1))
                {
                    trandom1 = (int)(trandom.NextUInt(0, (uint)Pop_Size / 2));
                    numbers1.Add(trandom1);
                }

                while (!numbers2.Contains(trandom2))
                {
                    trandom2 = (int)(trandom.NextUInt(0, (uint)Pop_Size / 2));
                    numbers2.Add(trandom2);
                }

                if (Population[trandom1][0] < Population[trandom2][0])
                {
                    PopulationAfterSelection[i][0] = Population[trandom1][0];
                    PopulationAfterSelection[i][1] = Population[trandom1][1];
                }
                else
                {
                    PopulationAfterSelection[i][0] = Population[trandom2][0];
                    PopulationAfterSelection[i][1] = Population[trandom2][1];
                }
               
            }
            for (int i = Pop_Size / 4; i < Pop_Size / 2; i++)
            {
                int trandom1 = (int)(trandom.NextUInt(0, (uint)Pop_Size / 2));
                int trandom2 = (int)(trandom.NextUInt(0, (uint)Pop_Size / 2));

                while (!numbers1.Contains(trandom1))
                {
                    trandom1 = (int)(trandom.NextUInt(0, (uint)Pop_Size / 2));
                    numbers1.Add(trandom1);
                }
               

                while (!numbers2.Contains(trandom2))
                {
                    trandom2 = (int)(trandom.NextUInt(0, (uint)Pop_Size / 2));
                    numbers2.Add(trandom2);
                }
               

                if (Population[trandom1][1] < Population[trandom2][1])
                {
                    PopulationAfterSelection[i][0] = Population[trandom1][0];
                    PopulationAfterSelection[i][1] = Population[trandom1][1];
                }
                else
                {
                    PopulationAfterSelection[i][0] = Population[trandom2][0];
                    PopulationAfterSelection[i][1] = Population[trandom2][1];
                }
            }
        }

        private void Mutation(ref double[][] tempTab)
        {
            double Angle = 0;
            double Radius = 0;
            for (int i = 0; i < tempTab.Length; i++)
            {
                //jesli wylosowana liczba jest mniejsza niz prawdopodobienstwo mutacji to mutujemy(losujemy liczbe z 
                //przedziału (0,1) wiec dla prawdopodobienstwa mutacji = 1 mutacja występuje za każdym razem)
                if (trandom.NextDouble(0,1)<_parameters.PlausOfMutation)
                {
                    Angle = trandom.NextDouble(0, 2 * Math.PI);
                    Radius = trandom.NextDouble(0, 1);
                    tempTab[i][0] = Math.Cos(Angle) * Radius + tempTab[i][0];
                    tempTab[i][1] = Math.Sin(Angle) * Radius + tempTab[i][1];

                    if (CheckPointsDomain(tempTab[i][0], tempTab[i][1]))
                    {
                        tempTab[i][1] += 10;
                    }


                    //sprawdzenie czy wychodzi z dziedziny jak tak to albo losujemy od nowa r i kąt albo dzielimy je przez 2 albo funkcja kary
                    //while (CheckPointsDomain(tempTab[i][0], tempTab[i][1]))
                    //{
                    //    //powtórne losowanie
                    //    //Angle = trandom.NextDouble(0, 2 * Math.PI);
                    //    //Radius = trandom.NextDouble(0, 1);

                    //    //dzielenie przez dwa
                    //    Angle /= 2;
                    //    Radius /= 2;
                    //    tempTab[i][0] = Math.Cos(Angle) * Radius + tempTab[i][0];
                    //    tempTab[i][1] = Math.Sin(Angle) * Radius + tempTab[i][1];
                    //}
                }
            }
            //nie wiem czy ma zwracac
           // return tempTab;
        }
        private bool CheckPointsDomain(double x1, double x2)
        {
            bool Result = false;

            if (x1<F1LeftConstraint || x1>F1RightConstraint)
            {
                Result = true;
            }
            if (x2 < F2LeftConstraint || x2 > F2RightConstraint)
            {
                Result = true;
            }
            return Result;
        }

        #region Crossover
       // Funkcja do krzyżowania: Tabel - tabela używana do krzyżowania, STSize rozmiar tabeli(tak naprawdę nie potrzebny można użyć .Length)
       double[][] Crossing(double[][] Tabel)
        {
            uint STSize = Convert.ToUInt32(Tabel.Length);
            // tabela pomocnicza 
            double[][] StartTabel = new double[Tabel.Length][];
            Array.Copy(Tabel, StartTabel, Tabel.Length);
            // tabela do której będą wpisywane punkty stworzone na podstawie krzyżowania(fuck nie jestem pewnien co się stanie jak będzie nieparzysta liczba osobników)
            double[][] EndTabel = new double[STSize / 2][];
            // pętla dla całej tabeli EndTable
            for (int i = 0; i < (STSize / 2); i++)
            {
                // Losowanie pierwszego osobnika
                uint los1 = trandom.NextUInt(0, Convert.ToUInt32(StartTabel.Length) - 1);
                Bieda:
                //Losowanie drugiego osobnika
                uint los2 = trandom.NextUInt(0, Convert.ToUInt32(StartTabel.Length) - 1);
                //Sprawdzanie czy nie wylosowało jakimś cudem tych samych osobników, nie jestem pewnie czy nie będzie problemów z goto
                if (los1 == los2)
                {
                    goto Bieda;
                }
                // tabela pomocnicza jednowymiarowa z wyliczonymi współrzędnymi nowego osobnika
                double[] tab = CreatePointUsingLines(StartTabel[los1][0], StartTabel[los1][1], StartTabel[los2][0], StartTabel[los2][1], 0, 100, 0, 100);
                // usuwanie wykorzystanych osobników
                StartTabel = WyrzucanieOsobnikow(StartTabel, StartTabel.Length, los1, los2);

                EndTabel[i] = new double[2];
                EndTabel[i][0] = tab[0];
                EndTabel[i][1] = tab[1];

            }

            return EndTabel;
        }


        //Funkcja do tworzenia dziecka z 2 osobników: xa - pierwsza współrzędna z pierwszego osobnika,xb - druga współrzędna z pierwszego osobnika,ya - pierwsza współrzędna z drugiego osobnika,yb - druga współrzędna z drugiego osobnika
        //BeginOdlegloscOdProstej i EndOdlegloscOdProstej - ustalanie przedziału w jakim ma być losowana odległość prostej równoległej od prostej przechodzącej przez dwa punkty,BeginWartoscX i EndWartoscX - musi być jakiś przedział dla wylosowania x-owej punktu na równoległej 
        double[] CreatePointUsingLines(double xa, double xb, double ya, double yb, double BeginOdlegloscOdProstej, double EndOdlegloscOdProstej, double BeginWartoscX, double EndWartoscX)
        {
            double[] tab = new double[2];
            tab[0] = trandom.NextDouble(BeginWartoscX, EndWartoscX);
            double a = trandom.NextDouble(BeginOdlegloscOdProstej, EndOdlegloscOdProstej);

            // Piękna funkcja na prostą przechodzącą przez dwa punkty, tab[0] to losowy x dla tworzonego punktu, tab[1] to y nowego punktu liczony na podstawie wzoru
            tab[1] = (((ya - yb) / (xa - xb)) * tab[0]) + ((ya - ((ya - yb) / (ya - xb))) * xa);

            return tab;
        }


        //Funkcja do usuwania elementu z tablicy: Tab - Tablica z której usuwa, IloscSkurwysynow - ilość elemetnów w tablicy z której usuwamy, Jebnij i Pierdolnij - 2 elemetny z tablicy które usuwamy
        double[][] WyrzucanieOsobnikow(double[][] Tab, int IloscOsobnikow, double Wyrzutek1, double Wyrzutek2)
        {
            // kurwa - zmienna pomocnicza 
            int counter = 0;
            // tworzy nową tabel z -2 elemetami
            double[][] NewTab = new double[IloscOsobnikow][];
            // leci po wszystkich osobnikach w przyjmowanej tabeli
            for (int i = 0; i < IloscOsobnikow; i++)
            {
                NewTab[i] = new double[2];
                // sprawdza czy trafiło na osobników skazanych na śmierć 
                if (i != Wyrzutek1 || i != Wyrzutek2)
                {
                    // mamy tylko 2 współrzędne więc eazy 
                    for (int j = 0; j < 2; j++)
                    {
                        // dodawanie osobników ktrzy przeżyli czystki etniczne 
                        NewTab[counter][j] = Tab[i][j];
                    }
                    counter++;
                }
            }

            return NewTab;
        }
        #endregion


        private void Start_Click(object sender, RoutedEventArgs e)
        {
            Start.IsEnabled = false;
            ParetoChart.MakeParetoFunctions();
            ReinitializeVariables();
            InitializePopulation(ref Population, Pop_Size);
            InitializePopulation(ref PopulationAfterSelection, Pop_Size / 2);
            InitializePopulation(ref PopulationAfterCrossing, Pop_Size / 4);
            worker.RunWorkerAsync();
            //EvolutionaryCore();
        }


        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            // zatrzymujemy w dowolnym momencie (po danej skonczonej iteracji) dzialanie algorytmu
        }


        // tutaj po prostu ustawiamy wszystkie wartosci w parameters i potem sa one wyswietlane na ekranie

        #region Stary reset

        //private void DValues_Click(object sender, RoutedEventArgs e)
        //{
        //   DefaultValue();
        //}

        //private void Reset_Click(object sender, RoutedEventArgs e)
        //{
        //    this._parameters = null;
        //    ConnectionHelper.ParametersObject = new Parameters();
        //    _parameters = ConnectionHelper.ParametersObject;
        //    this.DataContext = _parameters;
        //}

        #endregion

        private void DefaultValue()
        {
            _parameters.F1Formula = "x1";
            _parameters.F2Formula = "(1+x2)/x1";
            _parameters.F1LeftConstraint = 0.1;
            _parameters.F1RightConstraint = 1;
            _parameters.F2LeftConstraint = 0;
            _parameters.F2RightConstraint = 5;
            _parameters.Popsize = 200;
            _parameters.PlausOfMutation = 0.2;
            _parameters.PlausOfCrossing = 0.6;
            _parameters.Minimum = "EV MORONS";
            _parameters.SleepTime = 1;
            _parameters.IterationLimit = 200;
            _parameters.IterationNumber = 0;
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
            DefaultValue();
        }
    }
}
