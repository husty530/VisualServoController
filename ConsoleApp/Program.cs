using System;
using System.Linq;
using System.Collections.Generic;
using OpenCvSharp;
using Husty.OpenCvSharp.DepthCamera;
using VisualServoCore;
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

            //IDepthCamera camera = new Realsense(new(640, 360));                           // カメラデバイス

            IVision<BgrXyzMat> cap = new DummyBGRXYZStream();                               // カメラからの映像を流すやつ
            IController<BgrXyzMat, double> controller = new DummyDepthFusedController();    // 制御器本体
            ICommunication<IEnumerable<double>> server = new DummyCommunication();          // 外部と通信するやつ
            DataLogger<double> log = null;
            log = new();                                                                    // 記録が不要ならコメントアウト

            var connector = cap.Connect()
                .Subscribe(frame =>
                {
                    var results = controller.Run(frame);
                    var command = new double[] { results.Speed, results.Steer };
                    server.Send(command);
                    Cv2.ImShow(" ", frame.BGR);
                    Cv2.WaitKey(1);
                    log?.Write(results);
                    log?.Write(frame);
                });

            Console.ReadKey();                                                              // キーを押すと終了
            connector.Dispose();
            cap.Disconnect();
            server.Dispose();
            log?.Dispose();

        }
    }
}
