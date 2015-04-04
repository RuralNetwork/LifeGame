using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;

namespace LifeGame
{
    public class Event
    {
        // This class should contains time of the event, the message(actor, action), all the data from the previous n ticks and the data from the following n ticks
        // This class will manage also the UI, input and visualization
    }

    //This class can be instantiated multiple times to permit multiple simulations to be run at the same time
    public class Simulation
    {
        public int lastID; // used and managed by the beings
        public int TimeTick { get; set; }

        /// <summary>
        /// Tells if should be used PopulationCount and the hall of fame genome list to maintain the population.
        /// </summary>
        public bool IsInTrainingMode { get; set; }
        public int PopulationCount { get; set; }
        public bool IsRunning { get; private set; }
        public SimulationType Type { get; set; }
        /// <summary>
        /// History of events.
        /// </summary>
        public List<Event> Events { get; set; }

        Thread thread;
        GraphicsEngine engine;
        FastRandom rand = new FastRandom();

        public int GridWidth { get; private set; }// this must be an even number to permit the world to wrap around itself
        public int GridHeight { get; private set; }// this can be any number

        public SimEnvironment Environment { get; set; }
        public List<Being> Population { get; private set; }
        List<Genome> hallOfFame;

        public Thing[][] Terrain { get; set; }




        public Simulation(int gridWidth, int gridHeight, GraphicsEngine engine)
        {
            this.engine = engine;
            thread = new Thread(Update);
            thread.IsBackground = true;
            thread.Priority = ThreadPriority.Highest;

            IsInTrainingMode = true;
            PopulationCount = 100;
            hallOfFame = new List<Genome>(10);
            //Ho messo questo controllo per la larghezza del mondo, ma non so neanche se tu vuoi che sia possibile farlo grande quanto si vuole
            if ((engine.hexaW * gridWidth) - ((gridWidth - 1) * 10) < engine.canvasWidth && (engine.hexaH * gridHeight) - ((gridHeight - 1) * 10) < engine.canvasHeight)
            {
                GridWidth = gridWidth;
                GridHeight = gridHeight;

                Terrain = new Thing[GridWidth][];
                for (int i = 0; i < GridWidth; i++)
                {
                    Terrain[i] = new Thing[GridHeight];
                    for (int j = 0; j < GridHeight; j++)
                    {
                        Terrain[i][j] = new Thing(ThingType.Earth, this, engine, new GridPoint(i, j));// per metto Earth per ogni cella
                    }
                }
            }
            else
            {
                Debug.Write("Non bene");
                //engine.messageBox(null, null, 200, 20, "Mondo troppo largo, il massimo è:engine.canvasWidth/(engine.hexaW)+((canvasWidth/(engine.hexaW)-1))");
            }
            //Populate the terrain


        }

        public void TogglePause()
        {
            IsRunning = !IsRunning;
            if (IsRunning)
            {
                thread.Start();
            }
        }
        //This must be at a fixed rate, so the rate is the one defined by the user
        public void Update()
        {
            while (IsRunning)
            {
                TimeTick++;

                //Environment.Update();
                /*foreach (var arr in Terrain)
                {
                    foreach (var thing in arr)
                    {
                        thing.Update();
                    }
                }

                foreach (var arr in Terrain)
                {
                    foreach (var thing in arr)
                    {
                        thing.Apply();
                    }
                }
                */
                //// Manage population.
                //if (Type == SimulationType.Fast)
                //{

                //    if (IsInTrainingMode)// fixed population size must be used while training the beings to behave "normally".
                //    {
                //        if (_newState.Population.Count > PopulationCount)
                //        {
                //            _newState.Population.InsertionSort((a, b) => -a.FitnessMean.Value.CompareTo(b.FitnessMean.Value)); // the minus -> decrescent
                //            _newState.Population.RemoveRange(PopulationCount, _newState.Population.Count - PopulationCount);
                //        }
                //        else if (_newState.Population.Count < PopulationCount)// i don't just spawn new beings in random places and let die the ones spawned in unlucky places
                //        {                                                       // because creating a new genome (even copying an existing one) is very expensive
                //            int idx;
                //            while (_newState.Population.Count < PopulationCount)
                //            {
                //                while (true)
                //                {
                //                    idx = rand.Next(_newState.Population.Count);
                //                    var loc = _newState.Population[idx].Location.GetNearCell();// spawn in a cell adjacent to one of another being
                //                    if (loc.X >= 0 && loc.Y >= 0 && loc.X < GridWidth && loc.Y < GridHeight)
                //                    {
                //                        _newState.Population.Add(new Being(this, loc, hallOfFame[rand.Next(10)]));
                //                        break;
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}
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

        }
    }
}
