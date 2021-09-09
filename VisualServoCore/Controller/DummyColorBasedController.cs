using System;
using OpenCvSharp;
using Husty.OpenCvSharp;

namespace VisualServoCore.Controller
{
    public class DummyColorBasedController : IController<Mat, double>
    {

        // ------ Fields ------ //

        private readonly Random _randomGenerator = new();
        private readonly YoloDetector _detector;


        // ------ Constructors ------ //

        public DummyColorBasedController()
        {
            var cfg = "..\\..\\..\\..\\..\\model\\_.cfg";
            var weights = "..\\..\\..\\..\\..\\model\\_.weights";
            var names = "..\\..\\..\\..\\..\\model\\_.names";
            _detector = new(cfg, weights, names, new(512, 288), 0.5f);
        }


        // ------ Methods ------ //

        public LogObject<double> Run(Mat input)
        {
            if (input.Empty())
                throw new ArgumentException("Input image is empty!");
            var results = _detector.Run(input);
            var speed = 1.0;
            var steer = (_randomGenerator.NextDouble() - 0.5) * 100;
            Console.WriteLine($"Speed: {speed:f2} km/h, Steer: {steer:f2} deg");

            return new(DateTimeOffset.Now, speed, steer, results);
        }

    }
}
