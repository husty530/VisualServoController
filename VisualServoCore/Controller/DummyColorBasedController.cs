using System;
using System.Collections.Generic;
using OpenCvSharp;

namespace VisualServoCore.Controller
{
    public class DummyColorBasedController : IController<Mat, IEnumerable<double>>
    {

        // ------ Fields ------ //

        private readonly Random _randomGenerator = new();


        // ------ Constructors ------ //

        public DummyColorBasedController()
        {

        }


        // ------ Methods ------ //

        public IEnumerable<double> Run(Mat input)
        {
            if (input.Empty())
                throw new ArgumentException("Input image is empty!");
            var speed = 1.0;
            var steer = _randomGenerator.NextDouble() - 0.5;
            Console.WriteLine($"Speed: {speed:f2} km/h, Steer: {steer:f2} deg");
            return new double[]{ speed, steer };
        }

    }
}
