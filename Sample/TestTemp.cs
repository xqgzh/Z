using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sample
{
    class SynchronizedHandler<T>
    {
        private Func<T> Worker;

        public SynchronizedHandler(Func<T> functioner)
        {
            Worker.BeginInvoke(Result, null);
        }

        public void Result(IAsyncResult result)
        {
            
        }

        
    }
}
