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
        Eatable = 8,


    }

    /// <summary>
    /// Element that can interact with a being
    /// </summary>
    // TODO: consider using a more scientific attributes: eg using a radiation frequency chart to describe color and heat
    public abstract class Thing
    {
        protected Simulation simulation;

        // properties own by all things
        public int Altitude { get; private set; } // beings actually don't use this
        public float Height { get; set; }
        public float Alpha { get; private set; }// this can be either the transparency of the thing or the proportion of visual covered
        // We assume the things to occupy all cell's area, so if a thing should be narrow in real life, it will have a low value of Alpha.
        // this obviously is unrelated to the GUI
        public float Weigth { get; set; }
        public Color Color { get; set; }// TODO: this will be changed to a series of intensities for every frequency.
        //                                 then the beings can evolve to perceive some of these frequencies
        public float Painful { get; set; }
        public float Temperature { get; set; }// this will be included in the frequency graph
        public float Amplitude { get; set; }
        public float Pitch { get; set; }
        public float SmellIntensity { get; set; }
        public Vector3D Smell { get; set; }// I use Henning's smell prism (but it is old, if we find something more recent is better)


        public BoolProps BoolProperties { get; set; }
        public delegate void Effects(Being actor, float energy);
        public Dictionary<ActionType, Effects> Interactions { get; private set; }
        public Thing InnerThing { get; set; }// this is a Being for a terrain Thing, the carried object for a Being
        //in the simulation there are two types of interaction:
        // 1) Thing->Being
        // 2) Being->Thing  (this include Being->Being)
        // (Thing->Thing is managed from above by SimulationState)
        // While any Thing know how a Being's body react to stimuli, a Being can't know about the Thing, because every Thing react in a different way.



        public Thing(Simulation simulation)
        {
            this.simulation = simulation;
        }

        public abstract void Update(Thing container = null);
        public abstract void Draw(bool isCarriedObj = false);

        public Thing Clone()
        {
            return (Thing)MemberwiseClone();// this automatically creates a shallow copy of the current object, 
            // actually this is good because in this way there will be only one instance of the dictionary for every Thing of the same type.
            // shallow copy -> only value types and object references
        }
    }

    //TODO: add derived abstract classes such as eatable, collectible...
    //      in order to share properties and actions
}