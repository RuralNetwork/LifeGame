using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LifeGame
{
    [Flags] // this attribute is useless (most of they are) but helps creating the inline documentation
    public enum BoolProps
    {
        CanContainBeing = 1,
        CanBeCarried = 2,
        CanSeeThrogh = 4,

    }

    /// <summary>
    /// Element that can interact with a being
    /// </summary>
    // TODO: consider using a more scientific attributes: eg using a radiation frequency chart to describe color and heat
    public abstract class Thing
    {
        public GridPoint Location { get; set; }
        public int Altitude { get; private set; }
        protected Simulation simulation;

        // Sight
        public abstract float R { get; }
        public abstract float G { get; }
        public abstract float B { get; }

        //Shouldn't this be speed? it's a float, if you set every input as a number between 0 and 1,
        //but this one is either 0 or 1, the network won't understand that easily what is going on
        //even though NN are not understable by humans, so mine is just a guess based on my experiments

        //If you want to change names, just do it, then do a refractoring
        //this should be a float because things can have different velocities, ex the movements of leafs is
        //slower than a running being
        public abstract float Speed { get; }

        //feel
        //Since it's a sensation and not a state, it sould be Pain and Temperature, not Painful and Warmth

        //Theese are not sensations, theese are qualities of objects that the beings can feel
        public abstract float Painful { get; }// ex: the roses are painful when touched
        public abstract float Weight { get; }
        public abstract float Temperature { get; }// this is right to be temperature

        //hearing
        public abstract float Amplitude { get; }
        public abstract float Pitch { get; }

        //smell
        public abstract float SmellIntensity { get; }
        public abstract float Smell { get; }


        public BoolProps BoolProperties { get; set; }


        public delegate void Effects(Being actor);
        public readonly Dictionary<ActionType, Effects> Interactions;

        public Thing(Simulation simulation)
        {
            this.simulation = simulation;
        }

        public abstract void Update();
        public abstract void Draw();
    }

    //TODO: add derived abstract classes such as eatable, collectible...
    //      in order to share properties and actions
}