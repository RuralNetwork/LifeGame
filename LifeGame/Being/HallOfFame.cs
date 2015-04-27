using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGame
{
    /// <summary>
    /// Stores genomes ordered by fitness parameters.
    /// </summary>
    public class HallOfFame
    {
        const float THRESH = 0.5f;//min distance of fitness values
        const int COUNT = 10;
        static FastRandom rand = new FastRandom();
        float[] mags;

        public  Genome[] Genomes;// this array is always ordered by fitness magnitude

        public HallOfFame()
        {
            Genomes = new Genome[COUNT];
            mags = new float[COUNT];
            for (int i = 0; i < COUNT; i++)
            {
                Genomes[i] = new Genome();
            }
        }

        public Genome RandomGenome
        {
            get
            {
                return new Genome(Genomes[rand.Next(COUNT)]);
            }
        }

        public Genome RndOffspringGen
        {
            get
            {
                return new Genome(Genomes[rand.Next(COUNT)], Genomes[rand.Next(COUNT)]); // it may be the same genome, but it doesn't care
            }
        }


        public void TryEnqueue(Genome newGen)
        {
            var betterList = new List<int>(COUNT);
            var worseList = new List<int>(COUNT);
            var newGenMag = newGen.Magnitude;

            if (newGenMag > mags[COUNT - 1])// check if even better than the worst genome
            {
                for (int i = 0; i < COUNT; i++)// create better/worse index lists
                {
                    if (mags[i] > newGenMag)
                    {
                        betterList.Add(i);
                    }
                    else
                    {
                        worseList.Add(i);
                    }
                }

                foreach (var idx in betterList)// if the new genome is likely as good as the better ones (for every fitness parameter), discard it
                {
                    if (newGen.DistanceTo(Genomes[idx]) < THRESH)
                    {
                        return;
                    }
                }

                float minDist = 100000000f;
                int nearest = COUNT;
                foreach (var idx in worseList)// then override the nearest worse genome
                {
                    var dist = newGen.DistanceTo(Genomes[idx]);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        nearest = idx;
                    }
                }
                Genomes[nearest] = newGen;
                mags[nearest] = newGenMag;
            }
        }
    }
}
