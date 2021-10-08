using System;
using System.Linq;
using System.Collections.Generic;
using OpenCvSharp;
using Husty.OpenCvSharp;

namespace VisualServoCore
{

    public record Errors(double LateralError, double HeadingError);

    public class ErrorEstimator
    {

        // ------ Fields ------ //

        private readonly Size _size = new(1280, 960);
        private readonly IntrinsicCameraParameters _paramIn;
        private readonly ExtrinsicCameraParameters _paramEx;
        private readonly PerspectiveTransformer _transformer;
        private readonly YoloDetector _detector;
        private const int _maxWidth = 3000;
        private const int _maxDistance = 8000;
        private const int _focusWidth = 1500;
        private readonly Radar _radar;
        private Point2f[] _points;


        // ------ Constructors ------ //

        public ErrorEstimator()
        {
            _paramIn = IntrinsicCameraParameters.Load("..\\..\\..\\..\\calib\\dummy-intrinsic.json");
            _paramEx = ExtrinsicCameraParameters.Load("..\\..\\..\\..\\calib\\dummy-extrinsic.json");
            _transformer = new(_paramIn.CameraMatrix, _paramEx);

            var cfg = "..\\..\\..\\..\\model\\_.cfg";
            var weights = "..\\..\\..\\..\\model\\_.weights";
            var names = "..\\..\\..\\..\\model\\_.names";
            _detector = new(cfg, weights, names, _size, 0.5f);

            _radar = new(_maxWidth, _maxDistance, _focusWidth);
        }


        // ------ Public Methods ------ //

        public Errors Run(Mat frame)
        {
            Cv2.Resize(frame, frame, _size);
            using var copy = frame.Clone();
            Cv2.Undistort(copy, frame, _paramIn.CameraMatrix, _paramIn.DistortionCoeffs);
            _points = GetPoints(frame);
            return DoEstimateErrors(_points);
        }

        public Mat GetGroundCoordinateView()
        {
            return _radar.GetRadar(_points, null);
        }

        // ------ Private Methods ------ //

        private Point2f[] GetPoints(Mat input)
        {
            var w = input.Width;
            var h = input.Height;
            return _detector.Run(input)
                .Select(r =>
                {
                    r.DrawCenterPoint(input, new(0, 0, 180), 3);
                    return r.Box.Scale(w, h).ToRect().GetCenter();
                })
                .Select(c => _transformer.ConvertToWorldCoordinate(new(c.X, c.Y)))
                .ToArray();
        }

        private Errors DoEstimateErrors(IEnumerable<Point2f> points)
        {
            var errors = new Errors(0, 0);
            return errors;
        }

    }
}
