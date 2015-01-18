using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace LifeGame
{
    class Graphics
    {
        private Environment Environment;
        public Graphics(Environment environment) {
            Environment = environment;
        }
        //Array of items with draw

        //
        public void startSimulation(){
            Debug.Write("Simulation started\n");
        }
    }
}
