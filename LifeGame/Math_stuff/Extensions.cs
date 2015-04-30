using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LifeGame
{
    public static class Extensions
    {
        // I made this extension method just for experimenting, I could have done this more concisely implementing it for what i needed

        /// <summary>
        /// Insertion Sort. Preferable to LINQ Sort for nearly sorted data.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="comparison">Comparer</param>
        public static void InsertionSort<T>(this IList<T> list, Comparison<T> comparison) where T : class
        {
            int count = list.Count;
            for (int i = 1; i < count; i++)
            {
                int j = i;
                while (j > 0)
                {
                    if (comparison(list[j - 1], list[j]) > 0)
                    {
                        T temp = list[j - 1];
                        list[j - 1] = list[j];
                        list[j] = temp;
                        j--;
                    }
                }
            }
        }

        public static float Sigmoid(this float x)
        {
            return 1f / (1f + ((float)Math.Exp(-x)));// standard sigmoid
            //return 0.5f + (x / 2f / (0.2f + Math.Abs(x)));
            //postActArr[j] = (float)(Math.Atan(preActArr[j]) / Math.PI) + 0.5f;
            // TODO: reconsider the activation function, can "0.5+(x/(2*(0.2f+abs(x))))" be better for performance/quality?
        }

        public static float InverseSigmoid(this float x)
        {
            return (x > 0f && x < 1f ? (float)Math.Log(-x / (x - 1f)) : (x > 0.5f ? 10000000f : -10000000f));
            //return -(float)Math.Tan(Math.PI * (0.5 - x));
        }

        /// <summary>
        /// Convert an integer [0, 5] to an angle [0, 2PI]
        /// </summary>
        public static float DirectionToAngle(this CellDirection dir)
        {
            return ((float)dir + 0.5f) * (float)Math.PI / 3;
        }

        /// <summary>
        /// Convert an angle [0, 2PI] to an integer [0, 5]
        /// </summary>
        public static CellDirection AngleToDirection(this float angle)
        {
            return (CellDirection)(Math.Round(angle * 3 / (float)Math.PI - 0.5f));
        }
        /// <summary>
        /// Mod function extended to negative numbers
        /// </summary>
        /// <param name="max">the divisor</param>
        /// <returns></returns>
        public static int Cycle(this int n, int max)
        {
            return n >= 0 && n < max ? n : (n < 0 ? n + max : n - max);
        }


        /// <summary>
        /// The true modulus operator
        /// </summary>
        public static float Modulus(this float a, float b)
        {
            return a - b * (float)Math.Floor(a / b);
        }
    }
}
