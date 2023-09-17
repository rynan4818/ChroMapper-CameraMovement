using System;
using System.Diagnostics;

namespace ChroMapper_CameraMovement.Util
{
    internal class TimeoutTimer
    {
        private readonly Stopwatch _timer;
        private readonly long _timeoutTicks;

        public bool HasTimedOut => _timer.ElapsedTicks >= _timeoutTicks;

        public TimeoutTimer(float timeoutSec)
        {
            _timer = new Stopwatch();
            _timer.Start();
            _timeoutTicks = (long)(timeoutSec * TimeSpan.TicksPerSecond);
        }

        public void Stop()
        {
            _timer.Stop();
        }
    }
}
