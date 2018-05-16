﻿using EvolutionaryAlgorithmApp.UserControls;
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
            Population = _parameters.Population;
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
            FillRandomValues(ref Population);

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

                //PopulationAfterSelection = Selection(Population);

              . 

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
                
                PopulationAfterCrossing = Crossing(PopulationAfterSelection);

                // mozna tutaj juz wrzucic te osobniki na wykres dziedziny



                // obliczenie minimum

                // finalne utworzenie wykresu dziedziny oraz pareto frontu a takze wykresu wartosci poszczegolnych funkcji

                // jezeli przez 5 iteracji nie ma poprawy minimum zatrzymujemy algorytm

                // musimy obliczyc jeszcze to spierdolone odchylenie
                // suma po wszystkich punktach w populacji (od i do licznosci pareto frontu) |f(f1) - f2|
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

        private void Selection()
        {

        }

        private void Mutation()
        {

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
                StartTabel = JebanieOsobnikow(StartTabel, StartTabel.Length, los1, los2);

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
        double[][] JebanieOsobnikow(double[][] Tab, int IloscSkurwysynow, double Jebnij, double Pierdolnij)
        {
            // kurwa - zmienna pomocnicza 
            int kurwa = 0;
            // tworzy nową tabel z -2 elemetami
            double[][] NewTab = new double[(IloscSkurwysynow - 2)][];
            // leci po wszystkich osobnikach w przyjmowanej tabeli
            for (int i = 0; i < IloscSkurwysynow; i++)
            {
                NewTab[i] = new double[2];
                // sprawdza czy trafiło na osobników skazanych na śmierć 
                if (i != Jebnij || i != Pierdolnij)
                {
                    // mamy tylko 2 współrzędne więc eazy 
                    for (int j = 0; i < 2; i++)
                    {
                        // dodawanie osobników ktrzy przeżyli czystki etniczne 
                        NewTab[kurwa][j] = Tab[i][j];
                    }
                    kurwa++;
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
            worker.RunWorkerAsync();
            
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
