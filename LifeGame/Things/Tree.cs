using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGame.Things
{
    class Tree : Thing
    {

        public Tree(Simulation simulation)
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
