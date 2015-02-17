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
            //---------------------Test Space------------------------


            //-----------------------------------------------------

            Debug.Write("\n******************\nComponents Initialized\n******************\n");

            Engine = new GraphicsEngine(mainpanel);
            var simState = new SimulationState();
            //TODO: create simState with Engine

            Simulation = new Simulation(100, 100, Engine, simState);


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Debug.Write("Button clicked\n");
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    Engine.addCell(x, y);
                }
            }//Engine.addCell(0, 0);
            //this.Simulation;
        }
    }
}
