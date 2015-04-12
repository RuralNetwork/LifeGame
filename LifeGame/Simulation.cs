using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LifeGame
{
    public class Event
    {
        // This class should contains time of the event, the message(actor, action), all the data from the previous n ticks and the data from the following n ticks
        // This class will manage also the UI, input and visualization
    }

    //This class can be instantiated multiple times to permit multiple simulations to be run at the same time
    public class Simulation
    {
        Stopwatch watch = Stopwatch.StartNew();

        public int lastID; // used and managed by the beings
        public int TimeTick { get; set; }

        /// <summary>
        /// Tells if should be used PopulationCount and the hall of fame genome list to maintain the population.
        /// </summary>
        public bool TrainingMode { get; set; }
        public int PopulationCount { get; set; }
        public bool IsRunning { get; private set; }
        public SimulationType Type { get; set; }
        /// <summary>
        /// History of events.
        /// </summary>
        public List<Event> Events { get; set; }

        Thread thread;
        GraphicsEngine engine;
        FastRandom rand = new FastRandom();

        public int GridWidth { get; private set; }// this must be an even number to permit the world to wrap around itself
        public int GridHeight { get; private set; }// this can be any number

        public SimEnvironment Environment { get; set; }
        //List<Genome> hallOfFame;
        public Genome BestGenome;

        public Thing[][] Terrain { get; set; }

        /// <summary>
        /// This is the update and draw speed.
        /// </summary>
        public float FPS = 2;

        public bool MustDraw = true;


        public List<Being>[][] BeingLocQueue { get; private set; }

        //Born: true
        //Died: false
        //Nothing: null
        public Tuple<bool, Being>[][] BornDiedQueue { get; private set; }

        /// <summary>
        /// Dictionary containing all beings that are currently alive.
        /// When a Being die, its object will be moved to freeBeingObjs.
        /// </summary>
        public Dictionary<int, Being> Population { get; private set; }

        /// <summary>
        /// Dictionary containing old beings whose properties (+ other stuff related, ex. brain, genome) 
        /// will be overwritten when new offspring will born; these Being objects will be moved to Population
        /// </summary>
        public List<Being> freeBeingObjs { get; private set; }


        public Simulation(int gridWidth, int gridHeight, GraphicsEngine engine)
        {
            this.engine = engine;
            thread = new Thread(Update);
            thread.IsBackground = true;
            thread.Priority = ThreadPriority.Highest;
            thread.Start();

            TrainingMode = true;
            PopulationCount = 100;
            //hallOfFame = new List<Genome>(10);

            Environment = new SimEnvironment(this, engine);
            //Ho messo questo controllo per la larghezza del mondo, ma non so neanche se tu vuoi che sia possibile farlo grande quanto si vuole
            //Not needed ATM if ((engine.hexaW * gridWidth) - ((gridWidth - 1) * 10) < engine.canvasWidth && (engine.hexaH * gridHeight) - ((gridHeight - 1) * 10) < engine.canvasHeight)
            //{
            GridWidth = gridWidth;
            GridHeight = gridHeight;

            Terrain = new Thing[gridWidth][];
            BeingLocQueue = new List<Being>[gridWidth][];
            BornDiedQueue = new Tuple<bool, Being>[gridWidth][];
            for (int i = 0; i < gridWidth; i++)
            {
                Terrain[i] = new Thing[gridHeight];
                BeingLocQueue[i] = new List<Being>[gridHeight];
                BornDiedQueue[i] = new Tuple<bool, Being>[gridHeight];
                for (int j = 0; j < gridHeight; j++)
                {
                    BeingLocQueue[i][j] = new List<Being>();
                    Terrain[i][j] = new Thing(ThingType.Earth, this, engine, new GridPoint(i, j));// per metto Earth per ogni cella
                }
            }
            Population = new Dictionary<int, Being>();
            freeBeingObjs = new List<Being>();
            for (int i = 0; i < PopulationCount * 1.5f; i++)
            {
                var being = new Being(this, engine);
                freeBeingObjs.Add(being);
            }
            // }
            /*else
            {
                Debug.Write("Non bene");
                //engine.messageBox(null, null, 200, 20, "Mondo troppo largo, il massimo è:engine.canvasWidth/(engine.hexaW)+((canvasWidth/(engine.hexaW)-1))");
            }*/
            //Populate the terrain


        }
        bool popCreated;
        public void TogglePause()
        {
            if (!popCreated)
            {
                CreatePopulation();
            }
            IsRunning = !IsRunning;
        }

        //This must be at a fixed rate, so the rate is the one defined by the user
        public void Update()
        {
            //debug
            Type = SimulationType.Fast;
            while (true)
            {
                while (IsRunning)
                {
                    if (Type == SimulationType.Fast || watch.Elapsed.TotalSeconds > 1 / FPS)
                    {
                        Debug.WriteLine("Fps: " + (1 / (float)watch.Elapsed.TotalSeconds).ToString("0.0"));
                        watch.Restart();

                        TimeTick++;

                        Environment.Update();

                        int idx = 0;
#if DEBUG // change to release to execute parallel code
                        foreach (var arr in Terrain)
                        {
                            foreach (var thing in arr)
                            {
                                thing.Update();
                            }
                        }

                        foreach (var being in Population)
                        {
                            being.Value.Update();
                        }

                        foreach (var being in Population)
                        {
                            being.Value.Apply();
                        }

                        for (int x = 0; x < GridWidth; x++)// qui c'è qualche problema di incongruenza temporale, che è troppo lungo da correggere adesso
                        {
                            for (int y = 0; y < GridHeight; y++)
                            {
                                var beingList = BeingLocQueue[x][y];
                                if (beingList.Count > 0)
                                {

                                    var biggerBeing = beingList[0];
                                    idx = 0;
                                    for (int i = 1; i < beingList.Count; i++)
                                    {
                                        if (beingList[i].Properties[ThingProperty.Weigth] > biggerBeing.Properties[ThingProperty.Weigth])
                                        {
                                            biggerBeing = beingList[i];
                                            idx = i;
                                        }
                                    }
                                    beingList.RemoveAt(idx);
                                    Terrain[biggerBeing.Location.X][biggerBeing.Location.Y].InnerThing = null;

                                    var loc = new GridPoint(x, y);
                                    biggerBeing.Location = loc;
                                    Terrain[x][y].InnerThing = biggerBeing;
                                    foreach (var being in beingList)
                                    {
                                        var newLoc = loc;
                                        Thing newCell;
                                        do
                                        {
                                            newLoc = newLoc.GetNearCell();
                                            newCell = Terrain[newLoc.X][newLoc.Y];
                                        } while (newCell.InnerThing != null || BeingLocQueue[newLoc.X][newLoc.Y].Count != 0);
                                        newCell.InnerThing = being;
                                        Terrain[being.Location.X][being.Location.Y].InnerThing = null;
                                        being.Location = newLoc;
                                    }
                                    beingList.Clear();
                                }
                            }
                        }

                        // make born some beings
                        if (TrainingMode)
                        {
                            while (PopulationCount < PopulationCount)
                            {
                                Thing cell;
                                do
                                {
                                    cell = Terrain[rand.Next(GridWidth)][rand.Next(GridHeight)];
                                } while (cell.InnerThing != null || cell.Type == ThingType.Mountain || cell.Type == ThingType.Water);

                                var being = freeBeingObjs.Last();
                                freeBeingObjs.RemoveAt(freeBeingObjs.Count - 1);
                                being.OldLoc = cell.Location;
                                being.Father = null;
                                being.Mother = null;
                            }
                        }

                        // make die some beings
                        while (Population.Count > PopulationCount)
                        {
                            idx = rand.Next(Population.Count);
                            var being = Population.ElementAt(idx).Value;
                            var loc = being.Location;
                            BornDiedQueue[loc.X][loc.Y] = new Tuple<bool, Being>(false, being);
                        }


                        // apply births/deaths
                        for (int x = 0; x < GridWidth; x++)
                        {
                            for (int y = 0; y < GridHeight; y++)
                            {
                                var tuple = BornDiedQueue[x][y];
                                if (tuple != null)
                                {

                                    if (tuple.Item1)
                                    {
                                        var cell = Terrain[x][y];
                                        var being = tuple.Item2;
                                        Population.Add(being.ID, being);
                                        cell.InnerThing = being;
                                        being.Location = cell.Location;
                                        being.InitOffspring(new Genome(BestGenome));

                                        ///////////////////////////aggiungi qui codice per mostrare il being (e il suo innerthing)//////////////////////
                                    }
                                    else
                                    {
                                        var cell = Terrain[x][y];
                                        var being = tuple.Item2;
                                        Population.Remove(being.ID);
                                        freeBeingObjs.Add(being);
                                        being.Father.LivingOffsprings.Remove(being);//find a faster way to search it (indexing)
                                        being.Mother.LivingOffsprings.Remove(being);//find a faster way to search it (indexing)
                                        if (being.Sex)
                                        {
                                            foreach (var offspring in being.LivingOffsprings)
                                            {
                                                offspring.Father = null;
                                            }
                                        }
                                        else
                                        {

                                            foreach (var offspring in being.LivingOffsprings)
                                            {
                                                offspring.Mother = null;
                                            }
                                        }

                                        if (Thing.BiggerBetween(being, cell))
                                        {
                                            var dict = new Dictionary<ThingProperty, float>();
                                            for (int i = 0; i < Thing.nThingProps; i++)
                                            {
                                                dict.Add((ThingProperty)i, being.Properties[(ThingProperty)i]);
                                            }
                                            cell.ChangeType(ThingType.Corpse, dict);
                                        }

                                        //check if is best genome
                                        if (being.FitnessMean.Value > BestGenome.Fitness)
                                        {
                                            BestGenome = being.Genome;
                                            BestGenome.Fitness = being.FitnessMean.Value;
                                        }

                                        ///////////////////////////aggiungi qui codice per nascondere il being (e il suo innerthing)//////////////////////
                                    }
                                    BornDiedQueue[x][y] = null;
                                }
                            }
                        }


                        foreach (var arr in Terrain)
                        {
                            foreach (var thing in arr)
                            {
                                thing.Apply();
                            }
                        }

#else
                        //       Rifare questa parte
                        //Parallel.ForEach(Terrain, arr =>
                        //{
                        //    foreach (var thing in arr) thing.Update();
                        //});

                        //Parallel.ForEach(Population, being => being.Update());

                        //Parallel.ForEach(Terrain, arr =>
                        //{
                        //    foreach (var thing in arr) thing.Apply();
                        //});

                        //Parallel.ForEach(Population, being => being.Apply());

                        //Parallel.For(0, BeingLocQueue.Length, x =>
                        //{
                        //    for (int y = 0; y < BeingLocQueue[x].Length; y++)
                        //    {
                        //        var beingList = BeingLocQueue[x][y];
                        //        if (beingList.Count > 0)
                        //        {

                        //            var biggerBeing = beingList[0];
                        //            idx = 0;
                        //            for (int i = 1; i < beingList.Count; i++)
                        //            {
                        //                if (beingList[i].Properties[ThingProperty.Weigth] > biggerBeing.Properties[ThingProperty.Weigth])
                        //                {
                        //                    biggerBeing = beingList[i];
                        //                    idx = i;
                        //                }
                        //            }
                        //            beingList.RemoveAt(idx);
                        //            Terrain[biggerBeing.Location.X][biggerBeing.Location.Y].InnerThing = null;

                        //            var loc = new GridPoint(x, y);
                        //            biggerBeing.Location = loc;
                        //            Terrain[x][y].InnerThing = biggerBeing;
                        //            foreach (var being in beingList)
                        //            {
                        //                var newLoc = loc;
                        //                Thing newCell;
                        //                do
                        //                {
                        //                    newLoc = newLoc.GetNearCell();
                        //                    newCell = Terrain[newLoc.X][newLoc.Y];
                        //                } while (newCell.InnerThing != null || BeingLocQueue[newLoc.X][newLoc.Y].Count != 0);
                        //                newCell.InnerThing = being;
                        //                Terrain[being.Location.X][being.Location.Y].InnerThing = null;
                        //                being.Location = newLoc;
                        //            }
                        //            beingList.Clear();
                        //        }
                        //    }
                        //});

#endif
                        //Draw
                        if (MustDraw)
                        {
                            Environment.Draw();
                            foreach (var arr in Terrain)
                            {
                                foreach (var thing in arr)
                                {
                                    thing.Draw();
                                }
                            }
                        }

                    }
                }
            }
        }

        void CreatePopulation()
        {
            for (int i = 0; i < PopulationCount; i++)
            {
                Thing cell;
                do
                {
                    cell = Terrain[rand.Next(GridWidth)][rand.Next(GridHeight)];
                } while (cell.InnerThing != null || cell.Type == ThingType.Mountain || cell.Type == ThingType.Water);

                var being = freeBeingObjs.Last();
                Population.Add(being.ID, being);
                cell.InnerThing = being;
                freeBeingObjs.RemoveAt(freeBeingObjs.Count - 1);
                being.InitOffspring(new Genome(this));
                being.Location = cell.Location;
            }
            popCreated = true;
        }
        //public void Draw()
        //{
        //    foreach (var arr in Terrain)
        //    {
        //        foreach (var thing in arr)
        //        {
        //            thing.Draw();
        //        }
        //    }

        //}
    }
}
