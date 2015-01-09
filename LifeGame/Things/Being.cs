using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGame
{
    // The beings are contained in Cell.Items and they are moved to other cells when they do Walk action
    public class Being : Thing
    {
        Environment _environment;
        public Cell Location { get; set; }

        public Thing CarriedObj { get; set; }

        public Being(Environment environment)
        {
            _environment = environment;
        }

        public override void Update()
        {
            // run n cycles of his neural net
            // then do the choosen action

            // dummy code
            var food = new List<Thing>() { Location.Items[0] };
            var eating = new Action(ActionType.Eat, this, food);
            eating.Perform();
        }
    }
}
