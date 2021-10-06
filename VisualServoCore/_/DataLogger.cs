using System;
using System.IO;
using System.Text.Json;
using OpenCvSharp;
using Husty.OpenCvSharp.DepthCamera;

namespace VisualServoCore
{
    public class DataLogger<T> : IDisposable
    {

        private readonly string _name;
        private VideoWriter _cwrt;
        private BgrXyzRecorder _dwrt;
        private StreamWriter _sw;

        public DataLogger()
        {
            var t = DateTimeOffset.Now;
            _name = $"{t.Year}{t.Month:d2}{t.Day:d2}{t.Hour:d2}{t.Minute:d2}{t.Second}";
            if (!Directory.Exists("log")) Directory.CreateDirectory("log");
            Directory.CreateDirectory("log\\" + _name);
        }

        public void Write(LogObject<T> data)
        {
            if (_sw is null)
                _sw = new($"log\\{_name}\\{_name}.json");
            _sw.WriteLine(JsonSerializer.Serialize(data));
        }

        public void Write(Mat frame)
        {
            if (_cwrt is null)
                _cwrt = new($"log\\{_name}\\{_name}.mp4", FourCC.MPG4, 15, new(frame.Width, frame.Height));
            _cwrt.Write(frame);
        }

        public void Write(BgrXyzMat frame)
        {
            if (_dwrt is null)
                _dwrt = new($"log\\{_name}\\{_name}.yms");
            _dwrt.WriteFrame(frame);
        }

        public void Dispose()
        {
            _sw?.Dispose();
            _cwrt?.Dispose();
            _dwrt?.Dispose();
            _sw = null;
            _cwrt = null;
            _dwrt = null;
        }

    }
}
