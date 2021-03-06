﻿using System;
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
    [Serializable]
    public partial class Thing
    {
        protected static FastRandom rand = new FastRandom();

        //constant
        public static int nThingProps = Enum.GetNames(typeof(ThingType)).Length;// get the number of elements of the enum ThingProperty at runtime

        public Thing InnerThing { get; set; }
        public GridPoint Location { get; set; }

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

        public Dictionary<ActionType, Action<Thing, Being>> Interactions { get; set; }


        protected Action updateDel;

        public bool IsCarrObj;

        public Thing(ThingType type, GridPoint location)//The type of thing should already be in the initialization
        {
            Location = location;
            init(type);
            PropsQueueDelta = new Dictionary<ThingProperty, float>();
            PropsQueueReset = new Dictionary<ThingProperty, float>();
            NewTypeQueue = new List<Tuple<ThingType, Dictionary<ThingProperty, float>>>();
        }

        void init(ThingType type)
        {
            Type = type;
            Interactions = interactionsDicts[(int)type];
            updateDel = updateDels[(int)type];
            Properties = new Dictionary<ThingProperty, float>(propsDicts[(int)type]);
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
                    GraphicsEngine.Instance.ChangeCell(this);                       //<- qui c'è la chiamata all'engine
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