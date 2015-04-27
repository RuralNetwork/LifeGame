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
        public Substance SubstEncoded;
        public int DominanceLevel;
        public float MutationProb;

        public Allele(Substance subst, int domLvl, int mutProb)
        {
            SubstEncoded = subst;
            DominanceLevel = domLvl;
            MutationProb = mutProb;
        }

        public static Dictionary<Substance, Dictionary<string, int>> AlleleTypes;

        public static Dictionary<Substance, Action<Being, Allele, Allele>> ApplyList;

        static Allele()
        {
            AlleleTypes = new Dictionary<Substance, Dictionary<string, int>>();
            ApplyList = new Dictionary<Substance, Action<Being, Allele, Allele>>();

        }
    }

}
