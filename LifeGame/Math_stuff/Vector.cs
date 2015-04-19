using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                return (float)Math.Sqrt(X * X + Y * Y);
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
