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
        HideSelf,
        HideSth,
        Eat,
        Excrete, //(correggimi se è sbagliato)
        Breed,
        Fight,
        Take,
        Drop,
        // Communicate love?
        // communicate hatred?
        // ...

    }


    public class Action
    {

        public ActionType Type { get; set; }

        public Dictionary<string, byte> Parameters { get; set; }

        public Thing Actor { get; set; }
        public List<Thing> CoActors { get; set; }

        public Action(ActionType type, Thing actor, List<Thing> coactors)
        {
            Type = type;
            Actor = actor;
            CoActors = coactors;
        }

        public void Perform()
        {
            foreach (var coactor in CoActors)
            {
                if (coactor.Interactions.ContainsKey(this.Type))
                {
                    coactor.Interactions[this.Type](Actor); // <-- this is a delegate call  ( Effects(Thing actor) )
                }
            }
        }

    }
}
