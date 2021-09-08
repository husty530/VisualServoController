using System;
using System.Collections.Generic;

namespace VisualServoCore.Communication
{
    public class DummyCommunication : ICommunication<IEnumerable<byte>>
    {

        // ------ Fields ------ //

        private byte _count;


        // ------ Constructors ------ //

        public DummyCommunication()
        {

        }


        // ------ Methods ------ //

        public bool Send(IEnumerable<byte> sendmsg)
        {
            Console.Write("-->");
            foreach (var s in sendmsg)
                Console.WriteLine($"{ s}");
            return true;
        }

        public IEnumerable<byte> Receive()
        {
            if (_count > 255) _count = 0;
            return new byte[] { _count++ };
        }

        public void Dispose()
        {

        }

    }
}
