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
    }
    public enum SimulationType
    {
        Accurate,
        /// <summary>
        /// A hall of fame based simulation type. the best genomes will be inserted in the next offsprings after the old owners dies.
        /// </summary>
        Fast
    }
    public class Simulation
    {
        SimulationType Type { get; set; }
        public bool IsRunning { get; private set; }
        /// <summary>
        /// History of events.
        /// </summary>
        List<Event> Events { get; set; }

        Environment _environment;

        public Simulation(int gridWidth, int gridHeight)
        {
            
            _environment = new Environment(gridWidth, gridHeight);
        }

        void RunPause()
        {
            IsRunning = !IsRunning;
        }
    }
}
