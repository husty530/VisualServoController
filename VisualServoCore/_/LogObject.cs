using System;
using System.Collections.Generic;
using Husty.OpenCvSharp;

namespace VisualServoCore
{
    public record LogObject(
        DateTimeOffset Time,
        byte Speed,
        byte Steer,
        IEnumerable<YoloResult> Detections
    )
    { }
}
