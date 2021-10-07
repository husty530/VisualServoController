using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using Husty.OpenCvSharp;

namespace VisualServoCore.Controller
{
    public class ColorBasedController : IController<Mat, double>
    {

        // ------ Fields ------ //

        private readonly Size _size = new(640, 480);
        private readonly IntrinsicCameraParameters _paramIn;
        private readonly ExtrinsicCameraParameters _paramEx;
        private readonly PerspectiveTransformer _transformer;
        private readonly YoloDetector _detector;
        private readonly double _gain;
        private readonly int _maxWidth;
        private readonly int _maxDistance;
        private readonly int _focusWidth;
        private readonly Radar _radar;
        private Point2f[] _points;


        // ------ Constructors ------ //

        public ColorBasedController(double gain, int maxWidth, int maxDistance, int focusWidth)
        {
            _paramIn = IntrinsicCameraParameters.Load("");
            _paramEx = ExtrinsicCameraParameters.Load("");
            _transformer = new(_paramIn.CameraMatrix, _paramEx);

            var cfg = "..\\..\\..\\..\\..\\model\\_.cfg";
            var weights = "..\\..\\..\\..\\..\\model\\_.weights";
            var names = "..\\..\\..\\..\\..\\model\\_.names";
            _detector = new(cfg, weights, names, _size, 0.5f);

            _gain = gain;                       // for steering
            _maxWidth = maxWidth;               // maximum number of X-axis
            _maxDistance = maxDistance;         // maximum number of Z-axis
            _focusWidth = focusWidth;
            _radar = new(maxWidth, maxDistance, focusWidth);
        }


        // ------ Public Methods ------ //

        public LogObject<double> Run(Mat input)
        {
            Cv2.Resize(input, input, _size);
            input = input.Undistort(_paramIn.CameraMatrix, _paramIn.DistortionCoeffs);
            _points = GetPoints(input);
            var steer = CalculateSteer();
            return new(DateTimeOffset.Now, steer);
        }

        public Mat GetGroundCoordinateResults()
        {
            return _radar.GetRadar(_points, null);
        }

        // ------ Private Methods ------ //

        private Point2f[] GetPoints(Mat input)
        {
            var w = input.Width;
            var h = input.Height;
            return _detector.Run(input)
                .Select(r => r.Box.Scale(w, h).ToRect().GetCenter())
                .Select(c => _transformer.ConvertToWorldCoordinate(new((int)c.X, (int)c.Y)))
                .ToArray();
        }

        private double CalculateSteer()
        {
            return 0;
        }

    }
}
