using System;
using System.IO;
using System.Text.Json;
using OpenCvSharp;
using Husty.OpenCvSharp.DepthCamera;

namespace VisualServoCore
{
    public class DataLogger : IDisposable
    {

        private readonly string _name;
        private VideoWriter _cwrt;
        private VideoRecorder _dwrt;
        private StreamWriter _sw;

        public DataLogger()
        {
            var t = DateTimeOffset.Now;
            _name = $"{t.Year}{t.Month:d2}{t.Day:d2}{t.Hour:d2}{t.Minute:d2}{t.Second}";
            if (!Directory.Exists("log")) Directory.CreateDirectory("log");
            Directory.CreateDirectory("log\\" + _name);
        }

        public void Write(LogObject data)
        {
            if (_sw == null)
                _sw = new($"log\\{_name}\\{_name}.txt");
            _sw.WriteLine(JsonSerializer.Serialize(data));
        }

        public void Write(Mat frame)
        {
            if (_cwrt == null)
                _cwrt = new($"log\\{_name}\\{_name}.mp4", FourCC.MPG4, 15, new(frame.Width, frame.Height));
            _cwrt.Write(frame);
        }

        public void Write(BgrXyzMat frame)
        {
            if (_dwrt == null)
                _dwrt = new($"log\\{_name}\\{_name}.yms");
            _dwrt.WriteFrames(frame);
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
