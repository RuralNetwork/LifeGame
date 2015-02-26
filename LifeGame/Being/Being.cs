using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Media;

namespace LifeGame
{

    public class Phenome
    {
        public float Sex { get; private set; }
        public float HeightMul { get; private set; }//height multiplicator: assume that a being can grow during life, consider if we should change to static height
        public Color Color { get; set; }// switched to constant color during lifespan
    }

    public class Being : Thing
    {
        // mutable properties
        public float Health { get; set; }// can be healed
        public float Integrity { get; set; } // cannot be healed
        public float Thirst { get; set; }
        public float Hunger { get; set; }
        public float Wet { get; set; }

        // theese classes will be copied by reference in the next state, it's ok because other things can't modify them, so there will be no conflicts
        public Average FitnessMean { get; private set; }
        public Genome Genome { get; private set; }
        public NeuralNetwork Brain { get; private set; }
        public Phenome Phenome { get; private set; }


        public int Direction { get; private set; }

        public int ID { get; private set; }// this ID is used to display the beings

        public Being(Simulation simulation, Genome genome)
            : base(simulation)
        {
            FitnessMean = new Average();
            Genome = genome;
            if (simulation.lastID == 4 * 10e9)
            {
                simulation.lastID = 0;
            }
            ID = simulation.lastID;
            simulation.lastID++;
        }


        public override void Update(Thing container)//           INCOMPLETE
        {
            InnerThing.Update(this);



            Brain.Calculate();

            float max = 0;
            int n = 0;
            for (int i = 0; i < 7; i++)
            {
                if (Brain.Output[i] > max)
                {
                    max = Brain.Output[i];
                    n = i;
                }
            }
            var act = (ActionType)n;
            var dir = new Vector(Brain.Output[7].InverseSigmoid(), Brain.Output[8].InverseSigmoid());
            var mag = dir.Magnitude;
            var energy = Brain.Output[9].InverseSigmoid();
            switch (act)
            {
                case ActionType.Walk:
                    //if (energy<)
                    //{

                    //}
                    break;
                case ActionType.Sleep:
                    break;
                case ActionType.Eat:
                    break;
                case ActionType.Breed:
                    break;
                case ActionType.Fight:
                    break;
                case ActionType.Take:
                    break;
                case ActionType.Drop:
                    break;
                default:
                    break;
            }
            if (mag < 0)
            {

            }


        }

        public override void Draw(bool isCarriedObj = false)
        {
            if (InnerThing != null)
            {
                InnerThing.Draw(true);
            }
        }
    }
}
