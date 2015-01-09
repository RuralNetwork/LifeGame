using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGame
{
    /// <summary>
    /// Element that can perform and undergo an action
    /// </summary>
    public abstract class Thing
    {
        public Dictionary<string, byte> Properties { get; set; }
        public delegate void Effects(Thing actor);
        public readonly Dictionary<ActionType, Effects> Interactions;

        public abstract void Update();
    }

    //TODO: add derived abstract classes such as eatable, collectible...
    //      in order to share properties and actions
}
