using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using Husty.OpenCvSharp;

namespace VisualServoCore.Controller
{
    public class ColorBasedController : IController<Mat, short>
    {

        // ------ Fields ------ //

        private readonly IntrinsicCameraParameters _paramIn;
        private readonly ExtrinsicCameraParameters _paramEx;
        private readonly PerspectiveTransformer _transformer;
        private readonly YoloDetector _detector;


        // ------ Constructors ------ //

        public ColorBasedController()
        {

            // Initialize YOLO
            var cfg = "..\\..\\..\\..\\..\\model\\_.cfg";
            var weights = "..\\..\\..\\..\\..\\model\\_.weights";
            var names = "..\\..\\..\\..\\..\\model\\_.names";
            _detector = new(cfg, weights, names, new(512, 288), 0.5f);

            // Load camera parameters
            //_paramIn = IntrinsicCameraParameters.Load("..\\..\\..\\intrinsic.txt");
            //_paramEx = ExtrinsicCameraParameters.Load("..\\..\\..\\extrinsic.txt");
            //_transformer = new(_paramIn.CameraMatrix, _paramEx);
        }


        // ------ Methods ------ //

        public LogObject<short> Run(Mat input)
        {
            if (input.Empty())
                throw new ArgumentException("Input image is empty!");

            var w = input.Width;
            var h = input.Height;
            //input = input.Undistort(_paramIn.CameraMatrix, _paramIn.DistortionCoeffs);          // 歪み補正

            var results = _detector.Run(input);                                                 // YOLO

            //var points = results
            //    .Select(r => r.ScaledCenter(w, h))                                              // 矩形中心点を列挙して
            //    .Select(c => _transformer.ConvertToWorldCoordinate(new((int)c.X, (int)c.Y)));   // 画像座標→実空間座標に

            var steer = 0;

            Console.WriteLine($"Steer: {steer:f2} deg");

            return new(DateTimeOffset.Now, (short)steer);
        }

    }
}
