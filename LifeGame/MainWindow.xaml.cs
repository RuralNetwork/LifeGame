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
        double sidePanelWidth;

        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer(System.Windows.Threading.DispatcherPriority.Background);

        public MainWindow()
        {
            InitializeComponent();
            //---------------------Test Space------------------------
            //-------------------------------------------------------
            mainpanel.Height = mainwindow.Height;
            mainpanel.Width = mainwindow.Width - toolbox.Width.Value;
            new GraphicsEngine(this);
            new Simulation();
        }

        private void toggleState_Click(object sender, RoutedEventArgs e)
        {
            Simulation.Instance.IsSaved = false;
            //The function to call the mesagebox, then I'll implement it inside the graphical engine
            /*var prova = new dialoguebox("testo");
            mainpanel.Children.Add(prova);
            //Here to center the box
            Canvas.SetLeft(prova, 50);*/
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

        private void mainwindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!Simulation.Instance.IsSaved)
            {
                Serializer.Save(Simulation.Instance, "lastsim.sim");
                Properties.Settings.Default.lastSim = "lastsim";
                Properties.Settings.Default.Save();
            }

        }

        private void mainwindow_Loaded(object sender, RoutedEventArgs e)
        {
            mainMenu.Width = mainGrid.ActualWidth;
            mainMenu.Height = mainGrid.ActualHeight;
            mainMenu.Margin = new Thickness();
            try
            {
                Serializer.Load(ref Simulation.Instance, Properties.Settings.Default.lastSim + ".sim");
                Simulation.Instance.IsSaved = true;
                Simulation.Instance.InitLoad();
                mainMenu.simulationName.Text = "Simulazione: " + Properties.Settings.Default.lastSim;
            }
            catch (Exception)
            {
            }
            //debug
            timer.Tick += delegate
            {
                if (Simulation.Instance != null)
                {
                    Title = Simulation.Instance.ActualFPS.ToString("0.0");
                }
            };
            timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            timer.Start();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (GraphicsEngine.Instance != null)
            {
                if (speedSlider.Value != 100.0)
                {
                    GraphicsEngine.Instance.FPS = (float)speedSlider.Value;
                    SpeedText.Text = GraphicsEngine.Instance.FPS.ToString("0.0");
                }
                else
                {
                    GraphicsEngine.Instance.FPS = 0;
                    SpeedText.Text = "Max";
                }
            }
        }

        private void goBack_Click(object sender, RoutedEventArgs e)
        {
            if (Simulation.Instance != null && Simulation.Instance.IsRunning)
            {
                toggleState_Click(null, null);
            }
            mainMenu.Visibility = Visibility.Visible;
        }

        private void closeSidePanel_Click(object sender, RoutedEventArgs e)
        {
            mainGrid.ColumnDefinitions[0].Width = new GridLength(0);
        }
    }
}
