using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGame
{
    public class Average
    {
        public float Value { get; set; }
        public int Count { get; set; }

        public Average() { }
        public Average(float value, int count)
        {
            Value = value;
            Count = count;
        }

        public void Add(float value)
        {
            Value = Value * Count + value;
            Count++;
            Value /= Count;
        }
    }
}
