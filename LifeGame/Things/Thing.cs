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
    public abstract class Thing
    {
        protected Environment environment;
        // Sight
        public abstract float R { get; }
        public abstract float G { get; }
        public abstract float B { get; }
        //Shouldn't this be speed? it's a float, if you set every input as a number between 0 and 1,
        //but this one is either 0 or 1, the network won't understand that easily what is going on
        //even though NN are not understable by humans, so mine is just a guess based on my experiments
        public abstract float Moving { get; }

        //feel
        //Since it's a sensation and not a state, it sould be Pain and Temperature, not Painful and Warmth
        public abstract float Painful { get; }
        public abstract float Weight { get; }
        public abstract float Warmth { get; }

        //hearing
        public abstract float Amplitude { get; }
        public abstract float Pitch { get; }

        //smell
        public abstract float SmellIntensity { get; }
        public abstract float Smell { get; }

        
        public delegate void Effects(Being actor);
        public readonly Dictionary<ActionType, Effects> Interactions;

        public Thing(Environment environment)
        {
            this.environment = environment;
        }

        public abstract void Update();
        public abstract void Draw();
    }

    //TODO: add derived abstract classes such as eatable, collectible...
    //      in order to share properties and actions
}
