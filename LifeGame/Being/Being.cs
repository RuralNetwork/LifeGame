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

        static int[] dirIdxs = new int[] { 4, 5, 3, 1, 0, 2 };

        public Being(Simulation simulation, GraphicsEngine engine, GridPoint location, Genome genome)
            : base(simulation, engine, location)
        {
            FitnessMean = new Average();
            Genome = genome;
            if (simulation.lastID == 4 * 10e9)
            {
                simulation.lastID = 0;
            }
            ID = simulation.lastID;
            simulation.lastID++;

            Brain.State[1] = Sex;
        }

        public override void Update(Thing container)
        {
            InnerThing.Update(this);


            //Aliases
            var terrain = Simulation.Terrain;
            var envProps = Simulation.Environment.Properties;
            var bState = Brain.State;
            var width = Simulation.GridWidth;
            var height = Simulation.GridHeight;
            var locX = Location.X;
            var locY = Location.Y;
            var ccProps = terrain[locX][locY].Properties;

            float f1;
            //       autoperception
            var i = 1;// 0=bias,  1=sex
            bState[++i] = Properties[ThingProperty.Height];
            bState[++i] = Properties[ThingProperty.Temperature];
            bState[++i] = Properties[ThingProperty.SmellIntensity];
            bState[++i] = Properties[ThingProperty.Smell1];
            bState[++i] = Properties[ThingProperty.Smell2];
            bState[++i] = Properties[ThingProperty.Smell3];
            bState[++i] = Health;
            bState[++i] = Integrity;
            bState[++i] = Thirst;
            bState[++i] = Hunger;
            bState[++i] = Wet;
            f1 = Direction.DirectionToAngle();
            bState[++i] = (float)Math.Sin(f1);
            bState[++i] = (float)Math.Cos(f1);

            //      carried object
            if (InnerThing != null)
            {
                bState[++i] = Properties[ThingProperty.Color1];
                bState[++i] = Properties[ThingProperty.Color2];
                bState[++i] = Properties[ThingProperty.Color3];
                bState[++i] = envProps[ThingProperty.Moving];
                bState[++i] = envProps[ThingProperty.Painful];
                bState[++i] = envProps[ThingProperty.Weigth];
                bState[++i] = envProps[ThingProperty.Temperature];
                bState[++i] = envProps[ThingProperty.Amplitude];
                bState[++i] = envProps[ThingProperty.Pitch];
                bState[++i] = envProps[ThingProperty.SmellIntensity];
                bState[++i] = envProps[ThingProperty.Smell1];
                bState[++i] = envProps[ThingProperty.Smell2];
                bState[++i] = envProps[ThingProperty.Smell3];
            }
            else
            {
                for (int j = 0; j < 13; j++)
                {
                    bState[++i] = 0;
                }
            }

            //    common useful variables initialization
            var results = new float[12];

            const int size = 5;//                      <-- change this
            var matrixY = new float[2 * size + 1][];// precalculate cartesian Y of cells, relative to current location; the X doesn't need conversions
            for (int x = -size; x <= size; x++)
            {
                matrixY[x + size] = new float[2 * size + 1];
                for (int y = -size; y <= size; y++)
                {
                    matrixY[x + size][y + size] = new GridPoint(locX + x, locY + y).CartesianY - Location.CartesianY;
                }
            }

            //       environment smell (current cell, near cells, environment)
            for (int x = -size; x <= size; x++)
            {
                var gridX = (locX + x).Cycle(width);
                for (int y = -size; y <= size; y++)
                {
                    var gridY = (locY + y).Cycle(height);
                    var d = (float)Math.Sqrt(x * x + matrixY[x + size][y + size] * matrixY[x + size][y + size]);
                    // function: x = 1 / (d + 1)
                    // this function is lightweight and i get what i want:
                    // for d=0, x=1
                    // for d>0, x is progressively smaller
                    results[0] += terrain[gridX][gridY].Properties[ThingProperty.Smell1] /
                        terrain[gridX][gridY].Properties[ThingProperty.SmellIntensity] / (d + 1);
                    results[1] += terrain[gridX][gridY].Properties[ThingProperty.Smell2] /
                        terrain[gridX][gridY].Properties[ThingProperty.SmellIntensity] / (d + 1);
                    results[2] += terrain[gridX][gridY].Properties[ThingProperty.Smell3] /
                        terrain[gridX][gridY].Properties[ThingProperty.SmellIntensity] / (d + 1);
                }
            }
            //add environment smell to results
            bState[++i] = results[0] + envProps[ThingProperty.Smell1] /
                envProps[ThingProperty.SmellIntensity];
            bState[++i] = results[1] + envProps[ThingProperty.Smell2] /
                envProps[ThingProperty.SmellIntensity];
            bState[++i] = results[2] + envProps[ThingProperty.Smell3] /
                envProps[ThingProperty.SmellIntensity];

            //       environment hearing(near cells)
            results[0] = 0f;
            results[1] = 0f;
            results[2] = 0f;
            for (int x = -size; x <= size; x++)
            {
                var gridX = (locX + x).Cycle(width);
                for (int y = -size; y <= size; y++)
                {
                    if (x != 0 || y != 0)
                    {
                        var gridY = (locY + y).Cycle(height);
                        var cartX = (float)x;
                        var cartY = matrixY[x + size][y + size];
                        var d = (float)Math.Sqrt(cartX * cartY + cartY * cartY);
                        //                                                               3 \5 / 4 
                        int flags = (cartY < -1.501f * cartX ? 1 : 0);//                ___ \/ ___
                        flags += (cartY < 1.499f * cartX ? 2 : 0);//                     1  /\  2
                        flags += (cartY < -0.001 * cartX ? 2 : 0);//                       /0 \ 
                        // il 0.001 aggiunto/sottratto serve per redistribuire equamente le celle divise esattamente a metà

                        results[flags * 2] = terrain[gridX][gridY].Properties[ThingProperty.Pitch] /
                            terrain[gridX][gridY].Properties[ThingProperty.Amplitude] / (d + 1);
                        results[flags * 2 + 1] = terrain[gridX][gridY].Properties[ThingProperty.Amplitude] / (d + 1);

                    }
                }
            }
            for (int j = 0; j < 12; j++)
            {
                bState[++i] = results[j];
            }

            //environment sight(near cells)             //  questa parte dovrebbe essere molto più complessa (per le occlusioni) ma per ora lascio così
            results = new float[18];
            for (int x = -size; x <= size; x++)
            {
                var gridX = (locX + x).Cycle(width);
                for (int y = -size; y <= size; y++)
                {
                    if (x != 0 || y != 0)
                    {
                        var gridY = (locY + y).Cycle(height);
                        var cartX = (float)x;
                        var cartY = matrixY[x + size][y + size];
                        var d = (float)Math.Sqrt(cartX * cartY + cartY * cartY);


                        int flags = (cartY < -1.501f * cartX ? 1 : 0);
                        flags += (cartY < 1.499f * cartX ? 2 : 0);
                        flags += (cartY < -0.001 * cartX ? 2 : 0);

                        results[flags * 3] = terrain[gridX][gridY].Properties[ThingProperty.Color1] /
                            terrain[gridX][gridY].Properties[ThingProperty.Alpha] / (d + 1);
                        results[flags * 3 + 1] = terrain[gridX][gridY].Properties[ThingProperty.Color2] /
                            terrain[gridX][gridY].Properties[ThingProperty.Alpha] / (d + 1);
                        results[flags * 3 + 2] = terrain[gridX][gridY].Properties[ThingProperty.Color3] /
                            terrain[gridX][gridY].Properties[ThingProperty.Alpha] / (d + 1);
                    }
                }
            }
            // cycle from rightmost to leftmost (excluding back direction), relative to current (forward) direction
            int dir = (Direction + 4) % 6;
            for (int j = 0; j < 5; j++)
            {
                bState[++i] = results[dirIdxs[dir] * 3];
                bState[++i] = results[dirIdxs[dir] * 3 + 1];
                bState[++i] = results[dirIdxs[dir] * 3 + 2];
                dir++;
                if (dir == 6) dir = 0;
            }

            //current cell
            bState[++i] = ccProps[ThingProperty.Color1];
            bState[++i] = ccProps[ThingProperty.Color2];
            bState[++i] = ccProps[ThingProperty.Color3];
            bState[++i] = ccProps[ThingProperty.Moving];
            bState[++i] = ccProps[ThingProperty.Painful];
            bState[++i] = ccProps[ThingProperty.Temperature];
            bState[++i] = ccProps[ThingProperty.Amplitude];
            bState[++i] = ccProps[ThingProperty.Pitch];

            //environment
            bState[++i] = envProps[ThingProperty.Color1];
            bState[++i] = envProps[ThingProperty.Color2];
            bState[++i] = envProps[ThingProperty.Color3];
            bState[++i] = envProps[ThingProperty.Painful];
            bState[++i] = envProps[ThingProperty.Temperature];

            Brain.Calculate();

            // choose action
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
            var dirVec = new Vector(Brain.Output[7].InverseSigmoid(), Brain.Output[8].InverseSigmoid());
            var mag = dirVec.Magnitude;
            var energy = Brain.Output[9].InverseSigmoid();
            switch (act)
            {
                case ActionType.Walk:
                    // last free cell
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
