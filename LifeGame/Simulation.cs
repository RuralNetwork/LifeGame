using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public SimEnvironment Environment { get; private set; }


        GraphicsEngine engine;
        List<Genome> hallOfFame;
        FastRandom rand = new FastRandom();


        public Simulation(int width, int height, GraphicsEngine engine)
        {
            Environment = new SimEnvironment(width, height, engine, this);
            this.engine = engine;
            hallOfFame = new List<Genome>(10);
        }

        public void RunPause()
        {
            IsRunning = !IsRunning;
        }

        //for now i put some code here, until we better define the methods for progressing the simulation 
        public void Update()
        {
            Environment.Update();

            if (IsInTrainingMode)// fixed population size must be used while training the beings to behave "normally".
            {
                if (Environment.Population.Count > PopulationCount)
                {
                    Environment.Population.InsertionSort((a, b) => -a.FitnessHistory.Mean.CompareTo(b.FitnessHistory.Mean)); // the minus -> decrescent
                    Environment.Population.RemoveRange(PopulationCount, Environment.Population.Count - PopulationCount);
                }
                else if (Environment.Population.Count < PopulationCount)// i don't just spawn new beings in random places and let die the ones spawned in unlucky places
                {                                                       // because creating a new genome (even copying an existing one) is very expensive
                    int idx;
                    while (Environment.Population.Count < PopulationCount)
                    {
                        while (true)
                        {
                            idx = rand.Next(Environment.Population.Count);
                            var loc = Environment.Population[idx].Location.GetNearCell();// spawn in a cell adjacent to one of another being
                            if (loc.X >= 0 && loc.Y >= 0 && loc.X < Environment.GridWidth && loc.Y < Environment.GridHeight)
                            {
                                idx = rand.Next(10); // hallOfFame count
                                var genome = hallOfFame[idx];
                                Environment.Population.Add(new Being(Environment, loc, genome));
                                break;
                            }
                        }
                    }
                }

            }

        }
    }
}
