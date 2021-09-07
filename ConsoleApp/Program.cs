using System;
using OpenCvSharp;
using Husty.OpenCvSharp.DepthCamera;
using VisualServoCore.Vision;
using VisualServoCore.Controller;
using VisualServoCore.Communication;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {

            //IVision<Mat> cap = new DummyBGRStream();
            //IController<Mat> controller = new DummyColorBasedController();

            //IDepthCamera camera = new Realsense(new(640, 360), 10);
            IVision<BgrXyzMat> cap = new DummyBGRXYZStream();
            IController<BgrXyzMat> controller = new DummyDepthFusedController();

            ICommunication<string> server = new DummyCommunication();


            var connector = cap.Connect()
                .Subscribe(frame =>
                {
                    var (speed, steer) = controller.Run(frame);
                    server.Send($"{speed:f2},{steer:f2}");
                    Console.WriteLine($"Speed: {speed:f2} km/h, Steer: {steer:f2} rad");
                    Cv2.ImShow(" ", frame.BGR);
                    Cv2.WaitKey(1);
                });

            Console.ReadKey();
            connector.Dispose();
            cap.Disconnect();
            server.Dispose();

        }
    }
}
