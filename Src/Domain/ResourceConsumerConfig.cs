using System;

namespace Stargate.Domain
{
    public class ResourceConsumerConfig
    {
        public double costPerTon { get; set; } = 0;
        public double costPerSecondActive { get; set; } = 0;
        public double costToActivate { get; set; } = 0;
        public double minimumReserveToActivate { get; set; } = 0;

        public Action onRanOutOfResource { get; set; }
    }
}
