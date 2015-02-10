using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Diagnostics;

namespace LifeGame
{
    public class SimEnvironment : Thing
    {
        public int Time { get; set; }
        public Cell[][] Cells { get; private set; }

        public List<Being> Population { get; private set; }

        public SimEnvironment(int gridWidth, int gridHeight, GraphicsEngine engine)
            : base(null)
        {
            Cells = new Cell[gridHeight][];
            for (int i = 0; i < gridHeight; i++)
            {
                Cells[i] = new Cell[gridWidth];
                for (var j = 0; j < gridWidth; j++)
                {
                    Cells[i][j] = new Cell(i, j, this, engine);
                }
            }
        }

        public override void Update()
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

        public override void Draw()
        {
            //So each game-tick you just need to call this draw, every cell is updated
            foreach (var arr in Cells)
            {
                foreach (var cell in arr)
                {
                    cell.Draw();
                }
            }

        }

        public override float R
        {
            get { throw new NotImplementedException(); }
        }

        public override float G
        {
            get { throw new NotImplementedException(); }
        }

        public override float B
        {
            get { throw new NotImplementedException(); }
        }

        public override float Painful //if too cold
        {
            get { throw new NotImplementedException(); }
        }

        public override float Weight
        {
            get { return 0; }
        }

        public override float Amplitude // birds?  -> no, useless
        {
            get { return 0; }
        }

        public override float Pitch // birds?  -> no, useless
        {
            get { return 0; }
        }

        public override float SmellIntensity // if rainy?
        {
            get { throw new NotImplementedException(); }
        }

        public override float Smell // if rainy?
        {
            get { throw new NotImplementedException(); }
        }

        public override float Speed
        {
            get { throw new NotImplementedException(); }
        }

        public override float Temperature
        {
            get { throw new NotImplementedException(); }
        }
    }

    public class Cell
    {
        SimEnvironment environment;

        public GridPoint Location { get; set; }
        //Are you sure? If so, do we need a 3D model?
        //No, just overlayed bitmaps, but then the system for selecting the cells with mouse must consider the altitude (and it's not so straight forward)
        public int Altitude { get; set; }
        public Thing Item { get; set; }
        /// <summary>
        /// Items count
        /// </summary>
        public int Count { get; set; }

        public Cell(int x, int y, SimEnvironment environment, GraphicsEngine engine)
        {
            this.environment = environment;
            Location = new GridPoint(x, y);
        }

        public void Update()
        {
            Item.Update();
        }

        public void Draw()
        {
            //Item.Draw(); Commented for testing draw
            Debug.Write("Drew cell\n");
        }
    }
}
