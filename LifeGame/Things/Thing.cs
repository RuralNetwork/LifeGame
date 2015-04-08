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
        public Dictionary<ActionType, Effects> Interactions { get; set; }
        public Thing InnerThing { get; set; }// this is a Being for a terrain Thing
        //in the simulation there are two types of interaction:
        // 1) Thing->Being
        // 2) Being->Thing  (this include Being->Being)
        // (Thing->Thing is managed from above by SimulationState)
        // While any Thing know how a Being's body react to stimuli, a Being can't know about the Thing, because every Thing react in a different way.

        public List<Thing> InnerThingQueue { get; private set; }
        public Dictionary<ThingProperty, float> PropsQueue { get; private set; }

        protected delegate void UpdateDelegate();
        protected UpdateDelegate updateDel;

        public bool IsCarrObj;
        public int ID { get; private set; }// devo ancora pensare a come eliminarli, perchè queste proprietà sono in comune anche con i Being
        public Polygon polygon; //I'll use polygon in both thing and being, in being i'll change the images inside the polygon, hopefully

        public Thing(ThingType type, Simulation simulation, GraphicsEngine engine, GridPoint location)//The type of thing should already be in the initialization
        {
            Type = type;
            Simulation = simulation;
            Interactions = interactionsDicts[(int)type];
            Properties = propsDicts[(int)type];
            updateDel = updateDels[(int)type];
            InnerThingQueue = new List<Thing>();
            PropsQueue = new Dictionary<ThingProperty, float>();

            if (simulation.lastID == 4 * 10e9)
            {
                simulation.lastID = 0;
            }
            ID = simulation.lastID;
            simulation.lastID++;

            Location = location;
            Engine = engine;

            //Draw initial thing
            engine.addCell(this, location);
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
        public virtual void Update()
        {
            PropsQueue = new Dictionary<ThingProperty, float>();
            InnerThingQueue = new List<Thing>();
            updateDel();
            if (InnerThing != null)
            {
                InnerThing.Update();
            }
        }

        public virtual void Apply()
        {

            foreach (var prop in PropsQueue)
            {
                Properties[prop.Key] += prop.Value;
            }
            PropsQueue.Clear();

            // If InnerThing!=null then surely InnerThingQueue will be empty (unless there's a bug)
            // If InnerThing==null then there could be more than one thing that wants to go here,
            // only the bigger one will succeed, others will be placed in random near cells.

            if (InnerThingQueue.Count > 0)// also exclude carried objects
            {
                if (InnerThingQueue[0] == null)// being removed
                {
                    InnerThing = null;
                }
                else // cell already free, ready to be occupied
                {
                    foreach (var being in InnerThingQueue)
                    {
                        being.Apply();
                    }
                    InnerThing = InnerThingQueue[0];
                    int idx = 0;
                    for (int i = 1; i < InnerThingQueue.Count; i++)
                    {
                        if (InnerThingQueue[i].Properties[ThingProperty.Weigth] > InnerThing.Properties[ThingProperty.Weigth])// the heaviest wins
                        {
                            InnerThing = InnerThingQueue[i];
                            idx = i;
                        }
                    }
                    InnerThingQueue.RemoveAt(idx);
                    InnerThing.Location = Location;

                    // Find near free cells (free either now and in next step) to put the remaining beings in the queue using a chaotic path
                    foreach (var being in InnerThingQueue)
                    {
                        var newLoc = Location;
                        Thing newCell;
                        do
                        {
                            newLoc = newLoc.GetNearCell();
                            newCell = Simulation.Terrain[newLoc.X][newLoc.Y];
                        } while (newCell.InnerThing != null || newCell.InnerThingQueue.Count != 0);
                        newCell.InnerThing = being;
                    }
                }
                InnerThingQueue.Clear();
            }
        }

        public virtual void Draw(bool isCarriedObj = false)
        {
            if (InnerThing != null)
            {
                InnerThing.Draw();
            }
        }

        public void ChangeProp(ThingProperty prop, float deltaValue)
        {
            if (PropsQueue.ContainsKey(prop))
            {
                PropsQueue[prop] += deltaValue;
            }
            else
            {
                PropsQueue.Add(prop, deltaValue);
            }
        }
    }
}