using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGame
{
    public class Event
    {

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
        /// History of events. Key: time tick. Value: message.
        /// </summary>
        Dictionary<int, string> Events { get; set; }

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
