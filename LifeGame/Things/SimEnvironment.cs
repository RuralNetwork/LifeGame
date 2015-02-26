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
        public SimEnvironment(Simulation simulation)
            : base(simulation)
        {

        }

        public override void Update(Thing container = null)
        {
            throw new NotImplementedException();
        }

        public override void Draw(bool isCarriedObj = false)
        {
            throw new NotImplementedException();
        }
    }

}
