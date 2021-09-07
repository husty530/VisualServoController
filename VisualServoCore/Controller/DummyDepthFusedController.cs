using System;
using OpenCvSharp;
using Husty.OpenCvSharp.DepthCamera;

namespace VisualServoCore.Controller
{
    public class DummyDepthFusedController : IController<BgrXyzMat>
    {

        // ------ Fields ------ //

        private readonly Random _randomGenerator = new();


        // ------ Constructors ------ //

        public DummyDepthFusedController()
        {

        }


        // ------ Methods ------ //

        public (double Speed, double Steer) Run(BgrXyzMat input)
        {
            if (input.Empty())
                throw new ArgumentException("Input image is empty!");
            var speed = 1.0;
            var steer = _randomGenerator.NextDouble() - 0.5;
            return (speed, steer);
        }

    }
}
