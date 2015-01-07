using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LifeGame
{
    struct PointF
    {
        public float X { get; set; }
        public float Y { get; set; }
        public PointF(float x, float y)
        {
            X = x;
            Y = y;
        }
    }

    enum Terrain
    {
        Grass,
        Wood,
        River,
        Snow
    }

    class HexGrid : Grid
    {

    }

    class Cell
    {
        HexGrid parent;

        public readonly PointF Position { get; set; }
        public Terrain Terrain { get; set; }
        public int Altitude { get; set; }
        public List<Item> SubItems { get; set; }

        public Cell(HexGrid parent, float x, float y)
        {
            this.parent = parent;
            Position = new PointF(x, y);
        }
    }

    class Item
    {

    }
}
