namespace Stargate.Utilities
{
    public class TickerWatch
    {
        public float TimeSeconds => _tick / 60f;

        private long _tick;
        private bool _isRunning;

        public void Start()
        {
            _tick = 0;
            _isRunning = true;
        }

        public void Stop()
        {
            _isRunning = false;
        }

        public void Tick()
        {
            if (!_isRunning)
            {
                return;
            }

            _tick++;
        }
    }
}
