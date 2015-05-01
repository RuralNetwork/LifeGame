using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Media;

namespace LifeGame
{
    [Serializable]
    public class Being : Thing
    {
        //consts:
        public const float CAL2KG = 0.42f;// kg lifted with one calorie  ->  1 / (1kg * 9.81m/s^2 * 1m)

        public int ID { get; set; }

        // mutable properties
        public CellDirection Direction { get; private set; }
        public int NCellsWalk { get; private set; }
        public GridPoint OldLoc { get; set; }
        public ActionType LastAction { get; private set; }
        public ActionType OldAction { get; private set; }
        public float EnergySpent;

        //immutable properties
        public bool Sex { get { return Brain.State[1] > 0f; } }
        //public float HeightMul { get; set; }//height multiplicator: assume that a being can grow during life, consider if we should change to static height

        public Fitness Fitness { get; private set; }
        public int Lifespan { get; private set; }
        public Genome Genome { get; set; }
        public NeuralNetwork Brain { get; set; }
        public List<int> LivingOffsprings { get; private set; }

        //need to identify whose the father and the mother 
        public int Father { get; set; }
        public int Mother { get; set; }

        static int[] dirIdxs = new int[] { 4, 5, 3, 1, 0, 2 };


        public Being()
            : base(ThingType.Being, default(GridPoint))
        {
            InnerThing = new Thing(ThingType.Null, default(GridPoint));
            InnerThing.IsCarrObj = true;
            LivingOffsprings = new List<int>();
        }

        public void InitOffspring(Genome genome)
        {
            Properties[(ThingProperty)BeingMutableProp.Energy] = 10000f;
            Properties[(ThingProperty)BeingMutableProp.Health] = 1;
            Properties[(ThingProperty)BeingMutableProp.Integrity] = 1f;
            Properties[(ThingProperty)BeingMutableProp.Hunger] = 1f;
            Properties[(ThingProperty)BeingMutableProp.Thirst] = 1f;
            Lifespan = 0;
            LivingOffsprings.Clear();
            Genome = genome;
            genome.Apply(this);
            Fitness = new Fitness();

            walk = new float[6];
        }

        float[] walk;

        public override void Update()
        {
            //Aliases
            var sim = Simulation.Instance;
            var terrain = sim.Terrain;
            var environment = sim.Environment;
            var bState = Brain.State;
            var width = GraphicsEngine.GRID_WIDTH;
            var height = GraphicsEngine.GRID_HEIGHT;
            var locX = Location.X;
            var locY = Location.Y;
            var ccProps = terrain[locX][locY].Properties;



            Lifespan++;
            if (sim.TrainingMode)
            {
                //var fitness = //Properties[(ThingProperty)BeingMutableProp.Health] + Properties[(ThingProperty)BeingMutableProp.Hunger] +
                //    //Properties[(ThingProperty)BeingMutableProp.Thirst] + //((float)Lifespan).Sigmoid() +
                //    (walk[0].Sigmoid() + walk[1].Sigmoid() + walk[2].Sigmoid() + walk[3].Sigmoid() + walk[4].Sigmoid() + walk[5].Sigmoid()) * 10;
                //foreach (var id in LivingOffsprings)
                //{
                //    fitness += sim.Population[id].Properties[(ThingProperty)BeingMutableProp.Health];
                //}
                //FitnessMean.Add(fitness);
                Fitness.Update(FitnessParam.Walk, (walk[0].Sigmoid() + walk[1].Sigmoid() + walk[2].Sigmoid() + walk[3].Sigmoid() + walk[4].Sigmoid() + walk[5].Sigmoid()) * 10);
                Fitness.Update(FitnessParam.BrainSize, ((float)(Brain.Links.Length + Brain.State.Length) / 20f).Sigmoid() * 10f);
                //Fitness.Update(FitnessParam.Health, Properties[(ThingProperty)BeingMutableProp.Health]);
                //Fitness.Update(FitnessParam.Reproduction, Properties[(ThingProperty)BeingMutableProp.Health]);

            }

            OldLoc = Location;
            OldAction = LastAction;
            NCellsWalk = 0;

            mute();
            rest();


            //================================================ INPUT ========================================================
            float f1;
            //       autoperception
            var i = 1;// 0=bias,  1=sex
            //bState[++i] = Properties[ThingProperty.Height];
            //bState[++i] = Properties[ThingProperty.Temperature];
            //bState[++i] = Properties[ThingProperty.SmellIntensity];
            //bState[++i] = Properties[ThingProperty.Smell1];
            //bState[++i] = Properties[ThingProperty.Smell2];
            //bState[++i] = Properties[ThingProperty.Smell3];
            bState[++i] = Properties[ThingProperty.Wet];
            bState[++i] = Properties[(ThingProperty)BeingMutableProp.Health];
            bState[++i] = Properties[(ThingProperty)BeingMutableProp.Integrity];
            bState[++i] = Properties[(ThingProperty)BeingMutableProp.Thirst];
            bState[++i] = Properties[(ThingProperty)BeingMutableProp.Hunger];
            f1 = Direction.DirectionToAngle();
            bState[++i] = (float)Math.Sin(f1);
            bState[++i] = (float)Math.Cos(f1);
            //f1 = sim.TimeTick / sim.Environment.DayTicks * 2 * (float)Math.PI;
            //bState[++i] = (float)Math.Sin(f1);
            //bState[++i] = (float)Math.Cos(f1);
            //f1 = sim.TimeTick / sim.Environment.YearTicks * 2 * (float)Math.PI;
            //bState[++i] = (float)Math.Sin(f1);
            //bState[++i] = (float)Math.Cos(f1);


            //      carried object
            //bState[++i] = InnerThing.Properties[ThingProperty.Color1];
            //bState[++i] = InnerThing.Properties[ThingProperty.Color2];
            //bState[++i] = InnerThing.Properties[ThingProperty.Color3];
            //bState[++i] = InnerThing.Properties[ThingProperty.Moving];
            //bState[++i] = InnerThing.Properties[ThingProperty.Painful];
            //bState[++i] = InnerThing.Properties[ThingProperty.Weigth];
            //bState[++i] = InnerThing.Properties[ThingProperty.Temperature];
            //bState[++i] = InnerThing.Properties[ThingProperty.Amplitude];
            //bState[++i] = InnerThing.Properties[ThingProperty.Pitch];
            //bState[++i] = InnerThing.Properties[ThingProperty.SmellIntensity];
            //bState[++i] = InnerThing.Properties[ThingProperty.Smell1];
            //bState[++i] = InnerThing.Properties[ThingProperty.Smell2];
            //bState[++i] = InnerThing.Properties[ThingProperty.Smell3];

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
            //apply results
            //bState[++i] = results[0];
            //bState[++i] = results[1];
            //bState[++i] = results[2];

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
                        var d = (float)Math.Sqrt(cartX * cartX + cartY * cartY);
                        //                                                               3 \5 / 4 
                        int flags = (cartY < -1.501f * cartX ? 1 : 0);//                ___ \/ ___
                        flags += (cartY < 1.499f * cartX ? 2 : 0);//                     1  /\  2
                        flags += (cartY < -0.001 * cartX ? 2 : 0);//                       /0 \ 
                        // il 0.001 aggiunto/sottratto serve per redistribuire equamente le celle divise esattamente a metà

                        results[flags * 2] += terrain[gridX][gridY].Properties[ThingProperty.Pitch] /
                            terrain[gridX][gridY].Properties[ThingProperty.Amplitude] / (d + 1);
                        results[flags * 2 + 1] += terrain[gridX][gridY].Properties[ThingProperty.Amplitude] / (d + 1);
                        if (float.IsNaN(results[flags * 2]))
                        {

                        }

                    }
                }
            }
            //for (int j = 0; j < 12; j++)
            //{
            //    bState[++i] = results[j];
            //}

            //environment sight(near cells)             //  questa parte dovrebbe essere molto più complessa (per le occlusioni) ma per ora lascio così
            results = new float[18];
            for (int x = -size; x <= size; x++)                    //                        manca il movimento!
            {
                var gridX = (locX + x).Cycle(width);
                for (int y = -size; y <= size; y++)
                {
                    if (x != 0 || y != 0)
                    {
                        var gridY = (locY + y).Cycle(height);
                        var cartX = (float)x;
                        var cartY = matrixY[x + size][y + size];
                        var d = (float)Math.Sqrt(cartX * cartX + cartY * cartY);


                        int flags = (cartY < -1.501f * cartX ? 1 : 0);
                        flags += (cartY < 1.499f * cartX ? 2 : 0);
                        flags += (cartY < -0.001 * cartX ? 2 : 0);

                        var k = terrain[gridX][gridY].Properties[ThingProperty.Alpha] *
                            (-1 / (terrain[gridX][gridY].Properties[ThingProperty.Height] + 1) + 1) / (d + 1);

                        results[flags * 3] += terrain[gridX][gridY].Properties[ThingProperty.Color1] * k;
                        results[flags * 3 + 1] += terrain[gridX][gridY].Properties[ThingProperty.Color2] * k;
                        results[flags * 3 + 2] += terrain[gridX][gridY].Properties[ThingProperty.Color3] * k;
                        if (float.IsNaN(results[flags * 3]))
                        {

                        }
                    }
                }
            }
            // cycle from rightmost to leftmost (excluding back direction), relative to current (forward) direction
            int dir = ((int)Direction + 4) % 6;
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
            //bState[++i] = ccProps[ThingProperty.Temperature];
            //bState[++i] = ccProps[ThingProperty.Amplitude];
            //bState[++i] = ccProps[ThingProperty.Pitch];

            //environment
            //bState[++i] = environment.Painful;
            //bState[++i] = environment.Temperature;
            //Debug.Write("Input count: " + i);

            Brain.Calculate();

            //================================================ OUTPUT ========================================================
            // choose action
            float max = 0;
            int n = 0;
            for (int j = 0; j < 7; j++)
            {
                if (bState[++i] > max)
                {
                    max = bState[i];
                    n = j;
                }
            }
            LastAction = (ActionType)n;
            var dirVec = new Vector(bState[++i].InverseSigmoid(), bState[++i].InverseSigmoid());
            var mag = dirVec.Magnitude;
            int tgtType = (mag < 0.5f ? 0 : (mag < 1f ? 1 : 2));//target type
            var energy = Math.Max(bState[++i], 0.5f);
            energy = Math.Min((energy - 0.5f) * 2f * Properties[(ThingProperty)BeingMutableProp.Energy], Properties[(ThingProperty)BeingMutableProp.Energy] / 5); //beings can spend at most a fifth of their total energy
            EnergySpent = energy;
            var ang = (float)Math.Atan2(-dirVec.Y, dirVec.X);
            if (ang < 0)
            {
                if (true)
                {
                    var a = 10;
                    a = a + 20;
                }
            }
            var cDir = (ang >= 0 ? ang : ang + (float)Math.PI * 2).AngleToDirection();
            if (ang > Math.PI)
            {
                var a = 10;
                a = a + 20;

            }
            if (cDir == CellDirection.TopRight)
            {
                var a = 10;
                a = a + 20;

            }
            if (cDir == CellDirection.BottomRight)
            {
                var a = 10;
                a = a + 20;

            }
            GridPoint cellPt = (tgtType == 2 ? GraphicsEngine.Cycle(Location.GetNearCell(cDir)) : Location);
            var target = terrain[cellPt.X][cellPt.Y];

            //debug: sovrascrivi questi
            //act = ActionType.Walk;
            //tgtType = 2;
            //EnergySpent = 100f;
            //cDir = CellDirection.Bottom;// cambia questo per cambiare scegliere la direzione

            switch (LastAction)
            {
                case ActionType.Walk:
                    if (tgtType == 2)
                    {
                        cellPt = Location;
                        while (EnergySpent > 0 && NCellsWalk < 3)
                        {
                            cellPt = GraphicsEngine.Cycle(cellPt.GetNearCell(cDir));
                            target = terrain[cellPt.X][cellPt.Y];
                            walkThrough(target);
                            if (target.InnerThing != null)
                            {
                                walkThrough(target.InnerThing);
                            }
                            NCellsWalk++;
                            //FitnessPoints++;
                        }
                        if (NCellsWalk > 0)
                        {
                            sim.BeingLocQueue.Add(new Tuple<Being, GridPoint>(this, cellPt));
                            int a = (int)cDir;

                            walk[a]++;
                        }
                    }
                    break;
                case ActionType.Sleep:
                    ChangeProp((ThingProperty)BeingMutableProp.Energy, 1000f * energy / Properties[(ThingProperty)BeingMutableProp.Energy], false);
                    energy = 0;// prevent loss of energy
                    //sleep++;
                    //interact(target, ActionType.Sleep);
                    break;
                case ActionType.Eat:
                    interact(target, LastAction);
                    energy = 0;
                    break;
                //case ActionType.Fight:
                //    //if (tgtType == 0)
                //    //{
                //    //    interact(InnerThing, act);
                //    //}
                //    interact(target, act);
                //    break;
                //case ActionType.Breed:
                //    cellPt = Location.GetNearCell(cDir);
                //    var being = (Being)terrain[(cellPt.X).Cycle(width)][(cellPt.Y).Cycle(height)].InnerThing;
                //    if (being != null && being.Sex != this.Sex)
                //    {
                //        interact(being, ActionType.Breed);
                //    }
                //    break;
                //case ActionType.MakeSound:
                //    ChangeProp(ThingProperty.Amplitude, 500 * energy, true);
                //    break;
                //case ActionType.Take:
                //    interact(target, act);
                //    break;
                //case ActionType.Drop:
                //    drop(target);
                //    InnerThing.ChangeType(ThingType.Null, null);
                //    energy = 0;
                //    break;
                default:
                    energy = 0;
                    break;
            }


            //debug

            // being changes
            Direction = cDir;
            ChangeProp((ThingProperty)BeingMutableProp.Energy, -energy - 10f, false);
            if (Properties[(ThingProperty)BeingMutableProp.Hunger] + Properties[(ThingProperty)BeingMutableProp.Hunger] > 1f)
            {
                ChangeProp((ThingProperty)BeingMutableProp.Energy, +100f, false);
            }
            ChangeProp((ThingProperty)BeingMutableProp.Hunger, -(energy + 10f) * 0.002f, false);
            ChangeProp((ThingProperty)BeingMutableProp.Thirst, -(energy + 10f) * 0.005f, false);
            ChangeProp((ThingProperty)BeingMutableProp.Health, ((Properties[(ThingProperty)BeingMutableProp.Hunger] + Properties[(ThingProperty)BeingMutableProp.Thirst]) / 2 - 0.2f) * 0.05f, false);
            ChangeProp((ThingProperty)BeingMutableProp.Integrity, -0.01f, false);

            if (Properties[(ThingProperty)BeingMutableProp.Health] > Properties[(ThingProperty)BeingMutableProp.Integrity])
            {
                ChangeProp((ThingProperty)BeingMutableProp.Health, Properties[(ThingProperty)BeingMutableProp.Integrity], true);
            }

            if (Properties[(ThingProperty)BeingMutableProp.Health] < 0.01f)
            {
                sim.MakeDie(this); //  FATALITY!!!
            }

            InnerThing.Update();

        }

        public override void Apply()
        {
            foreach (var prop in PropsQueueDelta)
            {
                Properties[prop.Key] += prop.Value;
                if (Properties[prop.Key] < 0) Properties[prop.Key] = 0;
            }

            foreach (var prop in PropsQueueReset)
            {
                Properties[prop.Key] = prop.Value;
            }

            PropsQueueDelta.Clear();
            PropsQueueReset.Clear();
            InnerThing.Apply();
        }

        public override void Draw(bool isCarriedObj = false)
        {
            InnerThing.Draw(true);
        }

        void interact(Thing target, ActionType action)
        {
            target.Interactions[action](target, this);
        }

        //=================== Behaviour functions =============
        void drop(Thing target)
        {
            // NOTE: although it seams not by these formulas, it will be not subtracted any energy
            var quantity = EnergySpent * CAL2KG / InnerThing.Properties[ThingProperty.Weigth];
            if (InnerThing.Properties[ThingProperty.Height] * InnerThing.Properties[ThingProperty.Alpha] * quantity >
                target.Properties[ThingProperty.Height] * target.Properties[ThingProperty.Alpha])
            {
                var dict = new Dictionary<ThingProperty, float>(InnerThing.Properties);
                dict[ThingProperty.Height] *= EnergySpent / InnerThing.Properties[ThingProperty.Weigth];
                dict[ThingProperty.Weigth] *= EnergySpent / InnerThing.Properties[ThingProperty.Weigth];
                target.ChangeType(InnerThing.Type, dict);
            }

            if (quantity > 1f)
            {
                InnerThing.ChangeType(ThingType.Null, null);
            }
            else
            {
                InnerThing.ChangeProp(ThingProperty.Height, -InnerThing.Properties[ThingProperty.Height] * quantity, false);
                InnerThing.ChangeProp(ThingProperty.Weigth, -InnerThing.Properties[ThingProperty.Weigth] * quantity, false);
            }
        }

        void walkThrough(Thing t)
        {
            EnergySpent -= t.Properties[ThingProperty.Height] / Properties[ThingProperty.Height] *
                (t.Properties[ThingProperty.Alpha] + Properties[ThingProperty.Alpha]) * 10f + Properties[ThingProperty.Weigth] / CAL2KG;
        }
    }
}
