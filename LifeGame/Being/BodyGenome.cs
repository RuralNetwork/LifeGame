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

        public Allele[] Genes { get; private set; }

        public Chromosome(bool? sexual, params Allele[] genes)
            : this()
        {
            Sexual = sexual;
            Genes = genes;
        }
    }

    [Serializable]
    public class BodyGenome
    {
        public Chromosome[] ChromArr1 { get; private set; }// for now we test with 1 pair of sexual chromosomes and 1 pair of autosomes
        public Chromosome[] ChromArr2 { get; private set; }


        //public static float RProb = 0.1f;
        //public static float GProb = 0.1f;
        //public static float BProb = 0.1f;
        //public static float HeightMulProb = 0.3f;
        //public static float SightMulProb = 0.01f;
        //public static float HerbCarnProb = 0.02f;
        //RouletteWeel rw = new RouletteWeel(RProb, HeightMulProb, SightMulProb, HerbCarnProb);

        public BodyGenome(BodyGenome genome)
        {
            ChromArr1 = new Chromosome[2];
            ChromArr2 = new Chromosome[2];
            //ChromArr1[1]=new Chromosome(null,new Allele(Substance))
        }

        public BodyGenome(BodyGenome dadGen, BodyGenome mumGen)
        {
            //dadGen.
        }

        void mutate()
        {

        }

        public void Apply(Being being)
        {
            for (int i = 0; i < ChromArr1.Length; i++)
            {
                for (int j = 0; j < ChromArr1[i].Genes.Length; j++)
                {
                    var allele1 = ChromArr1[i].Genes[j];
                    var allele2 = ChromArr2[i].Genes[j];
                    Allele.ApplyList[allele1.SubstEncoded](being, allele1, allele2);
                }
            }
        }


    }
}
