using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

            //proprietà che non devono essere nulle: weight, amplitude, smellIntensity

            //---------------- Null:
            propsDict = new Dictionary<ThingProperty, float>();
            propsDict.Add(ThingProperty.Height, 0);
            propsDict.Add(ThingProperty.Alpha, 0);
            propsDict.Add(ThingProperty.Weigth, 0.0001f);
            propsDict.Add(ThingProperty.Color1, 0);
            propsDict.Add(ThingProperty.Color2, 0);
            propsDict.Add(ThingProperty.Color3, 0);
            propsDict.Add(ThingProperty.Moving, 0);
            propsDict.Add(ThingProperty.Painful, 0);
            propsDict.Add(ThingProperty.Temperature, 0);
            propsDict.Add(ThingProperty.Pitch, 0);
            propsDict.Add(ThingProperty.Amplitude, 0.0001f);
            propsDict.Add(ThingProperty.Smell1, 0);
            propsDict.Add(ThingProperty.Smell2, 0);
            propsDict.Add(ThingProperty.Smell3, 0);
            propsDict.Add(ThingProperty.SmellIntensity, 0.0001f);
            propsDict.Add(ThingProperty.Wet, 0);
            propsDicts.Add(propsDict);

            interactDict = new Dictionary<ActionType, Effects>();
            interactDict.Add(ActionType.Eat, (t, b) => { });
            interactDict.Add(ActionType.Fight, (t, b) => { });
            interactionsDicts.Add(interactDict);

            updateDels.Add(() => { });

            //---------------- Being:
            propsDict = new Dictionary<ThingProperty, float>();
            propsDict.Add(ThingProperty.Height, 1.5f);// Definisco qui le caratteristiche standard di un Being.
            propsDict.Add(ThingProperty.Alpha, 0.7f);//  Non essendoci ancora il sistema ereditario dei caratteri fisici,
            propsDict.Add(ThingProperty.Weigth, 30f);// ogni Being nascerà con queste proprietà. 
            propsDict.Add(ThingProperty.Color1, 0.64f);
            propsDict.Add(ThingProperty.Color2, 0.44f);
            propsDict.Add(ThingProperty.Color3, 0.25f);
            propsDict.Add(ThingProperty.Moving, 0);
            propsDict.Add(ThingProperty.Painful, 0);
            propsDict.Add(ThingProperty.Temperature, 310f);
            propsDict.Add(ThingProperty.Pitch, 440f);
            propsDict.Add(ThingProperty.Amplitude, 0.0001f);
            propsDict.Add(ThingProperty.Smell1, 0);
            propsDict.Add(ThingProperty.Smell2, 0);
            propsDict.Add(ThingProperty.Smell3, 0);
            propsDict.Add(ThingProperty.SmellIntensity, 0.0001f);
            propsDict.Add(ThingProperty.Wet, 0.2f);
            propsDict.Add((ThingProperty)BeingMutableProp.Energy, 500f);
            propsDict.Add((ThingProperty)BeingMutableProp.Health, 1);
            propsDict.Add((ThingProperty)BeingMutableProp.Integrity, 1);
            propsDict.Add((ThingProperty)BeingMutableProp.Hunger, 1);
            propsDict.Add((ThingProperty)BeingMutableProp.Thirst, 1);
            propsDicts.Add(propsDict);

            interactDict = new Dictionary<ActionType, Effects>();
            interactDict.Add(ActionType.Breed, (t, b) => { });
            // interactDict.Add(ActionType.Breed, )
            interactionsDicts.Add(interactDict);

            updateDels.Add(() => { });

            //---------------- Earth:
            propsDict = new Dictionary<ThingProperty, float>();
            propsDict.Add(ThingProperty.Height, 0.10f);
            propsDict.Add(ThingProperty.Alpha, 1f);
            propsDict.Add(ThingProperty.Weigth, 0);
            propsDict.Add(ThingProperty.Color1, 0.29f);
            propsDict.Add(ThingProperty.Color2, 0.18f);
            propsDict.Add(ThingProperty.Color3, 0.14f);
            propsDict.Add(ThingProperty.Moving, 0);
            propsDict.Add(ThingProperty.Painful, 0);
            propsDict.Add(ThingProperty.Temperature, 0);
            propsDict.Add(ThingProperty.Pitch, 0);
            propsDict.Add(ThingProperty.Amplitude, 0.0001f);
            propsDict.Add(ThingProperty.Smell1, 0);
            propsDict.Add(ThingProperty.Smell2, 0);
            propsDict.Add(ThingProperty.Smell3, 0);
            propsDict.Add(ThingProperty.SmellIntensity, 0.0001f);
            propsDict.Add(ThingProperty.Wet, 0);
            propsDicts.Add(propsDict);

            interactDict = new Dictionary<ActionType, Effects>();
            interactDict.Add(ActionType.Eat, (t, b) =>
            {

            });
            interactDict.Add(ActionType.Fight, (t, b) =>
            {

            });
            interactDict.Add(ActionType.Take, (t, b) =>
            {

            });

            interactionsDicts.Add(interactDict);

            updateDels.Add(() =>
            {
                //...
            });

            // defProps.Add(new float[0]);




            //---------------- Grass:
            propsDict = new Dictionary<ThingProperty, float>();
            propsDict.Add(ThingProperty.Height, 0.30f);
            propsDict.Add(ThingProperty.Alpha, 0.8f);
            propsDict.Add(ThingProperty.Weigth, 0);
            propsDict.Add(ThingProperty.Color1, 0.64f);
            propsDict.Add(ThingProperty.Color2, 0.75f);
            propsDict.Add(ThingProperty.Color3, 0.34f);
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
            interactDict.Add(ActionType.Eat, (t, b) => { });
            interactDict.Add(ActionType.Fight, (t, b) => { });
            interactionsDicts.Add(interactDict);

            updateDels.Add(() => { });


            //---------------- Water:
            propsDict = new Dictionary<ThingProperty, float>();
            propsDict.Add(ThingProperty.Height, 0.50f);
            propsDict.Add(ThingProperty.Alpha, 0.2f);
            propsDict.Add(ThingProperty.Weigth, 0);
            propsDict.Add(ThingProperty.Color1, 0.46f);
            propsDict.Add(ThingProperty.Color2, 0.75f);
            propsDict.Add(ThingProperty.Color3, 0.87f);
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
            interactDict.Add(ActionType.Eat, (t, b) =>
            {
                b.ChangeProp((ThingProperty)BeingMutableProp.Thirst, 0.05f * b.EnergySpent, false);
            });
            interactDict.Add(ActionType.Fight, (t, b) => { });
            interactionsDicts.Add(interactDict);

            updateDels.Add(() => { });


            //---------------- Sand:
            propsDict = new Dictionary<ThingProperty, float>();
            propsDict.Add(ThingProperty.Height, 0.20f);
            propsDict.Add(ThingProperty.Alpha, 1);
            propsDict.Add(ThingProperty.Weigth, 0);
            propsDict.Add(ThingProperty.Color1, 0.81f);
            propsDict.Add(ThingProperty.Color2, 0.65f);
            propsDict.Add(ThingProperty.Color3, 0.47f);
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
            interactDict.Add(ActionType.Eat, (t, b) => { });
            interactDict.Add(ActionType.Fight, (t, b) => { });
            interactionsDicts.Add(interactDict);

            updateDels.Add(() => { });

            //---------------- Mountain:
            propsDict = new Dictionary<ThingProperty, float>();
            propsDict.Add(ThingProperty.Height, 0);
            propsDict.Add(ThingProperty.Alpha, 0);
            propsDict.Add(ThingProperty.Weigth, 0);
            propsDict.Add(ThingProperty.Color1, 0.48f);
            propsDict.Add(ThingProperty.Color2, 0.42f);
            propsDict.Add(ThingProperty.Color3, 0.38f);
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
            interactDict.Add(ActionType.Eat, (t, b) => { });
            interactDict.Add(ActionType.Fight, (t, b) => { });
            interactionsDicts.Add(interactDict);

            updateDels.Add(() => { });

            //---------------- Bush:
            propsDict = new Dictionary<ThingProperty, float>();
            propsDict.Add(ThingProperty.Height, 1f);
            propsDict.Add(ThingProperty.Alpha, 0.9f);
            propsDict.Add(ThingProperty.Weigth, 0);
            propsDict.Add(ThingProperty.Color1, 0.36f);
            propsDict.Add(ThingProperty.Color2, 0.45f);
            propsDict.Add(ThingProperty.Color3, 0.30f);
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
            interactDict.Add(ActionType.Eat, (t, b) =>
            {
                b.ChangeProp((ThingProperty)BeingMutableProp.Hunger, 0.03f * b.EnergySpent, false);
            });
            interactDict.Add(ActionType.Fight, (t, b) => { });
            interactionsDicts.Add(interactDict);

            updateDels.Add(() => { });

            //---------------- Berry:
            propsDict = new Dictionary<ThingProperty, float>();
            propsDict.Add(ThingProperty.Height, 0);
            propsDict.Add(ThingProperty.Alpha, 0);
            propsDict.Add(ThingProperty.Weigth, 0);
            propsDict.Add(ThingProperty.Color1, 0.43f);
            propsDict.Add(ThingProperty.Color2, 0.12f);
            propsDict.Add(ThingProperty.Color3, 0.16f);
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
            interactDict.Add(ActionType.Eat, (t, b) => { });
            interactDict.Add(ActionType.Fight, (t, b) => { });
            interactionsDicts.Add(interactDict);

            updateDels.Add(() => { });

            //---------------- Tree:
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
            interactDict.Add(ActionType.Eat, (t, b) => { });
            interactDict.Add(ActionType.Fight, (t, b) => { });
            interactionsDicts.Add(interactDict);

            updateDels.Add(() => { });

            //---------------- Fruit:
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
            interactDict.Add(ActionType.Eat, (t, b) => { });
            interactDict.Add(ActionType.Fight, (t, b) => { });
            interactionsDicts.Add(interactDict);

            updateDels.Add(() => { });

            //---------------- Corpse:
            propsDict = new Dictionary<ThingProperty, float>();
            propsDict.Add(ThingProperty.Height, 0);
            propsDict.Add(ThingProperty.Alpha, 0);
            propsDict.Add(ThingProperty.Weigth, 0);
            propsDict.Add(ThingProperty.Color1, 0.31f);
            propsDict.Add(ThingProperty.Color2, 0.19f);
            propsDict.Add(ThingProperty.Color3, 0.11f);
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
            interactDict.Add(ActionType.Eat, (t, b) => { });
            interactDict.Add(ActionType.Fight, (t, b) => { });
            interactionsDicts.Add(interactDict);

            updateDels.Add(() => { });

            //---------------- Meat:
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
            interactDict.Add(ActionType.Eat, (t, b) => { });
            interactDict.Add(ActionType.Fight, (t, b) => { });
            interactionsDicts.Add(interactDict);

            updateDels.Add(() => { });


        }

        //=========== Common behavior & helper functions =========

        static public bool BiggerBetween(Thing thing1, Thing thing2)
        {
            return thing1.Properties[ThingProperty.Height] * thing1.Properties[ThingProperty.Alpha] >
                thing2.Properties[ThingProperty.Height] * thing2.Properties[ThingProperty.Alpha];
        }


        protected void mute()
        {
            ChangeProp(ThingProperty.Amplitude, 0, true);
        }

        protected void rest()
        {

            ChangeProp(ThingProperty.Moving, 0, true);
        }


    }
}
