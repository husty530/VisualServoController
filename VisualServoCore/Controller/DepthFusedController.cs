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
        private Point[] _points;
        private Point? _targetPoint;


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
            _points = FindBoxes(input).Select(r => GetXZ(input, r)).Where(xz => xz is not null).Select(xz => (Point)xz).ToArray();
            _targetPoint = SelectTarget(input, _points) ?? null;
            if (_targetPoint is Point target)
            {
                var steer = CalculateSteer(target);
                return new(DateTimeOffset.Now, (short)steer);
            }
            return new(DateTimeOffset.Now, 0);
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

        private Point? GetXZ(BgrXyzMat input, Rect box)
        {
            try
            {
                var targetX = 0;
                var targetZ = 0;
                var count = 0;
                var center = box.GetCenter();
                for (int y = center.Y - 2; y < center.Y + 2; y++)
                {
                    for (int x = center.X - 2; x < center.X + 2; x++)
                    {
                        var xyz = input.GetPointInfo(new(x, y));
                        if (xyz.Z is not 0)
                        {
                            targetX += (int)xyz.X;
                            targetZ += (int)xyz.Z;
                            count++;
                        }
                    }
                }
                if (count is 0) throw new Exception();
                return new(targetX / count, targetZ / count);
            }
            catch
            {
                return null;
            }
        }

        private Point? SelectTarget(BgrXyzMat input, Point[] points)
        {
            if (points.Length is 0) return null;
            var target = points[0];
            return target;
        }

        private double CalculateSteer(Point point)
        {
            return 0;
        }

    }
}
