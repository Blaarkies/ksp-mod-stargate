using System;
using Stargate.Interface;

namespace Stargate
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
        private long _tick = 0;

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
            this._part = part;
            var definition = PartResourceLibrary.Instance
                .GetDefinition(resourceName);
            _resourceId = definition.id;
            this._config = config;

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
            _tick = 0;
            BlaarkiesLog.OnScreen($"Consuming...");
        }


        public void update()
        {
            if (!_isConsuming)
            {
                return;
            }

            var consumed = _part.RequestResource(_resourceId, _costPerTickActive);

            _tick++;

            if (consumed == 0)
            {
                _isConsuming = false;
                _config.onRanOutOfResource?.Invoke();
            }
        }
    }
}
