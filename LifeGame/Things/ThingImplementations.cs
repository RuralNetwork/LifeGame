using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGame
{

    // Per poter passare da un tipo di Thing all'altro avrei dovuto usare qualche strana roba con il Reflection, che è sempre una cosa pesante.
    // I Thing possono avere proprietà peculiari (internalProps) ma queste non avranno nome, cumunque sono gestite totalmente dal Thing stesso.
    // Mantenendo una sola classe per tutti i Thing il codice diventa meno OO, ma più estendibile, potendo creare durante il runtime nuovi tipi di Thing (tipo con un editor)
    // In questo file definiamo tutto ciò che è peculiare ai vari thing, da come si comportano nella simulazione a come si disegnano.
    // Utilizziamo insiemi statici.
    public partial class Thing
    {
        // TODO: consider swotching from List<...> to Dictionary<ThingType, ...> for more clarity, but it will be slower
        static List<Dictionary<ThingProperty, float>> propsDicts = new List<Dictionary<ThingProperty, float>>();// default properties
        static List<Dictionary<ActionType, Effects>> interactionsDicts = new List<Dictionary<ActionType, Effects>>();
        static List<UpdateDelegate> updateDels = new List<UpdateDelegate>();

        static Thing()
        {
            Dictionary<ThingProperty, float> propsDict;
            Dictionary<ActionType, Effects> interactDict;

            //---------------- Being:
            propsDict = new Dictionary<ThingProperty, float>();
            propsDict.Add(ThingProperty.Height, 0);// Definisco qui le caratteristiche standard di un Being.
            propsDict.Add(ThingProperty.Alpha, 0);//  Non essendoci ancora il sistema ereditario dei caratteri fisici,
            propsDict.Add(ThingProperty.Weigth, 0);// ogni Being nascerà con queste proprietà. 
            propsDict.Add(ThingProperty.Color1, 0);
            propsDict.Add(ThingProperty.Color2, 0);
            propsDict.Add(ThingProperty.Color3, 0);
            propsDict.Add(ThingProperty.Moving, 0);
            propsDict.Add(ThingProperty.Painful, 0);
            propsDict.Add(ThingProperty.Temperature, 0);
            propsDict.Add(ThingProperty.Pitch, 0);
            propsDict.Add(ThingProperty.Amplitude, 0);
            propsDict.Add(ThingProperty.Smell1, 0);
            propsDict.Add(ThingProperty.Smell2, 0);
            propsDict.Add(ThingProperty.Smell3, 0);
            propsDict.Add(ThingProperty.SmellIntensity, 0);
            propsDict.Add(ThingProperty.Wet, 0.2f);
            propsDict.Add((ThingProperty)BeingMutableProp.Energy, 1);
            propsDict.Add((ThingProperty)BeingMutableProp.Health, 1);
            propsDict.Add((ThingProperty)BeingMutableProp.Integrity, 1);
            propsDict.Add((ThingProperty)BeingMutableProp.Hunger, 1);
            propsDict.Add((ThingProperty)BeingMutableProp.Thirst, 1);
            propsDicts.Add(propsDict);

            interactDict = new Dictionary<ActionType, Effects>();
            interactDict.Add(ActionType.Walk, walkThrough);
           // interactDict.Add(ActionType.Breed, )
            interactionsDicts.Add(interactDict);

            updateDels.Add(() => { });

            //---------------- Earth:
            propsDict = new Dictionary<ThingProperty, float>();
            propsDict.Add(ThingProperty.Height, 0);
            propsDict.Add(ThingProperty.Alpha, 0);
            propsDict.Add(ThingProperty.Weigth, 0);
            propsDict.Add(ThingProperty.Color1, 0);
            propsDict.Add(ThingProperty.Color2, 0);
            propsDict.Add(ThingProperty.Color3, 0);
            propsDict.Add(ThingProperty.Moving, 0);
            propsDict.Add(ThingProperty.Painful, 0);
            propsDict.Add(ThingProperty.Temperature, 0);
            propsDict.Add(ThingProperty.Pitch, 0);
            propsDict.Add(ThingProperty.Amplitude, 0);
            propsDict.Add(ThingProperty.Smell1, 0);
            propsDict.Add(ThingProperty.Smell2, 0);
            propsDict.Add(ThingProperty.Smell3, 0);
            propsDict.Add(ThingProperty.SmellIntensity, 0);
            propsDict.Add(ThingProperty.Wet, 0);
            propsDicts.Add(propsDict);

            interactDict = new Dictionary<ActionType, Effects>();
            interactDict.Add(ActionType.Walk, walkThrough);
            interactDict.Add(ActionType.Sleep, (t, b) =>
            {
                // b.Energy
            });
            //...
            interactionsDicts.Add(interactDict);

            updateDels.Add(() =>
            {
                //...
            });

            // defProps.Add(new float[0]);



            //---------------- Grass:

            //...
            //...


            //Berry:
            interactDict = new Dictionary<ActionType, Effects>();
            //dict.Add(ActionType)

        }

        //=========== Common behavior & helper functions =========

        static void walkThrough(Thing t, Being b)
        {
            b.DeltaEnergy -= t.Properties[ThingProperty.Height] * t.Properties[ThingProperty.Alpha] * b.Properties[ThingProperty.Weigth];
            //b.ModQueue.Add(new ThingMod(new Tuple<ThingProperty, float>((ThingProperty)BeingMutableProp.Energy,...........)));
        }

        protected void mute()
        {
            ChangeProp(ThingProperty.Amplitude, -Properties[ThingProperty.Amplitude]);
        }
    }
}
