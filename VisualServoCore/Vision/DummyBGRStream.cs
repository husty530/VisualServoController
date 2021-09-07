using System;
using System.Linq;
using System.Threading;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using OpenCvSharp;

namespace VisualServoCore.Vision
{
    public class DummyBGRStream : IVision<Mat>
    {

        // ------ Fields ------ //

        private int _frameNumber;
        private readonly bool _isFileSource;
        private readonly object _lockObj = new();
        private readonly Mat _red = new(360, 640, MatType.CV_8UC3, new(0, 0, 180));


        // ------ Properties ------ //

        public double Fps { get; }

        public int FrameCount { get; }

        public Size FrameSize { get; }


        // ------ Constructors ------ //

        public DummyBGRStream(int deviceIndex = 0)
        {
            Fps = 10;
            FrameCount = -1;
            FrameSize = new(640, 360);
            _isFileSource = false;
        }

        public DummyBGRStream(string sourceFile)
        {
            Fps = 10;
            FrameCount = 1000;
            FrameSize = new(640, 360);
            _isFileSource = true;
        }


        // ------ Methods ------ //

        public bool Read(ref Mat frame)
        {
            if (FrameCount != -1 && _frameNumber++ > FrameCount - 1) return false;
            frame = _red;
            Thread.Sleep(1000 / (int)Fps);
            return true;
        }

        public IObservable<Mat> Connect()
        {
            var frame = new Mat();
            if (_isFileSource)
            {
                return Observable.Range(0, FrameCount, ThreadPoolScheduler.Instance)
                    .Select(_ =>
                    {
                        lock (_lockObj)
                        {
                            try
                            {
                                frame = _red;
                                Thread.Sleep(1000 / (int)Fps);
                            }
                            catch { }
                        }
                        return frame;
                    })
                    .Publish().RefCount();
            }
            else
            {
                return Observable.Range(0, int.MaxValue, ThreadPoolScheduler.Instance)
                    .Select(_ =>
                    {
                        lock (_lockObj)
                        {
                            try
                            {
                                frame = _red;
                                Thread.Sleep(1000 / (int)Fps);
                            }
                            catch { }
                        }
                        return frame;
                    })
                    .Publish().RefCount();
            }
        }

        public void Disconnect()
        {

        }

    }
}
