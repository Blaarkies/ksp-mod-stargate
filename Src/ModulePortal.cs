using System;
using Stargate.Interface;
using Stargate.Utilities;

namespace Stargate
{
    public class ModulePortal : PartModule
    {
        [KSPField(
            guiName = "Target Stargate",
            groupName = "stargate",
            guiActive = true,
            unfocusedRange = 200f,
            guiActiveUnfocused = true)]
        private string _selectedStargateName = "None";

        private StargateSelector _stargateSelector;
        private ResourceConsumer _nqConsumer;
        private ResourceConsumer _ecConsumerDialing;
        private StargateDialer _stargateDialer;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            _stargateSelector = new StargateSelector(
                newName => _selectedStargateName = newName,
                vessel);

            _nqConsumer = new ResourceConsumer(
                "Naquadah",
                part,
                new ResourceConsumerConfig
                {
                    costPerTon = 1,
                    costPerSecondActive = 1, //0.01,
                    costToActivate = 2,
                    minimumReserveToActivate = 1,
                    onRanOutOfResource =
                        () => BlaarkiesLog.OnScreen($"Ran out of Naquadah"),
                });

            _ecConsumerDialing = new ResourceConsumer(
                "ElectricCharge",
                part,
                new ResourceConsumerConfig
                {
                    costPerSecondActive = 4,
                    minimumReserveToActivate = 80,
                    onRanOutOfResource =
                        () => BlaarkiesLog.OnScreen($"Ran out of Electric Charge"),
                });

            _stargateDialer = new StargateDialer(part);
        }

        [KSPEvent(
            groupDisplayName = "Stargate Controls",
            groupStartCollapsed = false,
            groupName = "stargate",
            name = "gateName",
            guiName = "Cycle Next Target",
            guiActive = true,
            active = true,
            unfocusedRange = 200f,
            externalToEVAOnly = false,
            guiActiveUnfocused = true,
            guiActiveUncommand = true,
            requireFullControl = false)]
        public void CycleNextGate()
        {
            _stargateSelector.CycleNextGate();
        }

        [KSPEvent(
            groupName = "stargate",
            name = "gateName",
            guiName = "Cycle Previous Target",
            guiActive = true,
            active = true,
            unfocusedRange = 200f,
            externalToEVAOnly = false,
            guiActiveUnfocused = true,
            guiActiveUncommand = true,
            requireFullControl = false)]
        public void CyclePreviousGate()
        {
            _stargateSelector.CyclePreviousGate();
        }

        [KSPEvent(
            groupName = "stargate",
            name = "teleportButton",
            guiName = "Test Teleport",
            guiActive = true,
            active = true,
            unfocusedRange = 100f,
            externalToEVAOnly = false,
            guiActiveUnfocused = true,
            guiActiveUncommand = true,
            requireFullControl = false)]
        public void TestTeleport()
        {
            var wormhole = new Wormhole(vessel, _stargateSelector);

            if (wormhole.Valid)
            {
                wormhole.TeleportSubject();
            }
        }

        [KSPEvent(
            name = "dialButton",
            guiName = "Dial Target Gate",
            groupName = "stargate",
            guiActive = true,
            active = true,
            unfocusedRange = 100f,
            externalToEVAOnly = false,
            guiActiveUnfocused = true,
            guiActiveUncommand = true,
            requireFullControl = false)]
        public void DialTargetGate()
        {
            var wormhole = new Wormhole(vessel, _stargateSelector);
            if (!wormhole.Valid)
            {
                return;
            }

            var canStartDialSequence = _nqConsumer.HasActivationResources();
            if (!canStartDialSequence)
            {
                BlaarkiesLog.OnScreen($"Not enough Naquadah");
                return;
            }

            _ecConsumerDialing.ConsumeContinuous();

            _stargateDialer.StartDialingSequence(
                () =>
                {
                    BlaarkiesLog.OnScreen($"Dialing complete");

                    _nqConsumer.ConsumeContinuous();
                });
        }

        private void FixedUpdate()
        {
            _stargateDialer.Update();
            _ecConsumerDialing.Update();
            _nqConsumer.Update();
        }

    }
}
