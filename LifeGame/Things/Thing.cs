using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LifeGame
{

    /// <summary>
    /// Element that can interact with a being
    /// </summary>
    // TODO: consider using a more scientific attributes: eg using a radiation frequency chart to describe color and heat
    public partial class Thing
    {
        //constant
        static int nThingProps = Enum.GetNames(typeof(ThingType)).Length;// get the number of elements of the enum ThingProperty at runtime

        protected Simulation Simulation;
        private GraphicsEngine Engine;
        public GridPoint Location { get; set; }

        public ThingType Type { get; private set; }

        static FastRandom rand = new FastRandom();

        public Dictionary<ThingProperty, float> Properties { get; private set; }

        //public BoolProps BoolProperties { get; set; }
        public delegate void Effects(Thing target, Being actor);
        Dictionary<ActionType, Effects> interactions { get; set; }
        public Thing InnerThing { get; set; }// this is a Being for a terrain Thing, the carried object for a Being
        //in the simulation there are two types of interaction:
        // 1) Thing->Being
        // 2) Being->Thing  (this include Being->Being)
        // (Thing->Thing is managed from above by SimulationState)
        // While any Thing know how a Being's body react to stimuli, a Being can't know about the Thing, because every Thing react in a different way.

        public List<ThingMod> ModQueue { get; set; }

        protected delegate void UpdateDelegate();
        protected UpdateDelegate updateDel;

        public int ID { get; private set; }// devi dirmi come vengono gestiti graficamente i Thing eliminati, così posso vedere come riciclare gli ID

        public Thing(ThingType type, Simulation simulation, GraphicsEngine engine, GridPoint location)//The type of thing should already be in the initialization
        {
            Type = type;
            Simulation = simulation;
            interactions = interactionsDicts[(int)type];
            Properties = propsDicts[(int)type];
            updateDel = updateDels[(int)type];
            ModQueue = new List<ThingMod>(10);

            if (simulation.lastID == 4 * 10e9)
            {
                simulation.lastID = 0;
            }
            ID = simulation.lastID;
            simulation.lastID++;

            Location = location;
            Engine = engine;

            //Draw initial thing
            engine.addCell(location);
        }

        /// <summary>
        /// This registers the changes to be applied at the end of the tick cycle, these are based on time and the environment
        /// </summary>
        /// <param name="container"></param>
        public virtual void Update()
        {
            updateDel();
            if (InnerThing != null)
            {
                InnerThing.Update();
            }
        }

        public void Apply()
        {
            // spesso viene richiesta la creazione di nuovi Thing, ma non esistendo lo spazio vuoto, 
            // si dovrà fare sempre un confronto tra la cella target e gli oggetti contenuti nella ModQueue.
            // la stessa cosa vale per 

            var typeChanged = false;
            foreach (var mod in ModQueue)
            {
                if (mod.Type == ModType.ThingType)
                {
                    typeChanged = true;
                    break;
                }
            }

            if (typeChanged)
            {
                var being = InnerThing;
                var things = new List<Thing>();
                foreach (var mod in ModQueue)
                {
                    if (mod.Type == ModType.ThingType)
                    {
                        things.Add(mod.Thing);
                        break;
                    }
                }
                //for
            }
            if (InnerThing != null)
            {
                InnerThing.Apply();
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

        public void Interact(ActionType action, Being actor)
        {
            interactions[action](this, actor);
        }
    }
}