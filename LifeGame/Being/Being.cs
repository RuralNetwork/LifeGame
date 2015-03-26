using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Media;

namespace LifeGame
{

    public class Being : Thing
    {
        // mutable properties
        public int Direction { get; private set; }
        public float Health { get; set; }// can be healed
        public float Integrity { get; set; } // cannot be healed
        public float Thirst { get; set; }
        public float Hunger { get; set; }
        public float Wet { get; set; }
        public float Sex { get; private set; }
        public float HeightMul { get; private set; }//height multiplicator: assume that a being can grow during life, consider if we should change to static height

        // theese classes will be copied by reference in the next state, it's ok because other things can't modify them, so there will be no conflicts
        public Average FitnessMean { get; private set; }
        public Genome Genome { get; private set; }
        public NeuralNetwork Brain { get; private set; }



        public int ID { get; private set; }// this ID is used to display the beings

        public Being(Simulation simulation, GraphicsEngine engine, GridPoint location, Genome genome)
            : base(simulation, engine, location)
        {
            _simulazione = simulation;
            FitnessMean = new Average();
            Genome = genome;
            if (simulation.lastID == 4 * 10e9)
            {
                simulation.lastID = 0;
            }
            ID = simulation.lastID;
            simulation.lastID++;

            Brain.State[1] = Sex; //               
        }

        public Simulation _simulazione;

        public override void Update(Thing container)//           INCOMPLETE
        {
            InnerThing.Update(this);

            float f1;
            //       autoperception
            var i = 1;// 0=bias,  1=sex
            Brain.State[++i] = Properties[ThingProperty.Height];
            Brain.State[++i] = Properties[ThingProperty.Temperature];
            Brain.State[++i] = Properties[ThingProperty.SmellIntensity];
            Brain.State[++i] = Properties[ThingProperty.Smell1];
            Brain.State[++i] = Properties[ThingProperty.Smell2];
            Brain.State[++i] = Properties[ThingProperty.Smell3];
            Brain.State[++i] = Health;
            Brain.State[++i] = Integrity;
            Brain.State[++i] = Thirst;
            Brain.State[++i] = Hunger;
            Brain.State[++i] = Wet;
            f1 = Direction.DirectionToAngle();
            Brain.State[++i] = (float)Math.Sin(f1);
            Brain.State[++i] = (float)Math.Cos(f1);

            //      carried object
            if (InnerThing != null)
            {
                Brain.State[++i] = Properties[ThingProperty.Color1];
                Brain.State[++i] = Properties[ThingProperty.Color2];
                Brain.State[++i] = Properties[ThingProperty.Color3];
                Brain.State[++i] = Simulation.Environment.Properties[ThingProperty.Moving];
                Brain.State[++i] = Simulation.Environment.Properties[ThingProperty.Painful];
                Brain.State[++i] = Simulation.Environment.Properties[ThingProperty.Weigth];
                Brain.State[++i] = Simulation.Environment.Properties[ThingProperty.Temperature];
                Brain.State[++i] = Simulation.Environment.Properties[ThingProperty.Amplitude];
                Brain.State[++i] = Simulation.Environment.Properties[ThingProperty.Pitch];
                Brain.State[++i] = Simulation.Environment.Properties[ThingProperty.SmellIntensity];
                Brain.State[++i] = Simulation.Environment.Properties[ThingProperty.Smell1];
                Brain.State[++i] = Simulation.Environment.Properties[ThingProperty.Smell2];
                Brain.State[++i] = Simulation.Environment.Properties[ThingProperty.Smell3];
            }
            else
            {
                for (int j = 0; j < 13; j++)
                {
                    Brain.State[++i] = 0;
                }
            }

            //    common useful variables initialization

            var result1 = 0f;
            var result2 = 0f;
            var result3 = 0f;
            var result4 = 0f;
            var result5 = 0f;
            var result6 = 0f;
            var result7 = 0f;
            var result8 = 0f;
            var result9 = 0f;
            var result10 = 0f;
            var result11 = 0f;
            var result12 = 0f;
            const int size = 5;//                      <-- change this
            var width = Simulation.GridWidth;
            var height = Simulation.GridHeight;
            var matrixY = new float[2 * size + 1][];// precalculate cartesian Y of cells, relative to current location; the X doesn't need conversions
            for (int x = -size; x <= size; x++)
            {
                matrixY[x + size] = new float[2 * size + 1];
                for (int y = -size; y <= size; y++)
                {
                    matrixY[x + size][y + size] = new GridPoint(Location.X + x, Location.Y + y).CartesianY - Location.CartesianY;
                }
            }

            //       environment smell (current cell, near cells, environment)
            for (int x = -size; x <= size; x++)
            {
                var gridX = (Location.X + x).Cycle(width);
                for (int y = -size; y <= size; y++)
                {
                    var gridY = (Location.Y + y).Cycle(height);
                    var d = (float)Math.Sqrt(x * x + matrixY[x + size][y + size] * matrixY[x + size][y + size]);
                    // function: x = 1 / (d + 1)
                    // this function is lightweight and i get what i want:
                    // for d=0, x=1
                    // for d>0, x is progressively smaller
                    result1 += Simulation.Terrain[gridX][gridY].Properties[ThingProperty.Smell1] /
                        Simulation.Terrain[gridX][gridY].Properties[ThingProperty.SmellIntensity] / (d + 1);
                    result2 += Simulation.Terrain[gridX][gridY].Properties[ThingProperty.Smell2] /
                        Simulation.Terrain[gridX][gridY].Properties[ThingProperty.SmellIntensity] / (d + 1);
                    result3 += Simulation.Terrain[gridX][gridY].Properties[ThingProperty.Smell3] /
                        Simulation.Terrain[gridX][gridY].Properties[ThingProperty.SmellIntensity] / (d + 1);
                }
            }
            //add environment smell to results
            result1 += Simulation.Environment.Properties[ThingProperty.Smell1] /
                Simulation.Environment.Properties[ThingProperty.SmellIntensity];
            result2 += Simulation.Environment.Properties[ThingProperty.Smell2] /
                Simulation.Environment.Properties[ThingProperty.SmellIntensity];
            result3 += Simulation.Environment.Properties[ThingProperty.Smell3] /
                Simulation.Environment.Properties[ThingProperty.SmellIntensity];

            Brain.State[++i] = result1;
            Brain.State[++i] = result2;
            Brain.State[++i] = result3;

            //       environment hearing(current cell, near cells, environment)
            result1 = 0f;
            result2 = 0f;
            result3 = 0f;


            //current cell

            Brain.State[++i] = Properties[ThingProperty.Color1];
            Brain.State[++i] = Properties[ThingProperty.Color2];
            Brain.State[++i] = Properties[ThingProperty.Color3];
            Brain.State[++i] = Simulation.Environment.Properties[ThingProperty.Moving];
            Brain.State[++i] = Simulation.Environment.Properties[ThingProperty.Painful];
            Brain.State[++i] = Simulation.Environment.Properties[ThingProperty.Temperature];
            Brain.State[++i] = Simulation.Environment.Properties[ThingProperty.Amplitude];
            Brain.State[++i] = Simulation.Environment.Properties[ThingProperty.Pitch];





            //environment
            Brain.State[++i] = Simulation.Environment.Properties[ThingProperty.Color1];
            Brain.State[++i] = Simulation.Environment.Properties[ThingProperty.Color2];
            Brain.State[++i] = Simulation.Environment.Properties[ThingProperty.Color3];
            Brain.State[++i] = Simulation.Environment.Properties[ThingProperty.Painful];
            Brain.State[++i] = Simulation.Environment.Properties[ThingProperty.Temperature];

            Brain.Calculate();

            float max = 0;
            int n = 0;
            for (i = 0; i < 7; i++)
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


            if (InnerThing != null)
            {
                InnerThing.Update();
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
