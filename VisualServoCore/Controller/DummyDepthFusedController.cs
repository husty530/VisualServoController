using System;
using System.Collections.Generic;
using Husty.OpenCvSharp.DepthCamera;

namespace VisualServoCore.Controller
{
    public class DummyDepthFusedController : IController<BgrXyzMat, IEnumerable<byte>>
    {

        // ------ Fields ------ //

        private readonly Random _randomGenerator = new();


        // ------ Constructors ------ //

        public DummyDepthFusedController()
        {

        }


        // ------ Methods ------ //

        public IEnumerable<byte> Run(BgrXyzMat input)
        {
            if (input.Empty())
                throw new ArgumentException("Input image is empty!");
            byte speed = 1;
            byte steer = (byte)((_randomGenerator.NextDouble() - 0.5) * 100);
            Console.WriteLine($"Speed: {speed} km/h, Steer: {steer} rad");
            return new byte[] { speed, steer };
        }

    }
}
