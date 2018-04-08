using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace mDNS
{
    class Program
    {
        static void Main(string[] args)
        {
            mDNSResponder responder = new mDNSResponder();

            responder.start();
            Thread.Sleep(10000);
            responder.stop();
        }
    }
}
