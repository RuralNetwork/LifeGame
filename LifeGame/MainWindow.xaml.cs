///
/// LifeGame
///     ___     ___
///    /   \___/   \___
///    \___/   \___/   \
///    /   \___/   \___/
///    \___/   \___/
///        \___/
///    ___                                         ___     ___             ___     ___
///   /   \                      ___           ___/   \___/   \        ___/   \___/   \___
///   \___/                     /   \         /   \___/   \___/       /   \___/   \___/   \
///   /   \                     \___/         \___/   \___/           \___/   \___/   \___/
///   \___/                     /   \         /   \___                /   \___     ___
///   /   \                     \___/         \___/   \___            \___/   \___/   \
///   \___/                     /   \         /   \___/   \           /   \___/   \___/
///   /   \                     \___/         \___/   \___/           \___/   \___/
///   \___/    ___              /   \         /   \                   /   \___     ___
///   /   \___/   \___          \___/         \___/                   \___/   \___/   \___
///   \___/   \___/   \         /   \         /   \                   /   \___/   \___/   \
///       \___/   \___/         \___/         \___/                   \___/   \___/   \___/
///       
// NOTE: Training fitness function will be, in order:
// Average hungry and thirst levels
// Average health level
// Number of successful reproductions
// (sum of lifespan of offsprings) Facoltative

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Media.Animation;
using System.Runtime.Serialization;

namespace LifeGame
{

    public partial class MainWindow : Window
    {


        FastRandom rand = new FastRandom();

        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer(System.Windows.Threading.DispatcherPriority.Background);

        public MainWindow()
        {
            InitializeComponent();
            Debug.Write("\n******************\nComponents Initialized\n******************\n");
            //---------------------Test Space------------------------
            //-------------------------------------------------------
            mainpanel.Height = mainwindow.Height;
            mainpanel.Width = mainwindow.Width - toolbox.Width.Value;
            new GraphicsEngine(mainpanel);

        }

        private void toggleState_Click(object sender, RoutedEventArgs e)
        {
            simSaved = false;
            //The function to call the mesagebox, then I'll implement it inside the graphical engine
            /*var prova = new dialoguebox("testo");
            mainpanel.Children.Add(prova);
            //Here to center the box
            Canvas.SetLeft(prova, 50);*/
            GraphicsEngine.Instance.editing = false;
            Simulation.Instance.TogglePause();
            //toggling text
            if ((string)toggleState.Content == "Ferma Simulazione")
            {
                toggleState.Content = "Continua Simulazione";
            }
            else
            {
                toggleState.Content = "Ferma Simulazione";
            }
        }

        private void createWorld_Click(object sender, RoutedEventArgs e)
        {
            new Simulation(30, 20);

            for (int i = 0; i < 30; i++)
            {
                int x = rand.Next(Simulation.Instance.GridWidth), y = rand.Next(Simulation.Instance.GridHeight);
                Simulation.Instance.Terrain[x][y].ChangeType(ThingType.Earth, null);
                Simulation.Instance.Terrain[x][y].Apply();

                x = rand.Next(Simulation.Instance.GridWidth);
                y = rand.Next(Simulation.Instance.GridHeight);
                Simulation.Instance.Terrain[x][y].ChangeType(ThingType.Sand, null);
                Simulation.Instance.Terrain[x][y].Apply();
            }
            for (int i = 0; i < 50; i++)
            {
                int x = rand.Next(Simulation.Instance.GridWidth), y = rand.Next(Simulation.Instance.GridHeight);
                Simulation.Instance.Terrain[x][y].ChangeType(ThingType.Water, null);
                Simulation.Instance.Terrain[x][y].Apply();

                x = rand.Next(Simulation.Instance.GridWidth);
                y = rand.Next(Simulation.Instance.GridHeight);
                Simulation.Instance.Terrain[x][y].ChangeType(ThingType.Bush, null);
                Simulation.Instance.Terrain[x][y].Apply();
            }

            SetLayout(1);

            //debug
            timer.Tick += delegate
            {
                if (Simulation.Instance != null)
                {
                    Title = Simulation.Instance.ActualFPS.ToString("0.0");
                }
            };
            timer.Interval = new TimeSpan(0, 0, 0, 0, 5);
            timer.Start();
        }

        private void toggleBrush(object sender, RoutedEventArgs e)
        {
            Button current = e.Source as Button;
            for (int i = 0; i < gridToolbox.Children.Count; i++)
            {
                Button f = gridToolbox.Children[i] as Button;
               //f.Background= GraphicsEngine.Instance.switchGround((ThingType)Enum.Parse(typeof(ThingType), f.Name, true));
                f.Background.Opacity = 0.5;
                f.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
            }
            //current.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(193, 193, 193));
            current.Background.Opacity = 1;
            current.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
            GraphicsEngine.Instance.changeBrush(current.Name);

        }
        private void toggleSpeed(object sender, RoutedEventArgs e)
        {
            Button current = e.Source as Button;
            for (int i = 0; i < gridSpeed.Children.Count; i++)
            {
                UIElement f = gridSpeed.Children[i];
                f.GetType().GetProperty("Background").SetValue(f, new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255)));
            }
            current.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(193, 193, 193));
            toggleSpeed(current.Name);

        }
        public void toggleSpeed(string speed)
        {
            switch (speed)
            {
                case "x1":
                    GraphicsEngine.Instance.FPS = 1;
                    break;
                case "x10":
                    GraphicsEngine.Instance.FPS = 10;
                    break;
                case "Max":
                    GraphicsEngine.Instance.FPS = 0;
                    break;
                default:
                    GraphicsEngine.Instance.FPS = 1;
                    break;
            }
        }

        void SetLayout(int l)
        {
            mainpanel.Visibility = Visibility.Hidden;
            openingTitle.Visibility = Visibility.Hidden;
            loadSimulation.Visibility = Visibility.Hidden;
            loadWorld.Visibility = Visibility.Hidden;
            createTerrain.Visibility = Visibility.Hidden;
            newPopulation.Visibility = Visibility.Hidden;
            loadGenome.Visibility = Visibility.Hidden;
            loadGenome.Visibility = Visibility.Hidden;
            toggleState.Visibility = Visibility.Hidden;
            gridSpeed.Visibility = Visibility.Hidden;
            gridToolbox.Visibility = Visibility.Hidden;
            saveSimulation.Visibility = Visibility.Hidden;
            saveGenome.Visibility = Visibility.Hidden;
            //saveButton.Visibility = Visibility.Hidden;
            //headBack.Visibility = Visibility.Hidden;
            switch (l)
            {
                case 0:
                    openingTitle.Visibility = Visibility.Visible;
                    loadSimulation.Visibility = Visibility.Visible;
                    createTerrain.Visibility = Visibility.Visible;
                    //loadWorldButton.Visibility = Visibility.Visible;
                    break;
                case 1:
                    mainpanel.Width = 910;
                    mainpanel.Height = 700;
                    GraphicsEngine.Instance.editing = true;
                    gridToolbox.Visibility = Visibility.Visible;
                    mainpanel.Visibility = Visibility.Visible;
                    newPopulation.Visibility = Visibility.Visible;
                    loadGenome.Visibility = Visibility.Visible;
                    break;
                case 2:
                    mainpanel.Visibility = Visibility.Visible;
                    toggleState.Visibility = Visibility.Visible;
                    gridSpeed.Visibility = Visibility.Visible;
                    saveSimulation.Visibility = Visibility.Visible;
                    saveGenome.Visibility = Visibility.Visible;
                    //saveButton.Visibility = Visibility.Visible;
                    //headBack.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }

        private void loadGenome_Click(object sender, RoutedEventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Inserire il nome del file del genoma");
            Tuple<HallOfFame, NNGlobalLists> tuple;
            if (Serializer.Load(out tuple, input))
            {
                Simulation.Instance.HallOfFame = tuple.Item1;
                Simulation.Instance.NNLists = tuple.Item2;
                SetLayout(2);
            }
        }

        private void loadWorld_Click(object sender, RoutedEventArgs e)
        {
            SetLayout(1);
            string input = Microsoft.VisualBasic.Interaction.InputBox("Prompt", "Title", "Default", -1, -1);
            if (Simulation.Instance != null)
            {
                SetLayout(1);
            }
        }

        private void goBack(object sender, RoutedEventArgs e)
        {
            Simulation.Instance.UnbindEngine();
            Simulation.Instance = null;
            SetLayout(0);
            GraphicsEngine.Instance.editing = false;
        }

        private void newPopulation_Click(object sender, RoutedEventArgs e)
        {
            toggleState_Click(null, null);
            SetLayout(2);
        }

        private void loadSimulation_Click(object sender, RoutedEventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Inserire il nome del file della simulazione");

            if (Serializer.Load(out Simulation.Instance, input))
            {
                Simulation.Instance.InitLoad();
                SetLayout(1);
            }
        }

        private void saveSimulation_Click(object sender, RoutedEventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Inserire il nome del file della simulazione");
            if (Serializer.Save(Simulation.Instance, input))
            {
                Properties.Settings.Default.lastSim = input;
                Properties.Settings.Default.Save();
                simSaved = true;
            }

        }

        private void saveGenome_Click(object sender, RoutedEventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Inserire il nome del file del genoma");
            Serializer.Save(new Tuple<HallOfFame, NNGlobalLists>(Simulation.Instance.HallOfFame, Simulation.Instance.NNLists), input);

        }

        bool simSaved;

        private void mainwindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!simSaved && Simulation.Instance != null)
            {
                Serializer.Save(Simulation.Instance, "lastsim");
                Properties.Settings.Default.lastSim = "lastsim";
                Properties.Settings.Default.Save();
            }

        }

        private void mainwindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Aprire l'ultima simulazione?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (Serializer.Load(out Simulation.Instance, Properties.Settings.Default.lastSim))
                {
                    Simulation.Instance.InitLoad();
                    SetLayout(1);
                }
            }
        }

        private void buttonToolHover(object sender, MouseEventArgs e)
        {
            Button current = e.Source as Button;
            current.Background = GraphicsEngine.Instance.switchGround((ThingType)Enum.Parse(typeof(ThingType), current.Name, true));
        }
    }
}
