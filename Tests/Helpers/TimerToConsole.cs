using System;
using System.Diagnostics;

namespace Tests.Helpers
{
    class TimerToConsole : IDisposable
    {

        private readonly Stopwatch _timer;

        private readonly string _message;

        public TimerToConsole(string message)
        {
            _message = message;
            _timer = new Stopwatch();
            _timer.Start();
        }

        public void Dispose()
        {
            Console.WriteLine("{0} took {1:f2} ms", _message, 1000.0 * _timer.ElapsedTicks / Stopwatch.Frequency);
        }
    }
}
