﻿///
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

namespace LifeGame
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        private Environment Environment { get; set; }
        private Graphics Engine { get; set; }
        private Simulation Simulation { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Debug.Write("\n******************\nComponents Initialized\n******************\n");
            //Graphics should know nothing about environment, Simulation manages the relation between environment and graphic
            //So basically, init of the graphic engine, init of the environment with the width and the height of the board, then initialize the simulation 
            
            Engine = new Graphics();
            Environment= new Environment(10,10,Engine);
            Simulation = new Simulation(Environment, Engine);
           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Debug.Write("Button clicked\n");
            //this.Simulation;
        }
    }
}
