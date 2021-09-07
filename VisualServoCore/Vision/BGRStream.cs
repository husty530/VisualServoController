using System;
using System.Linq;
using System.Threading;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using OpenCvSharp;

namespace VisualServoCore.Vision
{
    public class BGRStream : IVision<Mat>
    {

        // ------ Fields ------ //

        private readonly VideoCapture _cap;
        private readonly bool _isFileSource;
        private readonly object _lockObj = new();


        // ------ Properties ------ //

        public double Fps { get; }

        public int FrameCount { get; }

        public Size FrameSize { get; }


        // ------ Constructors ------ //

        public BGRStream(int deviceIndex = 0)
        {
            _cap = new(deviceIndex);
            Fps = _cap.Fps;
            FrameCount = -1;
            FrameSize = new(_cap.FrameWidth, _cap.FrameHeight);
            _isFileSource = false;
        }

        public BGRStream(string sourceFile)
        {
            _cap = new(sourceFile);
            Fps = _cap.Fps;
            FrameCount = _cap.FrameCount;
            FrameSize = new(_cap.FrameWidth, _cap.FrameHeight);
            _isFileSource = true;
        }


        // ------ Methods ------ //

        public bool Read(ref Mat frame)
        {
            return _cap.Read(frame);
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
                                _cap?.Read(frame);
                                Thread.Sleep(1000 / (int)Fps);
                            }
                            catch { }
                        }
                        return frame;
                    }).Publish().RefCount();
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
                                _cap?.Read(frame);
                            }
                            catch { }
                        }
                        return frame;
                    }).Publish().RefCount();
            }
        }

        public void Disconnect()
        {
            _cap?.Dispose();
        }

    }
}
