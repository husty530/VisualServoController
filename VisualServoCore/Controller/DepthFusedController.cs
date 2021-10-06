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
    public class DepthFusedController : IController<BgrXyzMat, short>
    {

        // ------ Fields ------ //

        private readonly double _gain;
        private readonly int _maxWidth;
        private readonly int _maxDistance;
        private readonly YoloDetector _detector;
        private readonly object _locker = new();
        private readonly Radar _radar;
        private Point2f[] _points;
        private Point2f _targetPoint;


        // ------ Constructors ------ //

        public DepthFusedController(double gain, int maxWidth, int maxDistance)
        {

            // initialize detector instance and some parameters

            var cfg = "..\\..\\..\\..\\..\\model\\_.cfg";
            var weights = "..\\..\\..\\..\\..\\model\\_.weights";
            var names = "..\\..\\..\\..\\..\\model\\_.names";
            _detector = new(cfg, weights, names, new(512, 288), 0.5f);
            _gain = gain;                       // for steering
            _maxWidth = maxWidth;               // maximum number of X-axis
            _maxDistance = maxDistance;         // maximum number of Z-axis
            _radar = new(maxWidth, maxDistance);

        }


        // ------ Public Methods ------ //

        public LogObject<short> Run(BgrXyzMat input)
        {
            var boxes = FindBoxes(input);
            _points = boxes.Select(r => r.GetCenter().ToPoint2f()).ToArray();
            _targetPoint = SelectTarget(input, boxes);
            var steer = CalculateSteer(_targetPoint.ToPoint());
            return new(DateTimeOffset.Now, (short)steer);
        }

        public Mat GetGroundCoordinateResults()
        {
            return _radar.GetRadar(_locker, _points, _targetPoint);
        }


        // ------ Private Methods ------ //

        private Rect[] FindBoxes(BgrXyzMat input)
        {
            var w = input.BGR.Width;
            var h = input.BGR.Height;
            var results = _detector.Run(input.BGR);
            var boxes = results.Where(r => r.Label is "person")
                .Where(r => r.Probability > 0.5)
                .Select(r =>
                {
                    r.DrawBox(input.BGR, new(0, 0, 160), 2);
                    return r.Box.Scale(w, h).ToRect();
                })
                .ToArray();
            return boxes;
        }

        private Point SelectTarget(BgrXyzMat input, Rect[] boxes)
        {
            var center = boxes[0].GetCenter();
            var xyz = input.GetPointInfo(center);
            var target = new Point(xyz.X, xyz.Z);
            return target;
        }

        private double CalculateSteer(Point point)
        {
            return 0;
        }

    }
}
