using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGame
{
    //questo enum serve solo come riferimento per gli indici, vediamo se tenerlo o no in futuro
    public enum ThingType
    {
        Earth = 0,
        Grass = 1,
        Water = 2,
        Bush = 3,
        Berry = 4,
        Tree = 5,
    }

    // Per poter passare da un tipo di Thing all'altro avrei dovuto usare qualche strana roba con il Reflection, che è sempre una cosa pesante.
    // I Thing possono avere proprietà peculiari (internalProps) ma queste non avranno nome, cumunque sono gestite totalmente dal Thing stesso.
    // Mantenendo una sola classe per tutti i Thing il codice diventa meno OO, ma più estendibile, potendo creare durante il runtime nuovi tipi di Thing (tipo con un editor)
    // In questo file definiamo tutto ciò che è peculiare ai vari thing, da come si comportano nella simulazione a come si disegnano.
    // Utilizziamo insiemi statici.
    public partial class Thing
    {
        static List<Dictionary<ActionType, Effects>> interactionsDicts = new List<Dictionary<ActionType, Effects>>();
        static List<int> nInternalProps = new List<int>();
        static List<UpdateDelegate> updateDels = new List<UpdateDelegate>();
        static Thing()
        {
            //Earth:
            var dict = new Dictionary<ActionType, Effects>();
            dict.Add(ActionType.Walk, b =>
            {
               // b.DeltaEnergy-=
            });
            dict.Add(ActionType.Sleep, b =>
            {
                //...
            });
            //...
            interactionsDicts.Add(dict);

            updateDels.Add(() =>
            {
                //...
            });

            nInternalProps.Add(3);//?

            //parte di drawing qui


            //Grass:

            //...
            //...


            //Berry:
            dict = new Dictionary<ActionType, Effects>();
            //dict.Add(ActionType)

        }
    }
}
