using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGame
{
    /// <summary>
    /// Circular buffer of key-value pairs. This is used for the addedNode and addedLink buffers. 
    /// When the max capacity is reached, the buffer starts to override the oldest values. 
    /// This can cause the same structures to have different IDs and then don't match during recombination, 
    /// however this is the only chice because the IDs grow very fast during the simulation and exceed the maximum element that a collection can contain.
    /// </summary>
    public class KVCircularBuffer<K, V>
    {
        KeyValuePair<K, V>[] buffer;
        int idx;
        int capacity;

        public KVCircularBuffer(int capacity)
        {
            buffer = new KeyValuePair<K, V>[capacity];
            this.capacity = capacity;
        }

        public void Enqueue(K key, V value)
        {
            buffer[idx] = new KeyValuePair<K, V>(key, value);
            idx++;
            if (idx == capacity) idx = 0;
        }

        public bool TryGetValue(K key, out V value)
        {
            foreach (var kvPair in buffer)
            {
                if (kvPair.Key.Equals(key))
                {
                    value = kvPair.Value;
                    return true;
                }
            }
            value = default(V);
            return false;
        }
    }
}
