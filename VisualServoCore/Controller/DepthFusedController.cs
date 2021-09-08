using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using Husty.OpenCvSharp;
using Husty.OpenCvSharp.DepthCamera;

namespace VisualServoCore.Controller
{
    public class DepthFusedController : IController<BgrXyzMat, LogObject>
    {

        // ------ Fields ------ //

        private readonly YoloDetector _detector;

        // ------ Constructors ------ //

        public DepthFusedController()
        {

            // initialize detector instance and some parameters

            var cfg = "..\\..\\..\\..\\..\\model\\_.cfg";
            var weights = "..\\..\\..\\..\\..\\model\\_.weights";
            var names = "..\\..\\..\\..\\..\\model\\_.names";
            _detector = new(cfg, weights, names, new(512, 288), 0.5f);

        }


        // ------ Methods ------ //

        public LogObject Run(BgrXyzMat input)
        {

            var w = input.BGR.Width;
            var h = input.BGR.Height;
            var results = _detector.Run(input.BGR);
            var targets = results.Where(r => r.Label == "person")
                .Where(r => r.Probability > 0.5)
                .Select(r =>
                {
                    r.DrawBox(input.BGR, new(0, 0, 160), 2);
                    return r.ScaledCenter(w, h);
                })
                .Select(r => input.GetPointInfo(r).Vector3);

            // generate steering angle from image processing and 3D coordinate information

            byte speed = 0;
            byte steer = 0;

            foreach (var t in targets)
            {

                Console.WriteLine($"XYZ = {t.X}, {t.Y}, {t.Z}");

            }
            Console.WriteLine($"Speed: {speed} km/h, Steer: {steer} deg");

            return new(DateTimeOffset.Now, speed, steer, results);
        }

    }
}
