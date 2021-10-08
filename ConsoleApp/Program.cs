using System;
using OpenCvSharp;
using VisualServoCore;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {

            // Mock Taueki
            // ↓拾ってください
            Errors _errors;

            ImageStream cap = new(0);
            ErrorEstimator estimator = new();
            VideoRecorder writer = null;
            //writer = new(new(1280, 960));

            var connector = cap.GetStream()
                .Subscribe(frame =>
                {
                    writer?.Write(frame);
                    _errors = estimator.Run(frame);
                    using var radar = estimator.GetGroundCoordinateView();
                    Console.WriteLine($"LateralE = {_errors.LateralError:f2}  : HeadingE = {_errors.HeadingError:f2}");
                    Cv2.ImShow("FRAME", frame);
                    Cv2.ImShow("RADAR", radar);
                    Cv2.WaitKey(1);
                });

            while (Console.ReadKey().Key is not ConsoleKey.Enter) ;
            connector.Dispose();
            cap.Dispose();
            writer?.Dispose();

        }
    }
}
