using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LifeGame
{
    [Flags] // this attribute is useless (most of them are) but helps creating the inline documentation
    public enum BoolProps
    {
        CanContainBeing = 1,
        CanBeCarried = 2,
        CanSeeThrogh = 4,
        Eatable = 8,


    }

    /// <summary>
    /// Properties that are common to all Things
    /// </summary>
    public enum ThingProperty
    {
        Height,
        Alpha,// this can be either the transparency of the thing or the proportion of visual covered
        // We assume the things to occupy all cell's area, so if a thing should be narrow in real life, it will have a low value of Alpha.
        // this obviously is unrelated to the GUI
        Weigth,
        Color1,// TODO: this will be changed to a series of intensities for every frequency.
        Color2,// then the beings can evolve to perceive some of these frequencies
        Color3,
        Moving,
        Painful,
        Temperature,// this will be included in the frequency graph
        Amplitude,
        Pitch,
        SmellIntensity,
        Smell1,// I use Henning's smell prism (but it is old, if we find something more recent is better)
        Smell2,
        Smell3,
        Wet
    }

    /// <summary>
    /// Element that can interact with a being
    /// </summary>
    // TODO: consider using a more scientific attributes: eg using a radiation frequency chart to describe color and heat
    public partial class Thing
    {

        protected Simulation Simulation;
        private GraphicsEngine Engine;
        public GridPoint Location { get; set; }

        public ThingType Type { get; private set; }

        static FastRandom rand = new FastRandom();

        public Dictionary<ThingProperty, float> Properties { get; private set; }
        List<float> internalProps;

        public BoolProps BoolProperties { get; set; }
        public delegate void Effects(Being actor);
        public Dictionary<ActionType, Effects> Interactions { get; private set; }
        public Thing InnerThing { get; set; }// this is a Being for a terrain Thing, the carried object for a Being
        //in the simulation there are two types of interaction:
        // 1) Thing->Being
        // 2) Being->Thing  (this include Being->Being)
        // (Thing->Thing is managed from above by SimulationState)
        // While any Thing know how a Being's body react to stimuli, a Being can't know about the Thing, because every Thing react in a different way.

        public List<ThingMod> ModQueue { get; set; }

        protected delegate void UpdateDelegate();
        protected UpdateDelegate updateDel;

        //Flag that is set to true when graphical update is needed
        private bool changed = false;

        public Thing(ThingType type, Simulation simulation, GraphicsEngine engine, GridPoint location)//The type of thing should already be in the initialization
        {
            Type = type;
            Simulation = simulation;
            Interactions = interactionsDicts[(int)type];
            internalProps = new List<float>(nInternalProps[(int)type]);
            updateDel = updateDels[(int)type];
            Properties = new Dictionary<ThingProperty, float>();
            for (int i = 0; i < 16; i++)//                              !!!!update this if the number of properties change
            {
                Properties.Add((ThingProperty)i, 0);
            }

            Location = location;
            Engine = engine;

            //Draw initial thing
            engine.addCell(location);
        }

        /// <summary>
        /// This registers the changes to be applied at the end of the tick cycle, these are based on time and the environment
        /// </summary>
        /// <param name="container"></param>
        public virtual void Update(Thing container = null)
        {
            //updateDel();
            if (InnerThing != null)
            {
                InnerThing.Update();
            }
            if (changed)
            {
                this.Draw();
            }
        }

        public virtual void Draw(bool isCarriedObj = false)
        {
            // 

            if (InnerThing != null)
            {
                InnerThing.Draw();
            }
        }

        public void Apply()
        {
            // spesso viene richiesta la creazione di nuovi Thing, ma non esistendo lo spazio vuoto, 
            // si dovrà fare sempre un confronto tra la cella target e gli oggetti contenuti nella ModQueue.
            // la stessa cosa vale per 

            var typeChanged = false;
            /*foreach (var mod in ModQueue)
            {

                if (typeChanged)
                {
                    break;
                }
            }*/
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