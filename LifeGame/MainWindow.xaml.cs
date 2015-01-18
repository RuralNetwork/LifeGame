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
        public Environment environment { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Debug.Write("\n******************\nComponents Initialized\n******************\n");
            GridPoint a = new GridPoint(0, 1);
            this.environment = new Environment();
            this.environment.Cells = new Cell[1][];
            this.environment.Cells[0] = new Cell[1];
            this.environment.Cells[0][0] = new Cell(this.environment, 0, 0);

            new Being(this.environment,1, new GridPoint(0, 0));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Debug.Write("Button clicked\n");
            this.environment.Draw();
        }
    }
}
