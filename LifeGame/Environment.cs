﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LifeGame
{
    

    public class Environment : Thing
    {
        public int Time;
        public Cell[][] Cells;//jagged array is faster than matrix
        public Environment()
        {

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

    public class Cell // I de-inherited it from Thing beacuse it doesn't have any perceptible property. We assume it as a mere container. It will contains the environment elements
    {
        Environment _parent;

        public GridPoint Location { get; set; }
        public int Altitude { get; set; }
        /// <summary>
        /// We should consider wheter keep this minimal and theoretical or go all the way
        /// Here for example could put only one item per cell and don't allow anything else if we keep it minimal
        /// </summary>
        public List<Thing> Items { get; set; }

        public Cell(Environment parent, int x, int y)
        {
            _parent = parent;
            Location = new GridPoint(x, y);
        }

        public void Update()
        {

            foreach (var item in Items)
            {
                item.Update();
            }
        }

        public void Draw()
        {
            foreach (var item in Items)
            {
                item.Draw();
            }
        }
    }
}
