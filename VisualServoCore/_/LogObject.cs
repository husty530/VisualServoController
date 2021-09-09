using System;
using System.Collections.Generic;
using Husty.OpenCvSharp;

namespace VisualServoCore
{
    public record LogObject<T>(
        DateTimeOffset Time,
        T Speed,
        T Steer,
        IEnumerable<YoloResult> Detections
    )
    { }
}
