using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGame
{
    // To make the simulation behave correctly, the the simulation states must be sepatated from one time tick to another, for each tick must be created another SimulationState.
    // This should not cause lot more overhead than using a single state, because the majority of properties of the things gets updated each tick.
    // A single SimulationState can be cloned/serialized in order to create a new simulation 
    // The logic is:  - get data from oldState, process it and write it to newState
    //                - oldState <- newState
    //                - create new newState

    /// <summary>
    /// Contains every mutable property of every Thing in one simulation tick
    /// </summary>
    public class SimulationState
    {
        public int TimeTick { get; set; }
        public SimEnvironment Environment { get;  set; }
        public List<Being> Population { get; private set; }

        public Thing[][] Terrain { get; private set; }

        public void Update()
        {
            TimeTick++;
            foreach (var arr in Terrain)
            {
                foreach (var thing in arr)
                {
                    thing.Draw();
                }
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
