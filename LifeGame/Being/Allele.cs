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

        public Allele()
        {

        }


        static Dictionary<Substance, Action<Being, Allele, Allele>> ApplyList;

        static Allele()
        {

        }
    }

}
