using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualServoCore.Communication
{
    public class CAN : ICommunication<byte>
    {

        // ------ Fields ------ //


        // ------ Constructors ------ //

        public CAN()
        {
            // ---
        }


        // ------ Methods ------ //

        public bool Send(byte canMsg)
        {

            // ---

            return false;
        }

        public byte Receive()
        {

            // ---

            return 0b0;
        }

        public void Dispose()
        {

        }

    }
}
