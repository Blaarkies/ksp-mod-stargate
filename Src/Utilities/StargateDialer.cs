using System;

namespace Stargate.Utilities
{
    public class StargateDialer
    {
        private readonly Part _stargate;
        private bool _isDialing;
        private Action _onComplete;
        private readonly TickerWatch _watch = new TickerWatch();

        public StargateDialer(Part stargate)
        {
            _stargate = stargate;
        }

        public void StartDialingSequence(Action onComplete)
        {
            _onComplete = onComplete;
            _isDialing = true;

            _watch.Start();
        }

        public void Update()
        {
            if (!_isDialing)
            {
                return;
            }

            if (_watch.TimeSeconds > 2)
            {
                BlaarkiesLog.OnScreen($"Chevron 1 engaged");
            }

            if (_watch.TimeSeconds > 4)
            {
                BlaarkiesLog.OnScreen($"Chevron 2 engaged");
            }

            if (_watch.TimeSeconds > 6)
            {
                BlaarkiesLog.OnScreen($"Chevron 7 locked");
            }

            if (_watch.TimeSeconds > 8)
            {
                BlaarkiesLog.OnScreen($"Kawoosh");

                _onComplete?.Invoke();
                _isDialing = true;

                CreateEventHorizon();

                _watch.Stop();
            }

            _watch.Tick();
        }

        private void CreateEventHorizon()
        {
            _stargate.explode();
        }
    }
}
