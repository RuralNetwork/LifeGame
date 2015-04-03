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
        Environment,
        Being,
        Earth,
        Grass,
        Water,
        Bush,
        Berry,
        Tree,
        Mountain,
    }

    // Per poter passare da un tipo di Thing all'altro avrei dovuto usare qualche strana roba con il Reflection, che è sempre una cosa pesante.
    // I Thing possono avere proprietà peculiari (internalProps) ma queste non avranno nome, cumunque sono gestite totalmente dal Thing stesso.
    // Mantenendo una sola classe per tutti i Thing il codice diventa meno OO, ma più estendibile, potendo creare durante il runtime nuovi tipi di Thing (tipo con un editor)
    // In questo file definiamo tutto ciò che è peculiare ai vari thing, da come si comportano nella simulazione a come si disegnano.
    // Utilizziamo insiemi statici.
    public partial class Thing
    {
        static List<Dictionary<ActionType, Effects>> interactionsDicts = new List<Dictionary<ActionType, Effects>>();
        static List<UpdateDelegate> updateDels = new List<UpdateDelegate>();
        static List<Dictionary<ThingProperty, float>> defProps = new List<Dictionary<ThingProperty, float>>();// default properties

        static Thing()
        {
            //Earth:
            var dict = new Dictionary<ActionType, Effects>();
            dict.Add(ActionType.Walk, (t, b) =>
            {
                b.DeltaEnergy -= t.Properties[ThingProperty.Wet] * 0.01f;
            });
            dict.Add(ActionType.Sleep, (t, b) =>
            {
                // b.Energy
            });
            //...
            interactionsDicts.Add(dict);

            updateDels.Add(() =>
            {
                //...
            });

           // defProps.Add(new float[0]);

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
