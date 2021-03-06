﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Runtime.Serialization;

namespace LifeGame
{
    public class Event
    {
        // This class should contains time of the event, the message(actor, action), all the data from the previous n ticks and the data from the following n ticks
        // This class will manage also the UI, input and visualization
    }

    //This class can be instantiated multiple times to permit multiple simulations to be run at the same time
    [Serializable]
    public class Simulation
    {

        public static Simulation Instance;

        static FastRandom rand = new FastRandom();

        public int TimeTick { get; set; }

        /// <summary>
        /// Tells if should be used PopulationCount and the hall of fame genome list to maintain the population.
        /// </summary>
        public bool TrainingMode { get; set; }

        /// <summary>
        /// History of events.
        /// </summary>
        public List<Event> Events { get; set; }


        public SimEnvironment Environment { get; set; }
        public HallOfFame HallOfFame;
        public NNGlobalLists NNLists;

        public Thing[][] Terrain { get; set; }


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

        public Simulation()
        {
            Instance = this;
            // timer.Interval = new TimeSpan(0, 0, 0, 0, 500);

            //thread = new Thread(Update);
            //thread.IsBackground = true;
            //thread.Priority = ThreadPriority.Highest;
            //thread.Start();

            TrainingMode = true;
            //hallOfFame = new List<Genome>(10);

            Environment = new SimEnvironment(this);

            Terrain = new Thing[GraphicsEngine.GRID_WIDTH][];
            BeingLocQueue = new List<Tuple<Being, GridPoint>>();
            BornDiedQueue = new Tuple<Being, Genome, Being, Being>[GraphicsEngine.GRID_WIDTH][];
            for (int i = 0; i < GraphicsEngine.GRID_WIDTH; i++)
            {
                Terrain[i] = new Thing[GraphicsEngine.GRID_HEIGHT];
                BornDiedQueue[i] = new Tuple<Being, Genome, Being, Being>[GraphicsEngine.GRID_HEIGHT];
                for (int j = 0; j < GraphicsEngine.GRID_HEIGHT; j++)
                {
                    Terrain[i][j] = new Thing(ThingType.Grass, new GridPoint(i, j));
                }
            }
            Population = new Dictionary<int, Being>();
            freeBeingObjs = new List<Being>();
            int id = 0;
            for (int i = 0; i < GraphicsEngine.MAX_POPULATION_COUNT; i++)
            {
                var being = new Being();
                being.ID = id++;
                freeBeingObjs.Add(being);
            }

            InitLoad();

            NNLists = new NNGlobalLists();
            HallOfFame = new HallOfFame();

        }

        //public void UnbindEngine()
        //{
        //    foreach (var arr in Terrain)
        //    {
        //        foreach (var thing in arr)
        //        {
        //            GraphicsEngine.Instance.removeThing(thing);
        //        }
        //    }
        //    foreach (var being in Population)
        //    {
        //        GraphicsEngine.Instance.removeThing(being.Value);
        //    }
        //}

        public void InitLoad()
        {
            foreach (var arr in Terrain)
            {
                foreach (var thing in arr)
                {
                    GraphicsEngine.Instance.ChangeCell(thing);
                }
            }
            foreach (var being in Population)
            {
                GraphicsEngine.Instance.addBeing(being.Value);
            }
        }

        public float ActualFPS;
        static ParallelOptions po = new ParallelOptions() { MaxDegreeOfParallelism = 10 };

        //This must be at a fixed rate, so the rate is the one defined by the user
        public void Update()
        {

            {

                if (Population.Count > 0)
                {
                    //  Debug.Write("    Population[0]:  Health: " + Population.ElementAt(0).Value.Properties[(ThingProperty)BeingMutableProp.Health].ToString("0.00"));
                }
                Debug.Write("    best fitness: " + HallOfFame.Genomes[0].Fitness[0].ToString("0.000") + "\n");

                TimeTick++;

                Environment.Update();

                int idx = 0;
#if DEBUG
                    var po = new ParallelOptions() { MaxDegreeOfParallelism = 1 };
#endif
                Parallel.ForEach(Terrain, po, arr =>
                {
                    foreach (var thing in arr) thing.Update();
                });

                Parallel.ForEach(Population, po, being => being.Value.Update());

                Parallel.ForEach(Terrain, po, arr =>
                {
                    foreach (var thing in arr) thing.Apply();
                });

                Parallel.ForEach(Population, po, being => being.Value.Apply());

                // draw: change being texture in order to match last action
                if (GraphicsEngine.Instance.FPS > 0)
                {
                    foreach (var being in Population)
                    {
                        if (being.Value.LastAction != being.Value.OldAction)
                        {
                            GraphicsEngine.Instance.ChangeBeingTex(being.Value);
                        }
                    }
                }


                // make born some beings
                if (TrainingMode)
                {
                    if (Population.Count < GraphicsEngine.POPULATION_COUNT)
                    {
                        Thing cell;
                        int x, y;
                        do
                        {
                            x = rand.Next(GraphicsEngine.GRID_WIDTH);
                            y = rand.Next(GraphicsEngine.GRID_HEIGHT);
                            cell = Terrain[x][y];
                        } while (cell.InnerThing != null || BornDiedQueue[x][y] != null);// || cell.Type == ThingType.Mountain || cell.Type == ThingType.Water

                        GiveBirth(HallOfFame.RndOffspringGen, new GridPoint(x, y));
                    }
                }

                // make die some beings
                while (Population.Count > GraphicsEngine.POPULATION_COUNT)
                {
                    idx = rand.Next(Population.Count);
                    var being = Population.ElementAt(idx).Value;
                    MakeDie(being);
                }


                // apply births/deaths
                //Parallel.For(0, GridWidth, po, x =>
                for (int x = 0; x < GraphicsEngine.GRID_WIDTH; x++)
                {
                    for (int y = 0; y < GraphicsEngine.GRID_HEIGHT; y++)
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

                                GraphicsEngine.Instance.addBeing(being); //             <-----chiamata all'engine
                            }
                            else
                            {
                                var cell = Terrain[x][y];
                                var being = tuple.Item1;
                                cell.InnerThing = null;

                                Population.Remove(being.ID); // remove form Population
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

                                // lay corpse
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
                                var fitnessArr = new float[Constants.FITNESS_PARAM_COUNT];
                                for (int i = 0; i < Constants.FITNESS_PARAM_COUNT; i++)
                                {
                                    fitnessArr[i] = being.Fitness.Parameters[i].Value;
                                }
                                being.Genome.Fitness = fitnessArr;
                                HallOfFame.TryEnqueue(being.Genome);

                                GraphicsEngine.Instance.RemoveBeing(being); //             <-----chiamata all'engine
                            }
                            BornDiedQueue[x][y] = null;
                        }
                    }
                }//);

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
                            newLoc = GraphicsEngine.Cycle(newLoc.GetNearCell());
                            newCell = Terrain[newLoc.X][newLoc.Y];
                        }
                        newCell.InnerThing = being;
                        Terrain[being.Location.X][being.Location.Y].InnerThing = null;
                        being.Location = newLoc;
                        GraphicsEngine.Instance.WalkAnimation(being); //             <-----chiamata all'engine
                    }
                    else
                    {

                    }
                    BeingLocQueue.RemoveAt(idx);
                }

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


    }
}
