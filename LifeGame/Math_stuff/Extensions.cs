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
    }
}
