using System;
using System.Diagnostics;

namespace ChroMapper_CameraMovement.Util
{
    internal class DebugTimer
    {
        public Stopwatch timer = new Stopwatch();
        public double _timerMax = 0;
        public double _timerMin = double.MaxValue;
        public double _timerTotal = 0;
        public int _timerCount = 0;

        public void Start()
        {
            this.timer.Start();
        }
        public void Stop()
        {
            timer.Stop();
            if (this._timerMax < timer.Elapsed.TotalMilliseconds)
                this._timerMax = timer.Elapsed.TotalMilliseconds;
            if (this._timerMin > timer.Elapsed.TotalMilliseconds)
                this._timerMin = timer.Elapsed.TotalMilliseconds;
            this._timerTotal += timer.Elapsed.TotalMilliseconds;
            this._timerCount++;
        }
        public void Log(int maxCount)
        {
            if (this._timerCount > maxCount)
            {
                UnityEngine.Debug.Log($"Ave:{Math.Floor(this._timerTotal / this._timerCount * 10000) / 10000}ms \tMax:{this._timerMax}ms \tMin:{this._timerMin}ms");
                this._timerCount = 0;
                this._timerTotal = 0;
                this._timerMax = 0;
                this._timerMin = 0;
            }
        }
    }
}
