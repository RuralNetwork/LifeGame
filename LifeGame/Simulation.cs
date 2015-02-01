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
        private SimEnvironment _environment;
        private GraphicsEngine _engine;

        SimulationType Type { get; set; }
        public bool IsRunning { get; private set; }
        /// <summary>
        /// History of events.
        /// </summary>
        List<Event> Events { get; set; }

        public Simulation(SimEnvironment environment, GraphicsEngine engine)
        {
            _environment = environment;
            _engine = engine;
        }

        void RunPause()
        {
            IsRunning = !IsRunning;
        }
    }
}
