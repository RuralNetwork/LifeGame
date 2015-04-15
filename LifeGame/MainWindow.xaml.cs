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

namespace LifeGame
{

    public partial class MainWindow : Window
    {
        private GraphicsEngine Engine { get; set; }
        private Simulation Simulation { get; set; }
        FastRandom rand = new FastRandom();

        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer(System.Windows.Threading.DispatcherPriority.Background);

        public MainWindow()
        {
            InitializeComponent();
            Debug.Write("\n******************\nComponents Initialized\n******************\n");
            //---------------------Test Space------------------------
            string a = "ciao";
            var b = a;
            b = "addio";
            var c = new List<Thing>();
            //c.Add(null);
            var d = c.Contains(null);
            // var window = new Window();
            //visualHost prova = new visualHost();
            //window.Content = prova;
            //this.Content = prova;
            //Application.Current.Run(window);
            //Bitmap bitm = LifeGame.Properties.Resources.erba_prova;
            //-------------------------------------------------------
            mainpanel.Height = mainwindow.Height;
            mainpanel.Width = mainwindow.Width - toolbox.Width.Value;
            Engine = new GraphicsEngine(mainpanel);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //The function to call the mesagebox, then I'll implement it inside the graphical engine
            /*var prova = new dialoguebox("testo");
            mainpanel.Children.Add(prova);
            //Here to center the box
            Canvas.SetLeft(prova, 50);*/
            Engine.editing = false;
            gridToolbox.Visibility = Visibility.Hidden;
            gridSpeed.Visibility = Visibility.Visible;
            Simulation.TogglePause();
            //toggling text
            if ((string)startSimulation.Content == "Ferma Simulazione")
            {
                startSimulation.Content = "Continua Simulazione";
            }
            else
            {
                startSimulation.Content = "Ferma Simulazione";
            }

            /*Debug.Write("Button clicked\n");
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    Engine.addCell(new GridPoint(x,y));
                }
            }*/
            //Engine.addCell(0, 0);
            //this.Simulation;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Debug.Write(mainwindow.Height);
        }

        private void createWorld_Click(object sender, RoutedEventArgs e)
        {
            mainpanel.Height = mainwindow.Height - (startSimulation.Height + 20);
            mainpanel.Width = mainwindow.Width - toolbox.Width.Value;
            Engine.canvasHeight = mainpanel.Height - (startSimulation.Height + 20);
            Engine.canvasWidth = mainpanel.Width - toolbox.Width.Value;
            Engine.editing = true;
            Simulation = new Simulation(31, 13, Engine);

            for (int i = 0; i < 50; i++)
            {
                int x = rand.Next(Simulation.GridWidth), y = rand.Next(Simulation.GridHeight);
                Simulation.Terrain[x][y].ChangeType(ThingType.Earth, null);
                Simulation.Terrain[x][y].Apply();

                x = rand.Next(Simulation.GridWidth);
                y = rand.Next(Simulation.GridHeight);
                Simulation.Terrain[x][y].ChangeType(ThingType.Sand, null);
                Simulation.Terrain[x][y].Apply();
            }
            for (int i = 0; i < 100; i++)
            {
                int x = rand.Next(Simulation.GridWidth), y = rand.Next(Simulation.GridHeight);
                Simulation.Terrain[x][y].ChangeType(ThingType.Water, null);
                Simulation.Terrain[x][y].Apply();

                x = rand.Next(Simulation.GridWidth);
                y = rand.Next(Simulation.GridHeight);
                Simulation.Terrain[x][y].ChangeType(ThingType.Bush, null);
                Simulation.Terrain[x][y].Apply();
            }


            opening_title.Visibility = Visibility.Hidden;
            createWorld.Visibility = Visibility.Hidden;
            gridToolbox.Visibility = Visibility.Visible;
            startSimulation.IsEnabled = true;


            //debug
            timer.Tick += delegate { Title = Simulation.ActualFPS.ToString("0.0"); };
            timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            timer.Start();
        }

        private void toggleBrush(object sender, RoutedEventArgs e)
        {
            Button current = e.Source as Button;
            for (int i = 0; i < gridToolbox.Children.Count; i++)
            {
                UIElement f = gridToolbox.Children[i];
                f.GetType().GetProperty("Background").SetValue(f, new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255)));
            }
            current.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(193, 193, 193));
            Engine.changeBrush(current.Name);

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
            Engine.toggleSpeed(current.Name);

        }
    }
}
