using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGame.Things
{
    class Berry : Thing
    {

        public Berry(Simulation environment)
            : base(environment)
        {
            Interactions.Add(ActionType.Eat, (b, e) => { b.Hunger -= 0.1f; });
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
