﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LifeGame
{
    class Constants
    {
        public const int INPUTS_COUNT = 28;
        public const int INPUTS_AND_BIAS_COUNT = INPUTS_COUNT + 1;
        public const int OUTPUTS_COUNT = 10;
        public static int FITNESS_PARAM_COUNT = Enum.GetNames(typeof(FitnessParam)).Length;
    }
}
