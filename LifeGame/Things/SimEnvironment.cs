using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Diagnostics;

namespace LifeGame
{
    public class SimEnvironment : Thing
    {
        public SimEnvironment(Simulation simulation, GridPoint location)
            : base(simulation, location)
        {

        }

        public override void Update(Thing container = null)
        {

        }

        public override void Draw(bool isCarriedObj = false)
        {

        }
    }

}
