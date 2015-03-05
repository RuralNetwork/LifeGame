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

    public enum ThingProperty
    {
        Altitude, // beings actually don't use this
        Height,
        Alpha,// this can be either the transparency of the thing or the proportion of visual covered
        // We assume the things to occupy all cell's area, so if a thing should be narrow in real life, it will have a low value of Alpha.
        // this obviously is unrelated to the GUI
        Weigth,
        Color1,// TODO: this will be changed to a series of intensities for every frequency.
        Color2,// then the beings can evolve to perceive some of these frequencies
        Color3,
        Painful,
        Temperature,// this will be included in the frequency graph
        Amplitude,
        Pitch,
        SmellIntensity,
        Smell1,// I use Henning's smell prism (but it is old, if we find something more recent is better)
        Smell2,
        Smell3
    }

    /// <summary>
    /// Element that can interact with a being
    /// </summary>
    // TODO: consider using a more scientific attributes: eg using a radiation frequency chart to describe color and heat
    public partial class Thing
    {

        protected Simulation simulation;

        public GridPoint Location;

        public Dictionary<ThingProperty, float> Properties { get; private set; }
        List<float> internalProps;

        public BoolProps BoolProperties { get; set; }
        public delegate void Effects(Being actor, float energy);
        public Dictionary<ActionType, Effects> Interactions { get; private set; }
        public Thing InnerThing { get; set; }// this is a Being for a terrain Thing, the carried object for a Being
        //in the simulation there are two types of interaction:
        // 1) Thing->Being
        // 2) Being->Thing  (this include Being->Being)
        // (Thing->Thing is managed from above by SimulationState)
        // While any Thing know how a Being's body react to stimuli, a Being can't know about the Thing, because every Thing react in a different way.

        public List<ThingMod> ModQueue { get; set; }

        delegate void UpdateDelegate();
        UpdateDelegate updateDel;


        public Thing(Simulation simulation, GridPoint location)
        {
            this.simulation = simulation;
            Interactions = new Dictionary<ActionType, Effects>();
            Properties = new Dictionary<ThingProperty, float>();
            for (int i = 0; i < 15; i++)//                              !!!!update this if the number of properties change
            {
                Properties.Add((ThingProperty)i, 0);
            }

            Location = location;
        }

        /// <summary>
        /// This registers the changes to be applied at the end of the tick cycle, these are based on time and the environment
        /// </summary>
        /// <param name="container"></param>
        public virtual void Update(Thing container = null)
        {
            updateDel();
            if (InnerThing != null)
            {
                InnerThing.Update();
            }
        }

        public virtual void Draw(bool isCarriedObj = false)
        {
            // chiama qui quello che ti serve per disegnare, che hai definito nel file ThingImplementations
            // tieni conto se il Thing è trasportato da un Being ( il parametro isCarriedObj)

            if (InnerThing != null)
            {
                InnerThing.Draw();
            }
        }

        public void Apply()
        {
            var typeChanged = false;
            foreach (var mod in ModQueue)
            {

                if (typeChanged)
                {
                    break;
                }
            }
            if (InnerThing != null)
            {
                InnerThing.Apply();
            }


        }

        //public Thing Clone()
        //{
        //    return (Thing)MemberwiseClone();// this automatically creates a shallow copy of the current object, 
        //    // actually this is good because in this way there will be only one instance of the dictionary for every Thing of the same type.
        //    // shallow copy -> only value types and object references
        //}
    }

    //TODO: add derived abstract classes such as eatable, collectible...
    //      in order to share properties and actions
}