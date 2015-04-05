using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGame
{

    /// <summary>
    /// Thing modification. In every Thing, there is the list ModQueue, that store an array of ThingMod
    /// </summary>
    public class ThingMod
    {
        public ModType Type { get; private set; }

        public Thing Thing { get; private set; }
        /// <summary>
        /// List of changes that a Thing has undergone, rapresented by a float that can be positive or negative
        /// </summary>
        public List<Tuple<ThingProperty, float>> DeltaValues { get; private set; }

        public ThingMod(params Tuple<ThingProperty, float>[] deltaValues)
        {
            Type = ModType.Property;
            DeltaValues = new List<Tuple<ThingProperty, float>>(deltaValues);
        }
        public ThingMod(Thing thing, bool isInnerThing = false)
        {
            Type = (isInnerThing ? ModType.InnerThing : ModType.ThingType);
            Thing = thing;
        }
    }
}
