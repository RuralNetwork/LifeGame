using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGame.Things
{
    class Berry : Thing
    {
        public float Quantity { get; set; }
        public override float R
        {
            get { return 0.8f; }
        }

        public override float G
        {
            get { return 0; }
        }

        public override float B
        {
            get { return 0.8f; }
        }
        public override float Weight
        {
            get { return 0.001f * Quantity; }
        }
        public override float Temperature
        {
            get { return 0;/* simulation.State.Temperature;*/ }
        }

        public override float Speed { get { return 0; } }

        public override float Painful { get { return 0; } }



        public override float Amplitude { get { return 0; } }

        public override float Pitch { get { return 0; } }

        public override float SmellIntensity
        {
            get { throw new NotImplementedException(); }
        }

        public override float Smell
        {
            get { throw new NotImplementedException(); }
        }

        public Berry(Simulation environment)
            : base(environment)
        {
            Quantity = 100;
        }

        public override void Update()
        {
            throw new NotImplementedException();
        }

        public override void Draw()
        {
            throw new NotImplementedException();
        }
    }
}
