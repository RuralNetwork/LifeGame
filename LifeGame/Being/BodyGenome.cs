using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LifeGame
{


    // /////////////////////////////////////For now i'm leaving the body genome and keeping the same phenome for every being


    public struct Chromosome
    {

        public bool? IsSexual { get; private set; }

        public float[] Genes { get; private set; }

        public Chromosome(float[] genes, bool? isSexual)
            : this()
        {
            Genes = genes;
            IsSexual = isSexual;
        }
    }

    public class BodyGenome
    {
        public Chromosome[] ChromArr1 { get; private set; }// for now we test with 1 pair of sexual chromosomes and 1 pair of autosomes
        public Chromosome[] ChromArr2 { get; private set; }

        //Consider switching to a more realistic rapresentation of genes

        public static float RProb = 0.1f;
        public static float GProb = 0.1f;
        public static float BProb = 0.1f;
        public static float HeightMulProb = 0.3f;
        public static float SightMulProb = 0.01f;
        public static float HerbCarnProb = 0.02f;
        //RouletteWeel rw = new RouletteWeel(RProb, HeightMulProb, SightMulProb, HerbCarnProb);

        public BodyGenome(Chromosome[] chromArr1, Chromosome[] chromArr2)
        {
            ChromArr1 = chromArr1.Clone() as Chromosome[];
        }

        public BodyGenome(BodyGenome parent1, BodyGenome parent2)
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
