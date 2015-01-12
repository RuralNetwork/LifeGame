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
        //////// Perceptible properties
        // Sight
        public float R
        {
            get
            {
                return this.Color.ScR * Size;
            }
        }
        public float G
        {
            get
            {
                return this.Color.ScG * Size;
            }
        }
        public float B
        {
            get
            {
                return this.Color.ScB * Size;
            }
        }
        public abstract float Moving { get; }

        //feel
        public abstract float Painful { get; }// only for carried object
        public abstract float Weight { get; }// only for carried object
        public abstract float Warmth { get; }

        //hearing
        public abstract float Amplitude { get; }
        public abstract float Pitch { get; }

        //smell
        public abstract float SmellIntensity { get; }
        public abstract float Smell { get; }
        

        ////////////Intrisic properties
        public Color Color { get; set; }
        public float Size { get; set; }

        public delegate void Effects(Being actor);
        public readonly Dictionary<ActionType, Effects> Interactions;

        public abstract void Update();
        public abstract void Draw();
    }

    //TODO: add derived abstract classes such as eatable, collectible...
    //      in order to share properties and actions
}
