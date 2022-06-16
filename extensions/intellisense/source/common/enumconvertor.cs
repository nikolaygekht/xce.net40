using System;
using System.Collections;
using System.Collections.Generic;

namespace gehtsoft.xce.intellisense.common
{

    public class EnumeratorConvertor<FROM, TO> : IEnumerator<TO>
           where FROM : TO
    {
        private IEnumerator<FROM> mEnumerator;

        public EnumeratorConvertor(IEnumerator<FROM> enumerator)
        {
            mEnumerator = enumerator;
        }

        public TO Current
        {
            get
            {
                return mEnumerator.Current;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return mEnumerator.Current;
            }
        }

        ~EnumeratorConvertor()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposal)
        {
            if (mEnumerator != null)
                mEnumerator.Dispose();
            mEnumerator = null;
            if (disposal)
                GC.SuppressFinalize(this);
        }

        public bool MoveNext()
        {
            return mEnumerator.MoveNext();
        }

        public void Reset()
        {
            mEnumerator.Reset();
        }
    }

}