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
        public SimEnvironment(Simulation simulation)
            : base(simulation)
        {

        }

        public override void Update()
        {
        }

        public override void Draw()
        {

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

        public override float Amplitude // can be a thunder
        {
            get { throw new NotImplementedException(); }
        }

        public override float Pitch  // can be a thunder
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

        public override float Speed
        {
            get { return 0; }
        }

        public override float Temperature
        {
            get { throw new NotImplementedException(); }
        }
    }

}
