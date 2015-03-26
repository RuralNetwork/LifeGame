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
    ///     0   1   2   3
    ///    ___     ___
    ///   /   \___/   \___
    /// 0 \___/   \___/   \ ---
    ///   /   \___/   \___/
    /// 1 \___/   \___/   \ ---
    ///       \___/   \___/
    /// 
    /// </code>
    /// </remarks>
    public struct GridPoint
    {
        static FastRandom rand = new FastRandom();

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
                return (X % 2 == 0 ? (float)Y : (float)Y + 0.5f) * 1.154700538379f;//<- 2/sqrt(3)
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="angle">angle in radians, interval ]-2PI,inf[ , from +x axis, counterclockwise</param>
        /// <returns></returns>
        public GridPoint GetNearCell(float angle)
        {
            return GetNearCell((CellDirection)angle.AngleToDirection());
        }

        public GridPoint GetNearCell(CellDirection direction = CellDirection.Random)
        {
            if (direction == CellDirection.Random) direction = (CellDirection)rand.Next(6);
            switch (direction)
            {
                case CellDirection.TopRight:
                    return new GridPoint(X + 1, X % 2 == 0 ? Y - 1 : Y);
                case CellDirection.Top:
                    return new GridPoint(X, Y - 1);
                case CellDirection.TopLeft:
                    return new GridPoint(X - 1, X % 2 == 0 ? Y - 1 : Y);
                case CellDirection.BottomLeft:
                    return new GridPoint(X - 1, X % 2 == 0 ? Y : Y + 1);
                case CellDirection.Bottom:
                    return new GridPoint(X, Y + 1);
                case CellDirection.BottomRight:
                    return new GridPoint(X + 1, X % 2 == 0 ? Y : Y + 1);
                default:
                    break;
            }
            return new GridPoint();
        }
    }

    public enum CellDirection
    {
        TopRight = 0,
        Top = 1,
        TopLeft = 2,
        BottomLeft = 3,
        Bottom = 4,
        BottomRight = 5,
        Random = 6,
    }
}
