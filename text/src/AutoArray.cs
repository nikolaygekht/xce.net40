using System;
using System.Collections.Generic;
using System.Text;

namespace gehtsoft.xce.text
{
    public class AutoArray<T>
    {
        private T[] array;

        public AutoArray()
        {
            array = null;
        }

        public T[] Array
        {
            get
            {
                return array;
            }
        }

        public T[] Ensure(int length)
        {
            if (array == null || array.Length < length)
            {
                length = (length / 4096 + 1) * 4096;
                array = new T[length];
            }
            return array;
        }
    }
}
