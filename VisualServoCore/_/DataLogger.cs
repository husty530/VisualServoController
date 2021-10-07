using System;
using System.IO;
using System.Text.Json;
using OpenCvSharp;

namespace VisualServoCore
{
    public class DataLogger<T> : IDisposable
    {

        private readonly string _name;
        private VideoWriter _cwrt;
        private StreamWriter _sw;

        public DataLogger(Size? size)
        {
            var t = DateTimeOffset.Now;
            _name = $"{t.Year}{t.Month:d2}{t.Day:d2}{t.Hour:d2}{t.Minute:d2}{t.Second}";
            if (!Directory.Exists("log")) Directory.CreateDirectory("log");
            Directory.CreateDirectory("log\\" + _name);
            _sw = new($"log\\{_name}\\{_name}.json");
            if (size is not null)
                _cwrt = new($"log\\{_name}\\{_name}.mp4", FourCC.MPG4, 15, (Size)size);
        }

        public void Write(LogObject<T> data)
        {
            if (_sw?.BaseStream is not null)
                _sw?.WriteLine(JsonSerializer.Serialize(data));
        }

        public void Write(Mat frame)
        {
            if (!(bool)_cwrt?.IsDisposed)
                _cwrt?.Write(frame);
        }

        public void Dispose()
        {
            _sw?.Close();
            _sw?.Dispose();
            _cwrt?.Release();
            _cwrt?.Dispose();
            _sw = null;
            _cwrt = null;
        }

    }
}
