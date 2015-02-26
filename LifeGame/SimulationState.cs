using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGame
{
    // To make the simulation behave correctly, the the simulation states must be sepatated from one time tick to another, for each tick must be created another SimulationState.
    // A SimulationState can be used to create a new simulation 
    // The logic is:  - get data from oldState, process it and write it to newState
    //                - oldState <- newState
    //                - create new newState

    /// <summary>
    /// Contains every mutable property of every Thing in one simulation tick
    /// </summary>
    public class SimulationState
    {
        Simulation simulation;

        public SimEnvironment Environment { get; set; }
        public List<Being> Population { get; private set; }

        public Thing[][] Terrain { get; private set; }

        public SimulationState(Simulation simulation)
        {
            this.simulation = simulation;
            Population = new List<Being>();
            Terrain = new Thing[simulation.GridWidth][];
            for (int i = 0; i < simulation.GridWidth; i++)
            {
                Terrain[i] = new Thing[simulation.GridHeight];
            }
        }


        //                 questo metodo va rifatto, pensavo di eliminare population e inserire i beings nei Thing contenitori (ovvero le celle), 
        //                 e Update veniva chiamato dando come parametro il contenitore
        public void Update()// effectively it doesn't write in this state, but in the next 
        {
            var width = Terrain.Length;
            var height = Terrain[0].Length;

            // copy things and beings
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    simulation._newState.Terrain[i][j] = Terrain[i][j].Clone();
                }
            }
            foreach (var tuple in simulation._newThingsQueue)// Tuple<GridPoint, Thing>
            {
                Terrain[tuple.Item1.X][tuple.Item1.Y] = tuple.Item2;
            }
            foreach (var being in Population)
            {
                var newBeing = (Being)being.Clone();// ouch, down casting is heavy
                newBeing.InnerThing = being.InnerThing.Clone();
                simulation._newState.Population.Add(newBeing);
            }

            // modify them
            foreach (var arr in Terrain)
            {
                foreach (var thing in arr)
                {
                    thing.Update();
                }
            }

            for (int i = 0; i < Population.Count; i++)
            {
                Population[i].Update(i);
            }

        }

        public void Draw()
        {
            foreach (var arr in Terrain)
            {
                foreach (var thing in arr)
                {
                    thing.Draw();
                }
            }

            foreach (var being in Population)
            {
                being.Draw();
            }
        }
    }
}
