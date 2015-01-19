using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGame
{
    public class SignalArray
    {
        readonly float[] _wrappedArray;
        readonly int _offset;
        readonly int _length;

        public SignalArray(float[] wrappedArray, int offset, int length)
        {
            _wrappedArray = wrappedArray;
            _offset = offset;
            _length = length;
        }

        /// <summary>
        /// Access underlying array elements through [] operator
        /// </summary>
        public float this[int index]
        {
            get
            {
                return _wrappedArray[_offset + index];
            }
            set
            {
                _wrappedArray[_offset + index] = value;
            }
        }
    }
}
