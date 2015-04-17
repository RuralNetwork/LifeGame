using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Threading;

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
        public DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Background);
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


        public List<Tuple<Being, GridPoint>> BeingLocQueue { get; private set; }

        //Born: 1° being=null
        //Died: 1° being!=null
        public Tuple<Being, Genome, Being, Being>[][] BornDiedQueue { get; private set; }

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
            Type = SimulationType.Fast; // fare in modo che si possa cambiare

            timer.Tick += Update;
            // timer.Interval = new TimeSpan(0, 0, 0, 0, 500);

            this.engine = engine;
            //thread = new Thread(Update);
            //thread.IsBackground = true;
            //thread.Priority = ThreadPriority.Highest;
            //thread.Start();

            TrainingMode = true;
            PopulationCount = 20;
            //hallOfFame = new List<Genome>(10);

            Environment = new SimEnvironment(this, engine);
            //Ho messo questo controllo per la larghezza del mondo, ma non so neanche se tu vuoi che sia possibile farlo grande quanto si vuole
            //Not needed ATM if ((engine.hexaW * gridWidth) - ((gridWidth - 1) * 10) < engine.canvasWidth && (engine.hexaH * gridHeight) - ((gridHeight - 1) * 10) < engine.canvasHeight)
            //{
            GridWidth = gridWidth;
            GridHeight = gridHeight;

            Terrain = new Thing[gridWidth][];
            BeingLocQueue = new List<Tuple<Being, GridPoint>>();
            BornDiedQueue = new Tuple<Being, Genome, Being, Being>[gridWidth][];
            for (int i = 0; i < gridWidth; i++)
            {
                Terrain[i] = new Thing[gridHeight];
                BornDiedQueue[i] = new Tuple<Being, Genome, Being, Being>[gridHeight];
                for (int j = 0; j < gridHeight; j++)
                {
                    Terrain[i][j] = new Thing(ThingType.Grass, this, engine, new GridPoint(i, j));// per metto Earth per ogni cella
                }
            }
            Population = new Dictionary<int, Being>();
            freeBeingObjs = new List<Being>();
            for (int i = 0; i < PopulationCount * 1.5f; i++)
            {
                var being = new Being(this, engine);
                freeBeingObjs.Add(being);
            }
            BestGenome = new Genome(this);
            // }
            /*else
            {
                Debug.Write("Non bene");
                //engine.messageBox(null, null, 200, 20, "Mondo troppo largo, il massimo è:engine.canvasWidth/(engine.hexaW)+((canvasWidth/(engine.hexaW)-1))");
            }*/
            //Populate the terrain


        }

        bool started;
        public void TogglePause()
        {
            if (!started)
            {
                timer.Start();
                started = true;
            }
            IsRunning = !IsRunning;
        }

        public float ActualFPS;

        //This must be at a fixed rate, so the rate is the one defined by the user
        public void Update(object sender, EventArgs e)
        {
            if (IsRunning)
            {
                if (Type == SimulationType.Fast || watch.Elapsed.TotalSeconds > 1 / FPS)
                {
                    ActualFPS = 1 / (float)watch.Elapsed.TotalSeconds;

                    Debug.Write("Fps: " + ActualFPS.ToString("0.0"));
                    Debug.Write(" Population.Count: " + Population.Count.ToString("00"));
                    Debug.Write(" Free Beings: " + freeBeingObjs.Count.ToString("00"));
                    if (Population.Count > 0)
                    {
                        Debug.Write("    Population[0]:  Health: " + Population.ElementAt(0).Value.Properties[(ThingProperty)BeingMutableProp.Health].ToString("0.00"));
                    }
                    Debug.Write("    best fitness: " + BestGenome.Fitness.ToString("0.00"));
                    int c = 0;
                    for (int x = 0; x < GridWidth; x++)
                    {
                        for (int y = 0; y < GridHeight; y++)
                        {
                            if (Terrain[x][y].InnerThing != null)
                            {
                                c++;
                            }
                        }
                    }
                    Debug.Write("    " + c);

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

                    foreach (var arr in Terrain)
                    {
                        foreach (var thing in arr)
                        {
                            thing.Apply();
                        }
                    }

                    foreach (var being in Population)
                    {
                        being.Value.Apply();
                    }


                    // make born some beings
                    if (TrainingMode)
                    {
                        for (int i = 0; i < PopulationCount - Population.Count; i++)
                        {
                            Thing cell;
                            int x, y;
                            do
                            {
                                x = rand.Next(GridWidth);
                                y = rand.Next(GridHeight);
                                cell = Terrain[x][y];
                            } while (cell.InnerThing != null || cell.Type == ThingType.Mountain || cell.Type == ThingType.Water || BornDiedQueue[x][y] != null);

                            GiveBirth(BestGenome, new GridPoint(x, y));
                        }
                    }

                    // make die some beings
                    for (int i = 0; i < Population.Count - PopulationCount; i++)
                    {
                        idx = rand.Next(Population.Count);
                        var being = Population.ElementAt(idx).Value;
                        MakeDie(being);
                    }


                    // apply births/deaths
                    for (int x = 0; x < GridWidth; x++)
                    {
                        for (int y = 0; y < GridHeight; y++)
                        {
                            var tuple = BornDiedQueue[x][y];
                            if (tuple != null)
                            {

                                if (tuple.Item1 == null)
                                {
                                    var cell = Terrain[x][y];
                                    var being = freeBeingObjs.Last();
                                    cell.InnerThing = being;

                                    freeBeingObjs.RemoveAt(freeBeingObjs.Count - 1); // remove form freeBeingObjs
                                    Population.Add(being.ID, being);                 // add to Population

                                    being.Location = cell.Location;
                                    being.OldLoc = cell.Location;
                                    if (tuple.Item3 != null)
                                    {
                                        being.Father = tuple.Item3.ID;
                                        being.Mother = tuple.Item4.ID;
                                        Population[being.Father].LivingOffsprings.Add(being.ID);
                                        Population[being.Mother].LivingOffsprings.Add(being.ID);
                                    }
                                    else
                                    {
                                        being.Father = -1;
                                        being.Mother = -1;
                                    }

                                    being.InitOffspring(tuple.Item2);

                                    ///////////////////////////aggiungi qui codice per mostrare il being (e il suo innerthing)//////////////////////
                                    engine.addBeing(being, being.Location);
                                }
                                else
                                {
                                    var cell = Terrain[x][y];
                                    var being = tuple.Item1;
                                    cell.InnerThing = null;

                                    Population.Remove(being.ID); // remove form Population               // here it's the only place I need being IDs
                                    freeBeingObjs.Add(being);    // add to freeBeingObjs

                                    if (being.Father != -1)
                                    {
                                        Population[being.Father].LivingOffsprings.Remove(being.ID);//find a faster way to search it (indexing)
                                    }
                                    if (being.Mother != -1)
                                    {
                                        Population[being.Mother].LivingOffsprings.Remove(being.ID);//find a faster way to search it (indexing)
                                    }

                                    if (being.Sex)
                                    {
                                        foreach (var offspringID in being.LivingOffsprings)
                                        {
                                            Population[offspringID].Father = -1;
                                        }
                                    }
                                    else
                                    {

                                        foreach (var offspringID in being.LivingOffsprings)
                                        {
                                            Population[offspringID].Mother = -1;
                                        }
                                    }

                                    //if (Thing.BiggerBetween(being, cell))
                                    //{
                                    //    var dict = new Dictionary<ThingProperty, float>();
                                    //    for (int i = 0; i < Thing.nThingProps; i++)
                                    //    {
                                    //        dict.Add((ThingProperty)i, being.Properties[(ThingProperty)i]);
                                    //    }
                                    //    cell.ChangeType(ThingType.Corpse, dict);
                                    //}

                                    //check if is best genome
                                    if (being.FitnessMean.Value > BestGenome.Fitness)
                                    {
                                        BestGenome = being.Genome;
                                        BestGenome.Fitness = being.FitnessMean.Value;
                                    }

                                    ///////////////////////////aggiungi qui codice per nascondere il being (e il suo innerthing)//////////////////////
                                    engine.removeBeing(being);
                                }
                                BornDiedQueue[x][y] = null;
                            }
                        }
                    }

                    // move beings
                    while (BeingLocQueue.Count > 0)
                    {
                        idx = rand.Next(BeingLocQueue.Count);
                        var tuple = BeingLocQueue[idx];
                        var being = tuple.Item1;
                        if (Population.ContainsKey(being.ID))
                        {
                            var newLoc = tuple.Item2;
                            Thing newCell = Terrain[newLoc.X][newLoc.Y];
                            while (newCell.InnerThing != null)
                            {
                                newLoc = newLoc.GetNearCell();
                                newLoc.X = newLoc.X.Cycle(GridWidth);
                                newLoc.Y = newLoc.Y.Cycle(GridHeight);
                                newCell = Terrain[newLoc.X][newLoc.Y];
                            }
                            newCell.InnerThing = being;
                            Terrain[being.Location.X][being.Location.Y].InnerThing = null;
                            being.Location = newLoc;
                            engine.changeBeing(being); //             <-----chiamata all'engine
                        }
                        else
                        {

                        }
                        BeingLocQueue.RemoveAt(idx);
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
                    Debug.WriteLine(null);
                }
            }
        }


        public void GiveBirth(Being dad, Being mum, GridPoint location)
        {
            BornDiedQueue[location.X][location.Y] = new Tuple<Being, Genome, Being, Being>(null, new Genome(dad.Genome, mum.Genome), dad, mum);
        }
        public void GiveBirth(Genome oldGen, GridPoint location)
        {
            BornDiedQueue[location.X][location.Y] = new Tuple<Being, Genome, Being, Being>(null, new Genome(oldGen), null, null);
        }

        public void MakeDie(Being being)
        {
            BornDiedQueue[being.Location.X][being.Location.Y] = new Tuple<Being, Genome, Being, Being>(being, null, null, null);
        }


        public void Cycle(ref GridPoint pt)
        {
            pt = new GridPoint(pt.X.Cycle(GridWidth), pt.Y.Cycle(GridHeight));
        }

    }
}
