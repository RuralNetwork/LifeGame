using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LifeGame
{

    [Serializable]
    public struct Chromosome
    {

        public bool? Sexual { get; private set; }

        public float[] Genes { get; private set; }

        public Chromosome(float[] genes, bool? sexual)
            : this()
        {
            Genes = genes;
            Sexual = sexual;
        }
    }

    [Serializable]
    public class BodyGenome
    {
        public BodyGenome FatherGen;
        public BodyGenome MotherGen;
        public Chromosome[] ChromArr1 { get; private set; }// for now we test with 1 pair of sexual chromosomes and 1 pair of autosomes
        public Chromosome[] ChromArr2 { get; private set; }

        //public Allele


        public static float RProb = 0.1f;
        public static float GProb = 0.1f;
        public static float BProb = 0.1f;
        public static float HeightMulProb = 0.3f;
        public static float SightMulProb = 0.01f;
        public static float HerbCarnProb = 0.02f;
        //RouletteWeel rw = new RouletteWeel(RProb, HeightMulProb, SightMulProb, HerbCarnProb);



        public BodyGenome(BodyGenome dadGen, BodyGenome mumGen)
        {
            ChromArr1 = new Chromosome[1];
            ChromArr2 = new Chromosome[1];
            for (int i = 0; i < 6; i++)
            {

            }
        }

        void mutate()
        {

        }


    }
}
