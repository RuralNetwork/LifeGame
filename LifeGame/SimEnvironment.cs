﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Diagnostics;

namespace LifeGame
{

    // Siccome il colore del cielo è inutile, il colore Del SimEnviroment sarà un fattore per determinare come i Thing vengono percepiti.
    // Formula: thingColor*envColor/255, per ogni componente
    [Serializable]
    public class SimEnvironment
    {
        Simulation simulation;

        //Properties
        public float Color1 { get; set; }
        public float Color2 { get; set; }
        public float Color3 { get; set; }
        public float Painful { get; set; }
        public float Temperature { get; set; }


        public int DayTicks { get; set; }
        public int YearTicks { get; set; }

        //These are properties from 0 to 1, like most Thing properties
        public float Clouds { get; set; }
        public float Rain { get; set; }
        public float Thunderstorm { get; set; }
        public float HailStorm { get; set; }
        public float Snow { get; set; }

        /// <summary>
        /// Parameters:
        /// 1) Type: clean sky, cloudy, rainy, thunderstorm, hailstorm, snowy
        /// 2) Intensity
        /// 3) Duration (in ticks)
        /// </summary>
        public Tuple<int, float, int> LockWeather { private get; set; }


        public SimEnvironment(Simulation simulation)
        {
            this.simulation = simulation;
            // questi valori non devono essere verosimili, ci serve una simulazione che appaia veloce facendo scorrere il tempo velocemente
            DayTicks = 240;
            YearTicks = 24000;
        }

        // SimEnvironment doen't need an apply method, it is not influenced by anything.
        public void Update()
        {
            //float tick = simulation.TimeTick;
            ////sun motion
            //var sunAng = -(float)(Math.Cos(tick / 120 * Math.PI));// * 0.3 * Math.PI   // questa è l'altezza del sole durante il giorno
            //var transSpeed = 0.1f;// lower is faster
            //var color = sunAng / (transSpeed + Math.Abs(sunAng)) / 2 + 0.5f; // fast sigmoid function: x / (k + abs(x)) / 2 + 0.5

            Color1 = 1;// per ora non esiste l'alternanza giorno/notte, la temperatura è sempre 25° e non ci sono precipitazioni
            Color2 = 1;
            Color3 = 1;
            Temperature = 298f;
        }

        public void Draw(bool isCarriedObj = false)
        {

        }

        public void StartCleanSky(float intensity)
        {

        }

        public void StartClouds(float intensity, int duration)
        {

        }

        public void StartRain(float intensity, int duration)
        {

        }

        public void StartThunderstorm(float intensity, int duration)
        {

        }

        public void StartHailstorm(float intensity, int duration)
        {

        }

        public void StartSnow(float intensity, int duration)
        {

        }
    }

}
