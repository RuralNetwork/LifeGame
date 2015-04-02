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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Media.Animation;

namespace LifeGame
{

    public partial class MainWindow : Window
    {
        private GraphicsEngine Engine { get; set; }
        private Simulation Simulation { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Debug.Write("\n******************\nComponents Initialized\n******************\n");
            //---------------------Test Space------------------------
            var a = (float)Math.Atan2(-2f, -1000000000f);
            var b = (a > 0 ? a : a + (float)Math.PI);
            // var window = new Window();
            //visualHost prova = new visualHost();
            //window.Content = prova;
            //this.Content = prova;
            //Application.Current.Run(window);

            //-------------------------------------------------------
            mainpanel.Height = mainwindow.Height;
            mainpanel.Width = mainwindow.Width;
            Engine = new GraphicsEngine(mainpanel);



        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mainpanel.Height = mainwindow.Height;
            mainpanel.Width = mainwindow.Width;
            Engine.canvasHeight = mainpanel.Height;
            Engine.canvasWidth = mainpanel.Width;
            Simulation = new Simulation(32, 15, Engine);
            //The function to call the mesagebox, then I'll implement it inside the graphical engine
            var prova = new dialoguebox("testo");
            mainpanel.Children.Add(prova);
            //Here to center the box
            Canvas.SetLeft(prova, 50);

            Simulation.TogglePause();
            startSimulation.Visibility = Visibility.Hidden;

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
    }
}
