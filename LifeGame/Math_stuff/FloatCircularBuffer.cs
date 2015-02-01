using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGame
{
    //this circular buffer gives an inconsistent mean for the first n values enqueued (where n is the buffer capacity). this was done for the sake of speed
    // this class is used for FitnessHistory property
    public class FloatCircularBuffer
    {
        float[] buffer;
        int idx;
        int capacity;

        public float Total { get; private set; }

        public FloatCircularBuffer(int capacity)
        {
            buffer = new float[capacity];
            this.capacity = capacity;
        }

        public float Mean
        {
            get
            {
                return Total / capacity;
            }
        }

        public void Enqueue(float item)
        {
            Total -= buffer[idx];
            idx++;
            if (idx == capacity) idx = 0;
            buffer[idx] = item;
            Total += item;
        }

    }
}
