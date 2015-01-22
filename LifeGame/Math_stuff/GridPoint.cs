using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGame
{
    /// <summary>
    /// Structure rapresenting the coordinate of a cell
    /// </summary>
    /// <remarks>
    /// The grid should look as below:
    /// <code>
    ///    0  1  2  3
    ///    __    __
    ///   /  \__/  \__
    /// 0 \__/  \__/  \ ---
    ///   /  \__/  \__/
    /// 1 \__/  \__/  \ ---
    ///      \__/  \__/
    /// 
    /// </code>
    /// </remarks>
    public struct GridPoint
    {

        public int X { get; set; }
        public int Y { get; set; }
        public float CartesianX
        {
            get
            {
                return (float)X;
            }
        }
        public float CartesianY
        {
            get
            {
                return X % 2 == 0 ? (float)Y : (float)Y + 0.5f;
            }
        }

        public GridPoint(int x, int y)
            : this()
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// For convention: the origin is the center of the cell (0, 0), then the cell (1, 0) correspond to the point (1, 0.5)
        /// </summary>
        static public GridPoint CartesianToGrid(float x, float y)
        {
            return new GridPoint((int)Math.Round(x), (int)(x % 2 == 0 ? Math.Round(y) : y));
        }

        public GridPoint Top()
        {
            return new GridPoint(X, Y - 1);
        }

        public GridPoint TopRight()
        {
            return new GridPoint(X + 1, X % 2 == 0 ? Y - 1 : Y);
        }

        public GridPoint BottomRight()
        {
            return new GridPoint(X + 1, X % 2 == 0 ? Y : Y + 1);
        }

        public GridPoint Bottom()
        {
            return new GridPoint(X, Y + 1);
        }

        public GridPoint BottomLeft()
        {
            return new GridPoint(X - 1, X % 2 == 0 ? Y : Y + 1);
        }

        public GridPoint TopLeft()
        {
            return new GridPoint(X - 1, X % 2 == 0 ? Y - 1 : Y);
        }
    }
}
