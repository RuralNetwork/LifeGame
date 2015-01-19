using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGame
{
    /// <summary>
    /// Theese are only active actions. Passive actions are implemented in Thing.Interactions delegates.
    /// </summary>
    public enum ActionType
    {
        Walk,
        Sleep,
        Eat,
        Breed,
        Fight,
        Take,
        Drop,
        // Communicate love?
        // communicate hatred?
        // ...

    }

    /// <summary>
    /// Can be performed by a being
    /// </summary>
    public class Action
    {

        public ActionType Type { get; set; }

        public Dictionary<string, byte> Parameters { get; set; }

        /// <summary>
        /// Who is performing the action
        /// </summary>
        public Being Actor { get; set; }

        /// <summary>
        /// Who/What is undergoing the action
        /// </summary>
        public Thing CoActor { get; set; }

        public Action(ActionType type, Being actor, Thing coactor)
        {
            Type = type;
            Actor = actor;
            CoActor = coactor;
        }

        public void Perform()
        {
            CoActor.Interactions[this.Type](Actor); // <-- this is a delegate call  ( Effects(Being actor) )
        }

    }
}
