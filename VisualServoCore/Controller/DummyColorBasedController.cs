using System;
using OpenCvSharp;

namespace VisualServoCore.Controller
{
    public class DummyColorBasedController : IController<Mat>
    {

        // ------ Fields ------ //

        private readonly Random _randomGenerator = new();


        // ------ Constructors ------ //

        public DummyColorBasedController()
        {

        }


        // ------ Methods ------ //

        public (double Speed, double Steer) Run(Mat input)
        {
            if (input.Empty())
                throw new ArgumentException("Input image is empty!");
            var speed = 1.0;
            var steer = _randomGenerator.NextDouble() - 0.5;
            return (speed, steer);
        }

    }
}
