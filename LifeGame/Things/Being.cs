using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace LifeGame
{
    public struct BeingState
    {
        public float Health;// can be healed
        public float Integrity; // cannot be healed
        public float Thirst;
        public float Hunger;
    }
    // The beings are contained in Cell.Items and they are moved to other cells when they do Walk action
    public class Being : Thing
    {
        public BeingState State { get; set; }

        Environment _environment;

        public GridPoint Location { get; set; }
        public Vector LastWalkDir { get; set; }
        public Thing CarriedObj { get; set; }

        public int ID { get; private set; }

        public Being(Environment environment, int id, GridPoint location)
        {
            _environment = environment;
            ID = id;
            Location = location;
        }

        public override void Update()
        {
            CarriedObj.Update();

            var inputs = new List<float>();
            var dir = LastWalkDir.Normalized;


            // run n cycles of his neural net
            // then do the choosen action + walk
        }

        public override void Draw()
        {
            CarriedObj.Draw();
            Debug.WriteLine("Drew %d\n", ID);
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

        public override float Moving// it depends on what was the magnitude of the previous vector of walk
        {
            get { throw new NotImplementedException(); }
        }

        public override float Painful //if it has fougth or tried to eat someone else
        {
            get { throw new NotImplementedException(); }
        }

        public override float Weight // the corpse? -> no it becomes a corpse thing, it doesn't perform actions
        {
            get { return 0; }
        }

        public override float Warmth
        {
            get { throw new NotImplementedException(); }
        }

        public override float Amplitude
        {
            get { throw new NotImplementedException(); }
        }

        public override float Pitch // could change with age or actions (eg mating call)
        {
            get { throw new NotImplementedException(); }
        }

        public override float SmellIntensity
        {
            get { throw new NotImplementedException(); }
        }

        public override float Smell
        {
            get { throw new NotImplementedException(); }
        }
    }
}
