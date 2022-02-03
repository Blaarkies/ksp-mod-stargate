using Stargate.Interface;

namespace Stargate.Utilities
{
    /// <summary>
    /// Handles the consumption of a resource, detects when it has run out, or when there is not enough to start with.
    /// </summary>
    public class ResourceConsumer
    {
        private readonly double _costPerTickActive;
        private readonly ResourceConsumerConfig _config;

        private readonly int _resourceId;
        private readonly Part _part;

        private bool _isConsuming = false;
        private readonly TickerWatch _watch = new TickerWatch();

        private double ResourceAvailable
        {
            get
            {
                _part.GetConnectedResourceTotals(
                    _resourceId,
                    out var available,
                    out var max);
                return available;
            }
        }

        public ResourceConsumer(
            string resourceName,
            Part part,
            ResourceConsumerConfig config)
        {
            _part = part;
            var definition = PartResourceLibrary.Instance
                .GetDefinition(resourceName);
            _resourceId = definition.id;
            _config = config;

            _costPerTickActive = config.costPerSecondActive / 60;
        }

        public bool HasActivationResources()
        {
            var available = ResourceAvailable;

            var totalActivationCost = _config.costToActivate
                                      + _config.minimumReserveToActivate;

            return available >= totalActivationCost;
        }

        public void ConsumeContinuous()
        {
            _isConsuming = true;
            _watch.Start();
        }


        public void Update()
        {
            if (!_isConsuming)
            {
                return;
            }

            var consumed = _part.RequestResource(_resourceId, _costPerTickActive);

            _watch.Tick();

            if (consumed < _costPerTickActive)
            {
                _isConsuming = false;
                _config.onRanOutOfResource?.Invoke();
                _watch.Start();
            }
        }
    }
}
