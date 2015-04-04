namespace LifeGame
{
    public enum SimulationType
    {
        //Slow: show every move, game tick is every 200ms, maybe 500ms or even less, 100mx
        Slow,
        //Medium: every move after the other, not really able to percieve the movements, but the animation still runs, smth like 20ms
        Medium,
        //Fast: graphicengine stops, doesn't update the screen anymore, each action is performed after the other, basically no game thick, as soon as it can, it just does the next thing
        Fast
    }

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

    /// <summary>
    /// Theese are only active actions. Passive actions are implemented in Thing.Interactions delegates.
    /// </summary>
    public enum ActionType
    {
        Walk,
        Sleep,
        Eat,
        Breed,
        Fight,
        Take,
        Drop,
        MakeSound,

    }

    /// <summary>
    /// Properties that are common to all Things. Some span form 0 to 1, some doesn't have upper limit
    /// </summary>
    public enum ThingProperty : int
    {
        /// <summary>
        /// In meters
        /// </summary>
        Height,
        Alpha,// this can be either the transparency of the thing or the proportion of visual covered
        // We assume the things to occupy all cell's area, so if a thing should be narrow in real life, it will have a low value of Alpha.
        // this obviously is unrelated to the GUI

        /// <summary>
        /// In kilograms
        /// </summary>
        Weigth,
        Color1,// TODO: this will be changed to a series of intensities for every frequency.
        Color2,// then the beings can evolve to perceive some of these frequencies
        Color3,
        /// <summary>
        /// (m*s)^-1  -> speed over surface. The surface is the area of the object that the being see.
        /// </summary>
        Moving,
        Painful,
        /// <summary>
        /// In Kelvin
        /// </summary>
        Temperature,// this will be included in the frequency graph
        /// <summary>
        /// In Hertz
        /// </summary>
        Pitch,
        /// <summary>
        /// In deciBels
        /// </summary>
        Amplitude,
        Smell1,// I use Henning's smell prism (but it is old, if we find something more recent is better)
        Smell2,
        Smell3,
        SmellIntensity,
        /// <summary>
        /// From 0 to 1, wet surface
        /// </summary>
        Wet,
        Custom1,
        Custom2,
        Custom3,
        Custom4,
        Custom5,
    }

    /// <summary>
    /// This enum is an extension of ThingProperty and should be used with Properties dictionary
    /// </summary>
    enum BeingMutableProp : int
    {
        Energy = 100,
        /// <summary>
        /// The maximum health is the Integrity value.
        /// It decreases due to hunger or thirst.
        /// In normal condition it slowly increase.
        /// When health reaches 0, the being dies.
        /// </summary>
        Health = 101,// can be healed
        /// <summary>
        /// Always decreases during lifetime, due to age or wounds
        /// </summary>
        Integrity = 102, // cannot be healed
        Thirst = 103,
        Hunger = 104,
    }
}