using System;
using System.Reactive.Linq;
using VisionLib;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var composer = new VisualComposer(true, false);
            Observable.Interval(TimeSpan.FromMilliseconds(100))
                .Subscribe(async _ =>
                {
                    var errors = await composer.GetCurrentErrors();
                    Console.WriteLine($"LateralE = {errors.LateralError:f2}  : HeadingE = {errors.HeadingError:f2}");
                });
            while (Console.ReadKey().Key is not ConsoleKey.Enter) ;
            composer.Dispose();
        }
    }
}
