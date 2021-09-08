using System;
using OpenCvSharp;
using Husty.OpenCvSharp.DepthCamera;
using VisualServoCore.Vision;
using VisualServoCore.Controller;
using VisualServoCore.Communication;
using System.Linq;
using System.Collections.Generic;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {

            //IVision<Mat> cap = new DummyBGRStream();
            //IController<Mat> controller = new DummyColorBasedController();

            IDepthCamera camera = new Realsense(new(640, 360));
            IVision<BgrXyzMat> cap = new BGRXYZStream(camera);
            IController<BgrXyzMat, IEnumerable<byte>> controller = new DepthFusedController();
            ICommunication<IEnumerable<byte>> server = new DummyCommunication();


            var connector = cap.Connect()
                .Subscribe(frame =>
                {
                    var commands = controller.Run(frame);
                    server.Send(commands);
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
