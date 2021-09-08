using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualServoCore.Communication
{
    public class CAN : ICommunication<IEnumerable<byte>>
    {

        // ------ Fields ------ //


        // ------ Constructors ------ //

        public CAN()
        {
            // ---
        }


        // ------ Methods ------ //

        public bool Send(IEnumerable<byte> canMsg)
        {

            // ---

            return false;
        }

        public IEnumerable<byte> Receive()
        {

            // ---

            return new byte[] { 0b0 };
        }

        public void Dispose()
        {

        }

    }
}
