using System;
using OpenCvSharp;
using VisualServoCore;
using VisualServoCore.Vision;
using VisualServoCore.Controller;
using VisualServoCore.Communication;
using Husty.OpenCvSharp;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {

            var gain = 1.0;
            var maxWidth = 3000;
            var maxDistance = 8000;
            var focusWidth = 1500;

            BGRStream cap = new();
            ColorBasedController controller = new(gain, maxWidth, maxDistance, focusWidth);
            DummyCommunication server = new();
            DataLogger<double> log = null;
            log = new();

            var connector = cap.Connect()
                .Subscribe(frame =>
                {
                    var results = controller.Run(frame);
                    server.Send(results.Steer);
                    Cv2.ImShow(" ", frame);
                    Cv2.WaitKey(1);
                    log?.Write(results);
                    log?.Write(frame);
                });

            while (Console.ReadKey().Key is not ConsoleKey.Enter) ;
            connector.Dispose();
            cap.Disconnect();
            server.Dispose();
            log?.Dispose();

        }
    }
}
