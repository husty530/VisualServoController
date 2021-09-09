using System;
using Husty.OpenCvSharp;
using Husty.OpenCvSharp.DepthCamera;

namespace VisualServoCore.Controller
{
    public class DummyDepthFusedController : IController<BgrXyzMat, double>
    {

        // ------ Fields ------ //

        private readonly Random _randomGenerator = new();
        private readonly YoloDetector _detector;

        // ------ Constructors ------ //

        public DummyDepthFusedController()
        {
            var cfg = "..\\..\\..\\..\\..\\model\\_.cfg";
            var weights = "..\\..\\..\\..\\..\\model\\_.weights";
            var names = "..\\..\\..\\..\\..\\model\\_.names";
            _detector = new(cfg, weights, names, new(512, 288), 0.5f);
        }


        // ------ Methods ------ //

        public LogObject<double> Run(BgrXyzMat input)
        {
            if (input.Empty())
                throw new ArgumentException("Input image is empty!");
            var results = _detector.Run(input.BGR);
            var speed = 1;
            var steer = (_randomGenerator.NextDouble() - 0.5) * 100;
            Console.WriteLine($"Speed: {speed} km/h, Steer: {steer} rad");

            return new(DateTimeOffset.Now, speed, steer, results);
        }

    }
}
