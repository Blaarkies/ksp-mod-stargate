using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Stargate
{
    /// <summary>
    /// Handles the connection and teleportation between 2 stargates.
    /// This entity is intended to live from the gate pre-dial sequence,
    /// up to:
    /// <br/>- The successful teleportation of the subject
    /// <br/>- Or cancellation of the wormhole
    /// </summary>
    public class Wormhole
    {
        private readonly Vessel _subject;
        private readonly Vessel _originGate;
        private readonly Vessel _destinationGate;

        private readonly Dictionary<int, Action<Vector3>> _teleportMethodMap;

        public readonly bool Valid;

        public Wormhole(Vessel originGate, StargateSelector stargateSelector)
        {
            _originGate = originGate;
            _destinationGate = FlightGlobals.FindVessel(stargateSelector.SelectedStargateId);
            if (!_destinationGate)
            {
                BlaarkiesLog.OnScreen($"No other gate selected");
                Valid = false;
                return;
            }

            var gateLocation = _originGate.CoM;
            var gateActiveSize = _originGate.vesselSize.magnitude * 2 + 100f;

            _subject = FlightGlobals.FindNearestVesselWhere(gateLocation,
                    v => v != _originGate && Vector3.Distance(v.CoM, gateLocation) < gateActiveSize)
                .FirstOrDefault();
            if (!_subject)
            {
                BlaarkiesLog.OnScreen($"No vessel in range");
                Valid = false;
                return;
            }

            Valid = true;

            _teleportMethodMap = new Dictionary<int, Action<Vector3>>
            {
                { 0, HandleToOrbitWormhole }, // orbit -> orbit
                { 1, HandleToOrbitWormhole }, // surface -> orbit
                { 2, HandleOrbitToSurfaceWormhole }, // orbit -> surface
                // { 2, HandleOrbitToSurfaceWormhole },
                // { 3, HandleSurfaceToSurfaceWormhole }, // surface -> surface
                { 3, HandleOrbitToSurfaceWormhole }, // surface -> surface
            };
        }

        public void TeleportSubject()
        {
            var gateLocation = _originGate.CoM;
            var relativeOffset = gateLocation - _subject.CoM;

            var methodBitmask = _subject.LandedOrSplashed.ToSign()
                                + 2 * _destinationGate.LandedOrSplashed.ToSign();

            _teleportMethodMap[methodBitmask].Invoke(relativeOffset);
        }

        private void HandleToOrbitWormhole(Vector3 offset)
        {
            var newOrbit = Orbit.OrbitFromStateVectors(
                _destinationGate.CoM - offset,
                _destinationGate.obt_velocity.Clone(),
                _destinationGate.orbit.referenceBody,
                Planetarium.GetUniversalTime()
            );

            SetNewOrbit(newOrbit);
        }

        private void HandleOrbitToSurfaceWormhole(Vector3 offset)
        {
            _subject.Splashed = false;
            _subject.Landed = false;
            _subject.landedAt = null;

            _subject.SetPosition(_destinationGate.CoM - offset);
            _subject.SetWorldVelocity(_destinationGate.obt_velocity - _subject.srf_velocity);

            // TODO: fix parts-jitter after teleport
            // TODO: fix destinationGate bumping up after teleport
            // TODO: fix aero effects 1 frame during teleportation
            _subject.UpdatePosVel();
            _subject.orbitDriver.UpdateOrbit();
        }

        private void SetNewOrbit(Orbit newOrbit)
        {
            _subject.Splashed = false;
            _subject.Landed = false;
            _subject.landedAt = null;

            try
            {
                OrbitPhysicsManager.HoldVesselUnpack(2);
            }
            catch (NullReferenceException)
            {
                Debug.Log("Could not hold vessel unpacked");
                BlaarkiesLog.OnScreen("Wormhole malfunction");
                return;
            }

            FlightGlobals.Vessels.ForEach(v => v.GoOnRails());

            _subject.orbitDriver.orbit.UpdateFromOrbitAtUT(
                newOrbit,
                Planetarium.GetUniversalTime(),
                newOrbit.referenceBody);

        }
    }
}
