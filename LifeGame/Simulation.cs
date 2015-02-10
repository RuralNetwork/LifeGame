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
        /// <summary>
        /// A hall of fame based simulation type. the best genomes will be inserted in the next offsprings after the old owners dies.
        /// </summary>
        //Fast: graphicengine stops, doesn't update the screen anymore, each action is performed after the other, basically no game thick, as soon as it can, it just does the next thing
        Fast
    }
    public class Simulation
    {
        public int PopulationCount;

        private SimEnvironment environment;
        private GraphicsEngine engine;

        //instead of using a SortedDictionary I use a Dictionary and store the fittest genomes in an hall of fame
        Dictionary<float, NNGenome> NNGenomeList; // key: 
        List<NNGenome> hallOfFame;

        SimulationType Type { get; set; }
        public bool IsRunning { get; private set; }
        /// <summary>
        /// History of events.
        /// </summary>
        List<Event> Events { get; set; }

        public Simulation(SimEnvironment environment, GraphicsEngine engine)
        {
            this.environment = environment;
            this.engine = engine;
        }

        public void RunPause()
        {
            IsRunning = !IsRunning;
        }

        //for now i put some code here, until we better define the methods for progressing the simulation 
        public void Update()
        {
            //code for SimulationType.Fast
            environment.Update();
            if (environment.Population.Count< PopulationCount)
            {
                
            }
            //if fixed size population (should be used while the beings don't know how reproduce yet)

        }
    }
}
