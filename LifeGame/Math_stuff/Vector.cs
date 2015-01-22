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
        public float Magnitude
        {
            get
            {
                return (float)Math.Sqrt(X * X + Y * Y); // .Net languages support only 64bit FPU operations, so every float is converted to double before operation
                                                        // math functions will be replaced when adding cudafy support
            }
        }
        public Vector Normalized
        {
            get
            {
                float mag = Magnitude;
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
