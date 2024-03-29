﻿using Stargate.Domain;
using UnityEngine;

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
        private readonly string _resourceName;
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
            _resourceName = resourceName;
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

        public bool ConsumeUnitPerTon(double tonMeasure)
        {
            // TODO: test if resources available. if not, don't even consume, just return false
            var demand = tonMeasure * _config.costPerTon;
            var consumed = _part.RequestResource(_resourceId, demand);
            if (consumed < demand * .9)
            {
                BlaarkiesLog.Debug($"Consumed only [{consumed}] {_resourceName}. Attempted {demand}");
                _config.onRanOutOfResource?.Invoke();
                return false;
            }

            return true;
        }

        public void ConsumeContinuous()
        {
            if (_isConsuming)
            {
                return;
            }

            _isConsuming = true;
            _watch.Start();
        }


        public void Update()
        {
            if (!_isConsuming)
            {
                return;
            }

            _watch.Tick();

            var consumed = _part.RequestResource(_resourceId, _costPerTickActive);
            if (consumed < _costPerTickActive * .9)
            {
                Debug.Log($"Consumed only {consumed} {_resourceName}. Attempted {_costPerTickActive}");
                _isConsuming = false;
                _config.onRanOutOfResource?.Invoke();
                _watch.Stop();
            }
        }

        public void Stop()
        {
            if (!_isConsuming)
            {
                return;
            }

            _isConsuming = false;
            _watch.Stop();
        }

    }
}
