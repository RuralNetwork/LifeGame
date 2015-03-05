using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGame
{
    public enum ModType
    {
        Property,
        ThingType,
    }

    public class ThingMod
    {
        public ModType Type { get; private set; }

        public int newType { get; private set; }
        /// <summary>
        /// List of changes that a Thing has undergone, rapresented by a float that can be positive or negative
        /// </summary>
        public List<Tuple<ThingProperty, float>> deltaValues { get; private set; }
    }
}
