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
using System.Windows.Threading;

namespace LifeGame
{

    public partial class MainWindow : Window
    {
        DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Background);

        Stopwatch idleWatch = Stopwatch.StartNew();
        Stopwatch simWatch = Stopwatch.StartNew();

        bool isRunning;

        public MainWindow()
        {
            InitializeComponent();
            //---------------------Test Space------------------------
            //-------------------------------------------------------
            new GraphicsEngine(this);
            new Simulation();
            Grid.SetRowSpan(mainMenu, 2);//this continues to revert to 0 in the designer

            InputManager.Current.PreProcessInput += (sender, e) =>
            {
                if (e.StagingItem.Input is MouseButtonEventArgs)
                {
                    idleWatch.Restart();
                }
            };

            timer = new DispatcherTimer(DispatcherPriority.Background);
            timer.Tick += (sender, e) =>
            {
                if (isRunning && Simulation.Instance != null && (GraphicsEngine.Instance.FPS == 0 || simWatch.Elapsed.TotalSeconds > 1 / GraphicsEngine.Instance.FPS))
                {
                    Title = (1 / simWatch.Elapsed.TotalSeconds).ToString("0.0");
                    simWatch.Restart();
                    Simulation.Instance.Update();
                    GraphicsEngine.Instance.Update();
                }

                if (idleWatch.Elapsed.TotalMinutes > 1.0)
                {
                    goBack_Click(null, null);
                    mainMenu.startButton_Click(null, null);
                    toggleState_Click(null, null);
                    speedSlider.Value = 5;
                    idleWatch.Reset();
                }
            };
            timer.Start();

        }

        private void toggleState_Click(object sender, RoutedEventArgs e)
        {
            GraphicsEngine.Instance.IsSaved = false;
            //The function to call the mesagebox, then I'll implement it inside the graphical engine
            /*var prova = new dialoguebox("testo");
            mainpanel.Children.Add(prova);
            //Here to center the box
            Canvas.SetLeft(prova, 50);*/
            isRunning = !isRunning;
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
            if (!GraphicsEngine.Instance.IsSaved)
            {
                Serializer.Save(Simulation.Instance, "lastsim.sim");
                Properties.Settings.Default.lastSim = "lastsim";
                Properties.Settings.Default.Save();
            }

        }

        private void mainwindow_Loaded(object sender, RoutedEventArgs e)
        {
            mainpanel.Width = GraphicsEngine.GRID_WIDTH * 30;
            mainpanel.Height = GraphicsEngine.GRID_HEIGHT * 34 + 17;
            infoPanel.Visibility = Visibility.Hidden;
            infoPanel.Margin = new Thickness();

            mainMenu.Width = mainGrid.ActualWidth;
            mainMenu.Height = mainGrid.ActualHeight;
            mainMenu.Margin = new Thickness();
            try
            {
                Serializer.Load(ref Simulation.Instance, Properties.Settings.Default.lastSim + ".sim");
                GraphicsEngine.Instance.IsSaved = true;
                Simulation.Instance.InitLoad();
                mainMenu.simulationName.Text = "Simulazione: " + Properties.Settings.Default.lastSim;
            }
            catch (Exception)
            {
            }
            //debug
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
            if (Simulation.Instance != null && isRunning)
            {
                toggleState_Click(null, null);
            }
            mainMenu.Visibility = Visibility.Visible;
        }

        private void closeSidePanel_Click(object sender, RoutedEventArgs e)
        {
            mainGrid.ColumnDefinitions[0].Width = new GridLength(0);
        }

        public void ViewReposition()
        {
            mainpanel.Margin = new Thickness((parentView.ActualWidth - mainpanel.ActualWidth) / 2, (parentView.ActualHeight - mainpanel.ActualHeight) / 2,
                                            (parentView.ActualWidth - mainpanel.ActualWidth) / 2, (parentView.ActualHeight - mainpanel.ActualHeight) / 2);
        }

        private void mainwindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }
    }
}
