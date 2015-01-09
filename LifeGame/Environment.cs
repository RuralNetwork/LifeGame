using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LifeGame
{
    public struct GridPoint
    {
        //    0  1  2  3
        //    __    __
        //   /  \__/  \__
        // 0 \__/  \__/  \ ---
        //   /  \__/  \__/
        // 1 \__/  \__/  \ ---
        //      \__/  \__/

        public float X { get; set; }
        public float Y { get; set; }

        public GridPoint(float x, float y)
            : this()
        {
            X = x;
            Y = y;
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

    public class Environment
    {
        public int Time;
        public Cell[][] Cells;//jagged array is faster than matrix
        public Environment()
        {
             
        }

        void RunStep()
        {
            Time++;
            foreach (var arr in Cells)
            {
                foreach (var cell in arr)
                {
                    cell.Update();
                }
            }
        }
    }

    public class Cell : Thing
    {
        Environment _parent;

        public GridPoint Position { get; set; }
        public int Altitude { get; set; }
        /// <summary>
        /// We should consider wheter keep this minimal and theoretical or go all the way
        /// Here for example could put only one item per cell and don't allow anything else if we keep it minimal
        /// </summary>
        public List<Thing> Items { get; set; }

        public Cell(Environment parent, float x, float y)
        {
            _parent = parent;
            Position = new GridPoint(x, y);
            //store territory info in Thing.Properties
        }

        public override void Update()
        {

            foreach (var item in Items)
            {
                item.Update();
            }
        }
    }
}
