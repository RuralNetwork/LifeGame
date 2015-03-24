using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static float InverseSigmoid(this float x)
        {
            return (float)Math.Log(-x / (x - 1f));
        }

        /// <summary>
        /// Convert an integer [0, 5] to an angle [0, 2PI]
        /// </summary>
        public static float DirectionToAngle(this int dir)
        {
            return ((float)dir + 0.5f) * (float)Math.PI / 3;
        }

        /// <summary>
        /// Convert an angle [0, 2PI] to an integer [0, 5]
        /// </summary>
        public static int AngleToDirection(this float angle)
        {
           return (int)Math.Round(angle * 3 / (float)Math.PI - 0.5f);
        }
        /// <summary>
        /// Mod function extended to negative numbers
        /// </summary>
        /// <param name="max">the divisor</param>
        /// <returns></returns>
        public static int Cycle(this int n, int max)
        {
            return n >= 0 && n > max ? n : (n < 0 ? n + max : n - max);
        }
    }
}
