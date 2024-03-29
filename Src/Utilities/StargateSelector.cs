﻿using System;
using UniLinq;

namespace Stargate.Utilities
{
    /// <summary>
    /// Handles the targeting and cycling through stargate selections
    /// </summary>
    public class StargateSelector
    {
        public Guid SelectedStargateId;
        private bool CanConnect => SelectedStargateId != Guid.Empty;

        private readonly Action<string> _setSelectedStargate;
        private readonly Vessel _originGate;

        public StargateSelector(
            Action<string> setSelectedStargate,
            Vessel originGate)
        {
            _setSelectedStargate = setSelectedStargate;
            _originGate = originGate;
        }

        private void SetSelectedTarget(Guid targetId, string targetName)
        {
            SelectedStargateId = targetId;
            _setSelectedStargate(targetName);
        }

        private (Guid, string) GetCycledStargateVesselName(int cycleShift)
        {
            // TODO: unloaded vessels don't display part.modules, this prevents searching/filtering to select only
            // stargates. Determine how to flag vessels as stargates (persistent data, or load craft when querying, ...)

            // var stargates = FlightGlobals.Vessels
            //     .Where(v => v.protoVessel.vesselModules.values.Contains(nameof(ModulePortal)));
            // BlaarkiesLog.OnScreen($"{stargates.Count()} stargates found");
            // ProtoPartSnapshot.

            var otherGates = FlightGlobals.Vessels
                .Where(v => v != _originGate
                            && v.vesselType != VesselType.SpaceObject)
                // .Where(v => v != vessel && v.parts.Any(p => p.Modules.Contains(nameof(ModulePortal))))
                .OrderBy(v => v.persistentId);

            if (!otherGates.Any())
            {
                BlaarkiesLog.OnScreen($"No other gates found");
                return (Guid.Empty, "None");
            }

            var indexOfSelection = CanConnect
                ? otherGates.TakeWhile(v => v.id != SelectedStargateId).Count()
                : 0;
            var nextIndex = Cycler.GetShiftFrom(otherGates.Count(), cycleShift, indexOfSelection);
            var newGate = otherGates.ElementAt(nextIndex < 0 ? nextIndex + otherGates.Count() : nextIndex);

            return (newGate.id, newGate.vesselName);
        }

        public void CyclePreviousGate()
        {
            var (targetId, targetName) = GetCycledStargateVesselName(-1);
            SetSelectedTarget(targetId, targetName);
        }

        public void CycleNextGate()
        {
            var (targetId, targetName) = GetCycledStargateVesselName(1);
            SetSelectedTarget(targetId, targetName);
        }
    }
}
