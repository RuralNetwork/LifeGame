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

namespace LifeGame
{
    /// <summary>
    /// Logica di interazione per MainMenu.xaml
    /// </summary>
    public partial class MainMenu : UserControl
    {
        FastRandom rand = new FastRandom();

        public MainMenu()
        {
            InitializeComponent();
        }

        private void newSimulation_Click(object sender, RoutedEventArgs e)
        {
            new Simulation();
            Simulation.Instance.IsSaved = true;
            simulationName.Text = "Nuova simulazione";
            newTerrain_Click(null, null);
            newGenome_Click(null, null);
            startButton.Content = "Inizia";
        }

        private void loadSimulation_Click(object sender, RoutedEventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Inserire il nome del file della simulazione");

            if (Serializer.Load(ref Simulation.Instance, input + ".sim"))
            {
                Simulation.Instance.InitLoad();
                Simulation.Instance.IsSaved = true;
                simulationName.Text = "Simulazione: " + input;
                terrainName.Text = "";
                genomeName.Text = "";
                startButton.Content = "Inizia";
            }
            else
            {
                MessageBox.Show("Caricamento fallito");
            }
        }

        private void saveSimulation_Click(object sender, RoutedEventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Inserire il nome del file della simulazione");
            if (Serializer.Save(Simulation.Instance, input + ".sim"))
            {
                Properties.Settings.Default.lastSim = input;
                Properties.Settings.Default.Save();
                Simulation.Instance.IsSaved = true;
                simulationName.Text = "Simulazione: " + input;
            }
            else
            {
                MessageBox.Show("Salvataggio fallito");
            }
        }

        private void newTerrain_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < GraphicsEngine.GRID_WIDTH; i++)
            {
                for (int j = 0; j < GraphicsEngine.GRID_HEIGHT; j++)
                {
                    Simulation.Instance.Terrain[i][j] = new Thing(ThingType.Grass, new GridPoint(i, j));
                    GraphicsEngine.Instance.ChangeCell(Simulation.Instance.Terrain[i][j]);
                }
            }
            foreach (var being in Simulation.Instance.Population)
            {
                Simulation.Instance.freeBeingObjs.Add(being.Value);
            }
            Simulation.Instance.Population.Clear();
            for (int i = 0; i < 30; i++)
            {
                int x = rand.Next(GraphicsEngine.GRID_WIDTH),
                    y = rand.Next(GraphicsEngine.GRID_HEIGHT);
                Simulation.Instance.Terrain[x][y].ChangeType(ThingType.Earth, null);
                Simulation.Instance.Terrain[x][y].Apply();

                x = rand.Next(GraphicsEngine.GRID_WIDTH);
                y = rand.Next(GraphicsEngine.GRID_HEIGHT);
                Simulation.Instance.Terrain[x][y].ChangeType(ThingType.Sand, null);
                Simulation.Instance.Terrain[x][y].Apply();

                x = rand.Next(GraphicsEngine.GRID_WIDTH);
                y = rand.Next(GraphicsEngine.GRID_HEIGHT);
                Simulation.Instance.Terrain[x][y].ChangeType(ThingType.Mountain, null);
                Simulation.Instance.Terrain[x][y].Apply();
            }
            for (int i = 0; i < 50; i++)
            {
                int x = rand.Next(GraphicsEngine.GRID_WIDTH),
                    y = rand.Next(GraphicsEngine.GRID_HEIGHT);
                Simulation.Instance.Terrain[x][y].ChangeType(ThingType.Water, null);
                Simulation.Instance.Terrain[x][y].Apply();

                x = rand.Next(GraphicsEngine.GRID_WIDTH);
                y = rand.Next(GraphicsEngine.GRID_HEIGHT);
                Simulation.Instance.Terrain[x][y].ChangeType(ThingType.Bush, null);
                Simulation.Instance.Terrain[x][y].Apply();
            }
            terrainName.Text = "Nuovo terreno";

        }

        private void loadTerrain_Click(object sender, RoutedEventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Inserire il nome del file del terreno");
            Thing[][] terr = new Thing[0][];
            if (Serializer.Load(ref terr, input + ".ter"))
            {
                foreach (var being in Simulation.Instance.Population)
                {
                    Simulation.Instance.freeBeingObjs.Add(being.Value);
                }
                Simulation.Instance.Population.Clear();
                Simulation.Instance.Terrain = terr;
                foreach (var arr in Simulation.Instance.Terrain)
                {
                    foreach (var thing in arr)
                    {
                        GraphicsEngine.Instance.ChangeCell(thing);
                    }
                }
                terrainName.Text = "Terreno: " + input;
            }
            else
            {
                MessageBox.Show("Caricamento fallito");
            }
        }

        private void saveTerrain_Click(object sender, RoutedEventArgs e)// here I save also the beings inside the terrain, later i will manage to resolve this
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Inserire il nome del file del terreno");

            foreach (var arr in Simulation.Instance.Terrain) // remove population from terrain,
            {
                foreach (var thing in arr)
                {
                    thing.InnerThing = null;
                }
            }
            if (Serializer.Save(Simulation.Instance.Terrain, input + ".ter"))//  save terrain,
            {
                terrainName.Text = "Terreno: " + input;
            }
            else
            {
                MessageBox.Show("Salvataggio fallito");
            }

            foreach (var being in Simulation.Instance.Population)// then repopulate terrain
            {
                Simulation.Instance.Terrain[being.Value.Location.X][being.Value.Location.Y] = being.Value;
            }

        }

        private void newGenome_Click(object sender, RoutedEventArgs e)
        {
            Simulation.Instance.NNLists = new NNGlobalLists();
            Simulation.Instance.HallOfFame = new HallOfFame();
            genomeName.Text = "Nuovo genoma";
        }

        private void loadGenome_Click(object sender, RoutedEventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Inserire il nome del file del genoma");
            Tuple<HallOfFame, NNGlobalLists> tuple = new Tuple<HallOfFame, NNGlobalLists>(null, null);
            if (Serializer.Load(ref tuple, input + ".gen"))
            {
                Simulation.Instance.HallOfFame = tuple.Item1;
                Simulation.Instance.NNLists = tuple.Item2;
                genomeName.Text = "Genoma: " + input;
            }
            else
            {
                MessageBox.Show("Caricamento fallito");
            }
        }

        private void saveGenome_Click(object sender, RoutedEventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Inserire il nome del file del genoma");
            if (Serializer.Save(new Tuple<HallOfFame, NNGlobalLists>(Simulation.Instance.HallOfFame, Simulation.Instance.NNLists), input + ".gen"))
            {
                genomeName.Text = "Genoma: " + input;
            }
            else
            {
                MessageBox.Show("Salvataggio fallito");
            }

        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            startButton.Content = "Riprendi";
        }
    }
}
