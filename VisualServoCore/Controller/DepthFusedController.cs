using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using Husty.OpenCvSharp.DepthCamera;

namespace VisualServoCore.Controller
{
    public class DepthFusedController : IController<BgrXyzMat>
    {

        // ------ Fields ------ //



        // ------ Constructors ------ //

        public DepthFusedController()
        {

            // initialize detector instance and some parameters

            // ---

        }


        // ------ Methods ------ //

        public (double Speed, double Steer) Run(BgrXyzMat input)
        {
            var speed = 0.0;
            var steer = 0.0;

            // generate steering angle from image processing and 3D coordinate information

            // ---

            return (speed, steer);
        }

    }
}
