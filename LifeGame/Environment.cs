﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Diagnostics;

namespace LifeGame
{

    public class Environment : Thing
    {
        public int Time;
        public Cell[][] Cells;//jagged array is faster than matrix

        public Environment(int gridWidth, int gridHeight)
        {
            Cells = new Cell[gridWidth][];
            for (int i = 0; i < gridHeight; i++)
			{
                Cells[i] = new Cell[gridHeight];

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

        public override float Moving
        {
            get { return 0; }
        }

        public override float Painful //if too cold
        {
            get { throw new NotImplementedException(); }
        }

        public override float Weight
        {
            get { return 0; }
        }

        public override float Warmth
        {
            get { throw new NotImplementedException(); }
        }

        public override float Amplitude // birds?
        {
            get { throw new NotImplementedException(); }
        }

        public override float Pitch // birds?
        {
            get { throw new NotImplementedException(); }
        }

        public override float SmellIntensity // if rainy?
        {
            get { throw new NotImplementedException(); }
        }

        public override float Smell // if rainy?
        {
            get { throw new NotImplementedException(); }
        }
    }

    public class Cell // I de-inherited it from Thing beacuse it doesn't have any perceptible property. We assume it as a mere container. It will contain the environment elements
    {
        Environment _parent;

        public GridPoint Location { get; set; }
        //Are you sure? If so, do we need a 3D model?
        public int Altitude { get; set; }
        public Thing Item { get; set; }
        /// <summary>
        /// Items count
        /// </summary>
        public int Count { get; set; }

        public Cell(Environment parent, int x, int y)
        {
            _parent = parent;
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
