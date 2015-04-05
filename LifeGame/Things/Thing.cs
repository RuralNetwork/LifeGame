using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Diagnostics;

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

        public bool MustDraw = true;// mettere false quando non deve disegnare
        //Flag that is set to true when graphical update is needed
        private bool changed = false;

        public int ID { get; private set; }// devo ancora pensare a come eliminarli, perchè queste proprietà sono in comune anche con i Being
        public Polygon polygon; //I'll use polygon in both thing and being, in being i'll change the images inside the polygon, hopefully

        public Thing(ThingType type, Simulation simulation, GraphicsEngine engine, GridPoint location)//The type of thing should already be in the initialization
        {
            Type = type;
            Simulation = simulation;
            interactions = interactionsDicts[(int)type];
            Properties  = defPropsDicts[(int)type];
            updateDel = updateDels[(int)type];

            if (simulation.lastID == 4 * 10e9)
            {
                simulation.lastID = 0;
            }
            ID = simulation.lastID;
            simulation.lastID++;

            Location = location;
            Engine = engine;

            //Draw initial thing
            engine.addCell(this,location);
        }
        /// <summary>
        /// In order to test if graphics and back-end are linked
        /// </summary>
        public void showID()
        {
            Debug.Write("My ID is: " + this.ID + "\n");
        }
        public void changeType(ThingType type)
        {
            this.Type = type;
            Engine.updateCell(this);
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
            if (MustDraw && changed)
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

        public void Interact(ActionType action, Being actor)
        {
            interactions[action](this, actor);
        }
    }
}