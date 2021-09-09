using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualServoCore.Communication
{
    public class CAN : ICommunication<IEnumerable<double>>
    {

        // ------ Fields ------ //


        // ------ Constructors ------ //

        public CAN()
        {
            // ---
        }


        // ------ Methods ------ //

        public bool Send(IEnumerable<double> command)
        {

            // ---

            return false;
        }

        public IEnumerable<double> Receive()
        {

            // ---

            return new double[] { 0.0 };
        }

        public void Dispose()
        {

        }

    }
}
