using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LifeGame
{
    public class Event
    {
        // This class should contains time of the event, the message(actor, action), all the data from the previous n ticks and the data from the following n ticks
        // This class will manage also the UI, input and visualization
    }
    public enum SimulationType
    {
        //Slow: show every move, game tick is every 200ms, maybe 500ms or even less, 100mx
        Slow,
        //Medium: every move after the other, not really able to percieve the movements, but the animation still runs, smth like 20ms
        Medium,
        //Fast: graphicengine stops, doesn't update the screen anymore, each action is performed after the other, basically no game thick, as soon as it can, it just does the next thing
        Fast
    }

    //This class can be instantiated multiple times to permit multiple simulations to be run at the same time
    public class Simulation
    {
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
        List<Genome> hallOfFame;
        FastRandom rand = new FastRandom();
        public int lastID; // used and managed by the beings

        public int GridWidth { get; private set; }
        public int GridHeight { get; private set; }

        public SimulationState State { get; private set; } // last finished state
        SimulationState newState; //a state that is being created


        public Simulation(int gridWidth, int gridHeight, GraphicsEngine engine, SimulationState simState)// <- simState must be create swr else
        {
            this.engine = engine;
            thread = new Thread(Update);
            thread.Priority = ThreadPriority.Highest;

            IsInTrainingMode = true;
            PopulationCount = 100;
            hallOfFame = new List<Genome>(10);

            GridWidth = gridWidth;
            GridHeight = gridHeight;

            //Create state
            State = new SimulationState();
            State.Environment = new SimEnvironment(this);


            for (int i = 0; i < PopulationCount; i++)
            {

            }

        }

        public void RunPause()
        {
            IsRunning = !IsRunning;
            if (IsRunning)
            {
                thread.Start();
            }
        }

        public void Update()
        {
            newState = new SimulationState();

            while (IsRunning)
            {

                State.Update();

                // Manage population.
                if (Type == SimulationType.Fast)
                {

                    if (IsInTrainingMode)// fixed population size must be used while training the beings to behave "normally".
                    {
                        if (State.Population.Count > PopulationCount)
                        {
                            State.Population.InsertionSort((a, b) => -a.FitnessHistory.Mean.CompareTo(b.FitnessHistory.Mean)); // the minus -> decrescent
                            State.Population.RemoveRange(PopulationCount, State.Population.Count - PopulationCount);
                        }
                        else if (State.Population.Count < PopulationCount)// i don't just spawn new beings in random places and let die the ones spawned in unlucky places
                        {                                                       // because creating a new genome (even copying an existing one) is very expensive
                            int idx;
                            while (State.Population.Count < PopulationCount)
                            {
                                while (true)
                                {
                                    idx = rand.Next(State.Population.Count);
                                    var loc = State.Population[idx].Location.GetNearCell();// spawn in a cell adjacent to one of another being
                                    if (loc.X >= 0 && loc.Y >= 0 && loc.X < GridWidth && loc.Y < GridHeight)
                                    {
                                        State.Population.Add(new Being(this, loc, hallOfFame[rand.Next(10)]));
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            State = newState;// apply changes 
        }

        //public void Draw()
        //{
        //    foreach (var arr in Cells)
        //    {
        //        foreach (var cell in arr)
        //        {
        //            cell.Draw();
        //        }
        //    }

        //}
    }
}
