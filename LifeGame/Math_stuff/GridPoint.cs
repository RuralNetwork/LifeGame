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

        const float Sqrt3Over2 = 0.86602540378f;//<- sqrt(3)/2

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
                return (X % 2 == 0 ? (float)Y : (float)Y + 0.5f) / Sqrt3Over2;
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
        static public GridPoint CartesianToGrid(float x, float y)// questo codice è difficile da spiegare, se ti interessa chiedimi
        {
            bool evenX = (Math.Floor(x + 1f / 3f) % 2 == 0);
            y = (evenX ? y * Sqrt3Over2 : y * Sqrt3Over2 - 0.5f);

            int gridX = (int)Math.Floor(x);
            int gridY = (int)Math.Floor(y);

            if (x - gridX < 1f / 3f || x - gridX > 2f / 3f)
            {
                gridX = (int)Math.Round(x);
                gridY = (int)Math.Round(y);
            }
            else
            {
                bool b;
                if (y - gridY < 0.5f)
                {
                    b = (y < -1.5f * x + 1);
                    gridY += (b || evenX ? 0 : 1);
                }
                else
                {
                    b = (y > 1.5f * x);
                    gridY += (!b && evenX ? 0 : 1);
                }
                gridX += b ? 0 : 1;
            }
            return new GridPoint(gridX, gridY);
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
