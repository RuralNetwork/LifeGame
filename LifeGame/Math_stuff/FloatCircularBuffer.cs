using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LifeGame
{//                                                                 OBSOLETE
    // this class is used for FitnessHistory property
    //public class FloatCircularBuffer
    //{
    //    float[] buffer;
    //    int idx;
    //    int capacity;
    //    bool isFull;// the buffer starts assigning values from idx 1. when idx 0 is assigned, the buffer is full.

    //    public float Total { get; private set; }
    //    public float Mean { get; private set; }

    //    public FloatCircularBuffer(int capacity)
    //    {
    //        buffer = new float[capacity];
    //        this.capacity = capacity;
    //    }

    //    public void Enqueue(float item)
    //    {
    //        Total -= buffer[idx];
    //        idx++;
    //        if (idx == capacity)
    //        {
    //            isFull = true;
    //            idx = 0;
    //        }
    //        buffer[idx] = item;
    //        Total += item;
    //        Mean = Total / (isFull ? capacity : idx);
    //    }

    //}
}
