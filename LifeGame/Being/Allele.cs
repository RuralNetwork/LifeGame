using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGame
{

    [Serializable]
    public class Allele
    {
        public PhenomeEffect SubstEncoded;
        public int DominanceLevel;
        public float MutationProb;

        public Allele(PhenomeEffect subst, int domLvl, int mutProb)
        {
            SubstEncoded = subst;
            DominanceLevel = domLvl;
            MutationProb = mutProb;
        }

        public static Dictionary<PhenomeEffect, Dictionary<string, int>> AlleleTypes;

        public static Dictionary<PhenomeEffect, Action<Being, Allele, Allele>> ApplyList;

        static Allele()
        {
            AlleleTypes = new Dictionary<PhenomeEffect, Dictionary<string, int>>();
            ApplyList = new Dictionary<PhenomeEffect, Action<Being, Allele, Allele>>();

        }
    }

}
