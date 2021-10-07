using System;
using System.Collections.Generic;

namespace VisualServoCore.Communication
{
    public class DummyCommunication : ICommunication<double>
    {

        // ------ Fields ------ //

        private byte _count;


        // ------ Constructors ------ //

        public DummyCommunication()
        {

        }


        // ------ Methods ------ //

        public bool Send(double sendmsg)
        {
            Console.Write("-->");
            Console.WriteLine($"{sendmsg}");
            return true;
        }

        public double Receive()
        {
            if (_count > 255) _count = 0;
            return _count++;
        }

        public void Dispose()
        {

        }

    }
}
