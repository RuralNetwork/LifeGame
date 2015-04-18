using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        protected static FastRandom rand = new FastRandom();

        //constant
        public static int nThingProps = Enum.GetNames(typeof(ThingType)).Length;// get the number of elements of the enum ThingProperty at runtime

        protected Simulation simulation;
        protected GraphicsEngine engine;

        public Thing InnerThing { get; set; }
        public GridPoint Location { get; set; }
        public GridPoint OldLoc { get; set; }

        public ThingType Type { get; private set; }
        public List<Tuple<ThingType, Dictionary<ThingProperty, float>>> NewTypeQueue { get; private set; }
        /// <summary>
        /// Queue containing pending modification to be applied when all object in the simulation have generated their own pending changes.
        /// </summary>
        public Dictionary<ThingProperty, float> PropsQueueDelta { get; private set; }
        public Dictionary<ThingProperty, float> PropsQueueReset { get; private set; }

        //Dictionary<...,...> -> the keys must be unique
        //List<Tuple<...,...> -> the keys may not be unique


        public Dictionary<ThingProperty, float> Properties { get; private set; }

        public delegate void Effects(Thing target, Being actor);
        public Dictionary<ActionType, Effects> Interactions { get; set; }


        public delegate void UpdateDelegate();
        protected UpdateDelegate updateDel;

        public bool IsCarrObj;
        public int ID { get; private set; }
        public Polygon polygon; //I'll use polygon in both thing and being, in being i'll change the images inside the polygon, hopefully

        public Thing(ThingType type, Simulation simulation, GraphicsEngine engine, GridPoint location)//The type of thing should already be in the initialization
        {
            init(type);
            this.simulation = simulation;
            PropsQueueDelta = new Dictionary<ThingProperty, float>();
            PropsQueueReset = new Dictionary<ThingProperty, float>();
            NewTypeQueue = new List<Tuple<ThingType, Dictionary<ThingProperty, float>>>();

            ID = simulation.lastID;
            simulation.lastID++;

            Location = location;
            this.engine = engine;

            //Draw initial thing
            engine.addCell(this, location);
        }

        void init(ThingType type)
        {
            Type = type;
            Interactions = interactionsDicts[(int)type];
            updateDel = updateDels[(int)type];
            Properties = new Dictionary<ThingProperty, float>(propsDicts[(int)type]);
        }

        /// <summary>
        /// In order to test if graphics and back-end are linked
        /// </summary>
        public void showID()
        {
            Debug.Write("My ID is: " + this.ID + "\n");
        }

        /// <summary>
        /// This registers the changes to be applied at the end of the tick cycle, these are based on time and the environment
        /// </summary>
        /// <param name="container"></param>
        public virtual void Update()
        {
            updateDel();
        }

        public virtual void Apply()
        {
            if (NewTypeQueue.Count > 0)
            {
                var biggerThing = NewTypeQueue.First();
                foreach (var newThing in NewTypeQueue)
                {
                    if (newThing.Item2[ThingProperty.Height] * newThing.Item2[ThingProperty.Alpha] > //criterio: grandezza, non il peso
                        biggerThing.Item2[ThingProperty.Height] * biggerThing.Item2[ThingProperty.Alpha])
                    {
                        biggerThing = newThing;
                    }
                }
                Type = biggerThing.Item1;
                Interactions = interactionsDicts[(int)Type];
                updateDel = updateDels[(int)Type];
                Properties = biggerThing.Item2;

                NewTypeQueue.Clear();

                if (!IsCarrObj)
                {
                    engine.updateCell(this);                       //<- qui c'è la chiamata all'engine

                }
            }
            else
            {
                foreach (var prop in PropsQueueDelta)
                {
                    Properties[prop.Key] += prop.Value;
                    if (Properties[prop.Key] < 0) Properties[prop.Key] = 0.0001f;
                }

                foreach (var prop in PropsQueueReset)
                {
                    Properties[prop.Key] = prop.Value;
                }
            }

            PropsQueueDelta.Clear();
            PropsQueueReset.Clear();
        }

        public virtual void Draw(bool isCarriedObj = false)
        {

        }


        /// <summary>
        /// Change type of thing.
        /// </summary>
        /// <param name="newType"></param>
        /// <param name="props">Dictionary containing some properties. For all properties not included here it will be taken the default value of the type specified.
        ///                     It can be null
        /// </param>
        public void ChangeType(ThingType newType, Dictionary<ThingProperty, float> props)
        {
            var newDict = new Dictionary<ThingProperty, float>(propsDicts[(int)newType]);

            if (props != null)
            {
                foreach (var prop in props)
                {
                    newDict[prop.Key] = prop.Value;
                }
            }

            NewTypeQueue.Add(new Tuple<ThingType, Dictionary<ThingProperty, float>>(newType, newDict));
        }

        public void ChangeProp(ThingProperty prop, float deltaValue, bool toOverride)
        {
            if (!toOverride)
            {
                if (PropsQueueDelta.ContainsKey(prop))
                {
                    PropsQueueDelta[prop] += deltaValue;
                }
                else
                {
                    PropsQueueDelta.Add(prop, deltaValue);
                }
            }
            else
            {
                if (PropsQueueReset.ContainsKey(prop))
                {
                    PropsQueueReset[prop] = deltaValue;
                }
                else
                {
                    PropsQueueReset.Add(prop, deltaValue);
                }
            }
            // else, if a Thing changed type it shouldn't be changed
        }
    }
}