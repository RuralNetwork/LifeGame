using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGame
{

    public struct Vector
    {
        public float X { get; set; }
        public float Y { get; set; }
        ///<summary>
        ///Eucledian length
        ///</summary>
        public float Magnitude
        {
            get
            {
                return (float)Math.Sqrt(X * X + Y * Y); // .Net languages support only 64bit FPU operations, so every float is converted to double before operation
                                                        // math functions will be replaced when adding cudafy support
            }
        }
        ///<summary>
        ///Returns the normalized vector
        ///</summary>
        public Vector Normalized
        {
            get
            {
                float mag = this.Magnitude;
                return new Vector(X / mag, Y / mag);
            }
        }

        public Vector(float x, float y)
            : this()
        {
            X = x;
            Y = y;
        }
    }
}
