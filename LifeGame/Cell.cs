using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    class HexGrid
    {
        //Prova modifica
    }

    class Cell
    {
        HexGrid parent;

        public readonly PointF Position { get; set; }
        public Terrain Terrain { get; set; }
        public int Altitude { get; set; }
        /// <summary>
        /// We should consider wheter keep this minimal and theoretical or go all the way
        /// Here for example could put only one item per cell and don't allow anything else if we keep it minimal
        /// </summary>
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
    //The traits of a being, should be a module
    class Trait{

    }
    class Being
    {
        public Being(PointF position, Trait parents)
        {

        }
    }
}
